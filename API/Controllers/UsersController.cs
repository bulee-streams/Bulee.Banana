using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using API.Models;
using API.Extensions;
using API.Models.ViewModels;
using AutoMapper;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegiserViewModel data)
        {
            var user = mapper.Map<User>(data);
            user.TimeAdded = DateTime.Now;

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

            return Created("api/v1/users/register", "You've been registered");
        }
    }
}