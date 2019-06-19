using System;
using System.Threading.Tasks;
using API.Models;
using API.Controllers;
using API.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using AutoMapper;
using FluentAssertions;

namespace API.UnitTests
{
    public class UsersControllerTests
    {
        private class Arrangement
        {
            public IMapper Mapper { get; }

            public UsersController SUT { get; }

            public RegiserViewModel User { get; }

            public UserManager<User> UserManager { get; }

            public ILogger<UsersController> Logger { get; }


            public Arrangement(IMapper mapper, ILogger<UsersController> logger, RegiserViewModel user, UserManager<User> userManager)
            {
                User = user;
                Mapper = mapper;
                Logger = logger;
                UserManager = userManager;
                SUT = new UsersController(Mapper, UserManager, Logger);
            }
        }

        private class ArrangementBuilder
        {
           private RegiserViewModel user;
           private Mock<IMapper> mapper = new Mock<IMapper>();
           private static Mock<IUserStore<User>> mockUserStore = new Mock<IUserStore<User>>();
           private Mock<UserManager<User>> userManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            private readonly string username = "username";
            private readonly string email = "user@email.com";
            private readonly string password = "passworD01";

        public ArrangementBuilder WithUser()
        {
          user = new RegiserViewModel()
          {
              UserName = username,
              Email = email,
              Password = password
          };

            return this;
        }

        public ArrangementBuilder WithMapper()
        {
           mapper = new Mock<IMapper>();
          
           mapper.Setup(x => x.Map<User>(It.IsAny<RegiserViewModel>()))
                 .Returns((RegiserViewModel source) =>
                 {
                       var user = new User()
                       {
                         UserName = username,
                         Email = email,
                       };
                       return user;
                 });

           return this;
        }

        public ArrangementBuilder WithSuccessfulUserManager()
        {
           userManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
           return this;
        }

        public ArrangementBuilder WithUnSuccessfulUserManager()
        {
           userManager.Setup(m => m.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());
           return this;
        }

        public Arrangement Build()
            {
                var logger = new Mock<ILogger<UsersController>>();
                return new Arrangement(mapper.Object, logger.Object, user, userManager.Object);
            }
        }

        [Fact]
        public void Ctor_WithNullLogger_ShouldThrowException()
        {
            // Arrange 
            var arrangement = new ArrangementBuilder()
                              .WithMapper()
                              .WithSuccessfulUserManager()
                              .Build();

            // Act
            var error = Record.Exception(() => new UsersController(arrangement.Mapper, 
                                                                   arrangement.UserManager, 
                                                                   null));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null.\r\nParameter name: logger");
        }

        [Fact]
        public void Ctor_WithNullUserManager_ShouldThrowException()
        {
            // Arrange 
            var arrangement = new ArrangementBuilder()
                              .WithMapper()
                              .Build();

            // Act
            var error = Record.Exception(() => new UsersController(arrangement.Mapper,
                                                                   null,
                                                                   arrangement.Logger));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null.\r\nParameter name: userManager");
        }

        [Fact]
        public void Ctor_WithNullUserMapper_ShouldThrowException()
        {
            // Arrange 
            var arrangement = new ArrangementBuilder()
                              .WithSuccessfulUserManager()
                              .Build();

            // Act
            var error = Record.Exception(() => new UsersController(null,
                                                                   arrangement.UserManager,
                                                                   arrangement.Logger));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null.\r\nParameter name: mapper");
        }

        [Fact]
        public async Task CreateUser_DatabaseOnline_ShouldReturnOk()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithUser()
                                .WithMapper()
                                .WithSuccessfulUserManager()
                                .Build();

            // Act 
            var result = await arrangement.SUT.Register(arrangement.User);

            // Assert 
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task CreateUser_DatabaseOffline_ShouldReturnBadRequest()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithUser()
                                .WithMapper()
                                .WithUnSuccessfulUserManager()
                                .Build();

            // Act 
            var result = await arrangement.SUT.Register(arrangement.User);

            // Assert 
            result.Should().BeOfType<BadRequestResult>();
        }
    }
}
