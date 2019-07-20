﻿using System;
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
using System.Linq;
using API.Extensions;

namespace API.UnitTests
{
    public class UsersControllerTests
    {
        private class Arrangement
        {
            public IMapper Mapper { get; }

            public UsersController SUT { get; }

            public RegiserViewModel User { get; }

            public IUserQueries UserQueries { get; }

            public UserManager<User> UserManager { get; }

            public ILogger<UsersController> Logger { get; }


            public Arrangement(IMapper mapper, IUserQueries userQueries, ILogger<UsersController> logger, RegiserViewModel user, UserManager<User> userManager)
            {
                User = user;
                Mapper = mapper;
                Logger = logger;
                UserManager = userManager;
                UserQueries = userQueries;
                SUT = new UsersController(Mapper, UserQueries, UserManager, Logger);
            }
        }

        private class ArrangementBuilder
        {
           private RegiserViewModel user;
           private Mock<IMapper> mapper = new Mock<IMapper>();
           private Mock<IUserQueries> queries = new Mock<IUserQueries>();
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

        public ArrangementBuilder WithSuccessfulUserNameLookUp()
        {
           queries.Setup(q => q.UserNameExist(It.IsAny<UserManager<User>>(), It.IsAny<string>())).Returns(true);
           return this;
        }

        public ArrangementBuilder WithSuccessfulUserEmailLookUp()
        {
           queries.Setup(q => q.EmailExist(It.IsAny<UserManager<User>>(), It.IsAny<string>())).Returns(true);
           return this;
        }

        public Arrangement Build()
            {
                var logger = new Mock<ILogger<UsersController>>();
                return new Arrangement(mapper.Object, queries.Object, logger.Object, user, userManager.Object);
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
                                                                   arrangement.UserQueries,
                                                                   arrangement.UserManager, 
                                                                   null));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null."+ Environment.NewLine +"Parameter name: logger");
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
                                                                   arrangement.UserQueries,
                                                                   null,
                                                                   arrangement.Logger));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null."+ Environment.NewLine +"Parameter name: userManager");
        }

        [Fact]
        public void Ctor_WithNullUserQueries_ShouldThrowException()
        {
            // Arrange 
            var arrangement = new ArrangementBuilder()
                              .WithMapper()
                              .Build();

            // Act
            var error = Record.Exception(() => new UsersController(arrangement.Mapper,
                                                                   null,
                                                                   arrangement.UserManager,
                                                                   arrangement.Logger));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null." + Environment.NewLine + "Parameter name: userQueries");
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
                                                                   arrangement.UserQueries,
                                                                   arrangement.UserManager,
                                                                   arrangement.Logger));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null."+ Environment.NewLine +"Parameter name: mapper");
        }

        [Fact]
        public async Task CreateUser_DatabaseOnline_ShouldReturnCreated()
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
            var resultMessage = result.Should().BeOfType<CreatedResult>().Subject;
            resultMessage.Value.Should().Be("You've been registered");
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
            var resultMessage = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            resultMessage.Value.Should().Be("Sorry you can't be registered at the moment");
        }

        [Fact]
        public async Task CreateUser_UserNameExists_ShouldReturnBadRequest()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithUser()
                                .WithMapper()
                                .WithSuccessfulUserNameLookUp()
                                .WithSuccessfulUserManager()
                                .Build();

            // Act 
            var result = await arrangement.SUT.Register(arrangement.User);

            // Assert 
            var resultMessage = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            resultMessage.Value.Should().Be("Sorry this username has already been registered");
        }

        [Fact]
        public async Task CreateUser_EmailExists_ShouldReturnBadRequest()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithUser()
                                .WithMapper()
                                .WithSuccessfulUserEmailLookUp()
                                .WithSuccessfulUserManager()
                                .Build();

            // Act 
            var result = await arrangement.SUT.Register(arrangement.User);

            // Assert 
            var resultMessage = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            resultMessage.Value.Should().Be("Sorry this email has already been registered");
        }
    }
}
