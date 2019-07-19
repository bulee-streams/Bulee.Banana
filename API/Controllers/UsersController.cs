using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using API.Models;
using API.Models.ViewModels;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    [ApiController]
    [ValidateModel]
    [Route("api/v1/[controller]")]
    public class UsersController : Controller
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly ILogger<UsersController> logger;

        public UsersController(IMapper mapper,
                               UserManager<User> userManager,
                               ILogger<UsersController> logger)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegiserViewModel data)
        {
            var user = mapper.Map<User>(data);
            user.TimeAdded = DateTime.Now;

            if(userManager.UserNameExists(user.UserName)) {
                return BadRequest("sorry this username has already been used");
            }


            if(userManager.EmailExists(user.Email)) {
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