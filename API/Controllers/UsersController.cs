using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.Models.ViewModels;
using API.Repositories.Interfaces;
using API.Email.Interfaces;
using API.Models;

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

        [HttpPost("registration-complete/{token}")]
        public async Task<IActionResult> RegistrationComplete(string token)
        {
            var valid = await userRepository.IsEmailConfirmationValid(token);

            if (!valid) {
                return BadRequest("Sorry this request is invalid");
            }

            return Created("api/v1/users/registration-complete", "Your email has been confirmed");
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegiserViewModel data)
        {
           if(userRepository.DoesUsernameExist(data.UserName)) {
                return BadRequest("Sorry this username has already been used");
           }

           if(userRepository.DoesEmailExist(data.UserName)) {
                return BadRequest("Sorry this email address has already been used");
           }

            var result = await userRepository.Create(data.UserName, data.Email, data.Password);

            if(result == null)
            {
                logger.Log(LogLevel.Error, "User: " + data.UserName + " hasn't been registered");
                return BadRequest("Sorry you can't be registered at the moment");
            }

            //await SendCofirmationEmail(data.UserName, data.Email);

            return Created("api/v1/users/register", "You've been registered");
        }

        //private async Task SendCofirmationEmail(string username, string emailAddress)
        //{
        //    var objectData = new ConfirmationEmail() { UserName = username, Url = "http://suckit.com" };

        //    var result = await email.Send("Bulee Services", username,  emailAddress,
        //                                  "banana@bulee.com", templateId, objectData);

        //    if(result != HttpStatusCode.OK) {
        //        logger.Log(LogLevel.Error, "confirmation email for: " + username + " has not been sent");
        //    }
        //}
    }
}