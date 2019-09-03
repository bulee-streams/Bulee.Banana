using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using API.Helpers;
using API.Email.Interfaces;
using API.Models.ViewModels;
using API.Repositories.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [ValidateModel]
    [Route("api/v1/[controller]")]
    public class UsersController : Controller
    {
        private readonly IEmail email;
        private readonly IUserRepository userRepository;
        private readonly ILogger<UsersController> logger;

        public UsersController(IEmail email,
                               ILogger<UsersController> logger,
                               IUserRepository userRepository)
        {
            this.email = email ?? throw new ArgumentNullException(nameof(email));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpGet("test")]
        public string Test() => "This is a test endpoint";

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Login([FromBody] LoginViewModel data)
        {
            var token = userRepository.Login(data.Username, data.Password);

            if(token == null) {
                return BadRequest("Sorry your username or password is invalid");
            }

            return Ok(token);
        }

        [HttpPost("password-reset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PasswordRest([FromBody] PasswordResetViewModel data)
        {
            if(await userRepository.PasswordReset(data.Token, data.Password) == false) {
                return BadRequest("Sorry the password wasn't reset");
            }

            return Ok();
        }

        [HttpGet("request-password-reset/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestPasswordRest(string username)
        {
            var token = await userRepository.CreatePasswordResetToken(username);

            if(token == Guid.Empty) {
                return BadRequest("Sorry this user doesn't exist");
            }

            return Ok(token.ToString());
        }


        [HttpGet("registration-complete/{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrationComplete(string token)
        {
            var valid = await userRepository.IsEmailConfirmationValid(token);

            if (!valid) {
                return BadRequest("Sorry this request is invalid");
            }

            return Ok();
        }


        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegiserViewModel data)
        {
           if(userRepository.DoesUsernameExist(data.Username)) {
                return BadRequest("Sorry this username has already been used");
           }

           if(userRepository.DoesEmailExist(data.Email)) {
                return BadRequest("Sorry this email address has already been used");
           }

            var result = await userRepository.Create(data.Username, data.Email, data.Password);

            if(result == null)
            {
                logger.Log(LogLevel.Error, "User: " + data.Username + " hasn't been registered");
                return BadRequest("Sorry you can't be registered at the moment");
            }

            var emailResult = await email.SendCofirmationEmail(result.Username, 
                                                               result.Email, 
                                                               result.EmailConfirmationToken);

            if(emailResult != HttpStatusCode.OK) {
                logger.Log(LogLevel.Error, "User: " + data.Username + "registration email hasn't been sent");
            }

            return Created("api/v1/users/register", "You've been registered");
        }
    }
}