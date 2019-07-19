using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using API.Models;
using API.Models.ViewModels;
using AutoMapper;
using Microsoft.Extensions.Logging;

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

            var result = await userManager.CreateAsync(user, data.Password);
            if(!result.Succeeded)
            {
                logger.Log(LogLevel.Error, "User: " + user.UserName + " hasn't been registered");
                return BadRequest();
            }

            return Ok();
        }
    }
}