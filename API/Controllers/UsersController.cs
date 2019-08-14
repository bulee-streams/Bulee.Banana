using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using API.Models;
using API.Extensions;
using API.Models.ViewModels;
using AutoMapper;
using System.Linq;

namespace API.Controllers
{
    [ApiController]
    [ValidateModel]
    [Route("api/v1/[controller]")]
    public class UsersController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUserQueries userQueries;
        private readonly UserManager<User> userManager;
        private readonly ILogger<UsersController> logger;

        public UsersController(IMapper mapper,
                               IUserQueries userQueries,
                               UserManager<User> userManager,
                               ILogger<UsersController> logger)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("test")]
        public string Test() => "This is a test endpoint";

        [HttpGet("registration-complete/{id}")]
        public async Task<IActionResult> RegistrationComplete(string id)
        {
            Guid guidId;
            if(Guid.TryParse(id, out guidId))
            {
                var user = userManager.Users.Where(u => u.Id == guidId).FirstOrDefault();
                if(user != null)
                {
                    user.EmailConfirmed = true;
                    var result = await userManager.UpdateAsync(user);
                    
                    if(result.Succeeded) {
                        return Ok();
                    }
                }
            }

            return BadRequest("Sorry can't complete the account registration");
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegiserViewModel data)
        {
            var user = mapper.Map<User>(data);

            if(userQueries.UserNameExist(userManager, user.UserName)) {
                return BadRequest("Sorry this username has already been registered");
            }


            if(userQueries.EmailExist(userManager, user.Email)) {
                return BadRequest("Sorry this email has already been registered");
            }

            var result = await userManager.CreateAsync(user, data.Password);

            if(!result.Succeeded)
            {
                logger.Log(LogLevel.Error, "User: " + user.UserName + " hasn't been registered");
                return BadRequest("Sorry you can't be registered at the moment");
            }

            var userId = userManager.Users
                                    .Where(u => u.NormalizedUserName == user.NormalizedUserName)
                                    .FirstOrDefault()
                                    .Id;

            return Created("api/v1/users/register", "You've been registered");
        }
    }
}