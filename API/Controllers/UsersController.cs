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
        public async Task<IActionResult> Register([FromForm] RegiserViewModel data)
        {
            var user = mapper.Map<User>(data);

            var result = await userManager.CreateAsync(user, data.Password);
            if(result.Succeeded)
            {
                user.TimeAdded = DateTime.Now;
                logger.Log(LogLevel.Information, "User: " + user.UserName + " has been registered");
            }

            return Ok();
        }
    }
}