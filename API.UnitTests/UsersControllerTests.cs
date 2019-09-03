using System;
using System.Threading.Tasks;
using API.Models;
using API.Controllers;
using API.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using API.Repositories.Interfaces;
using API.Email.Interfaces;

namespace API.UnitTests
{
    public class UsersControllerTests
    {
        private class Arrangement
        {
            public IEmail Email { get; }

            public UsersController SUT { get; }

            public RegiserViewModel User { get; }


            public IUserRepository UserRepository { get; }

            public ILogger<UsersController> Logger { get; }


            public Arrangement(IEmail email, ILogger<UsersController> logger, RegiserViewModel user, IUserRepository userRepository)
            {
                User = user;
                Email = email;
                Logger = logger;
                UserRepository = userRepository;
                SUT = new UsersController(Email, Logger, UserRepository);
            }
        }

        private class ArrangementBuilder
        {
           private RegiserViewModel user;
           private Mock<IUserRepository> userRepository = new Mock<IUserRepository>();

            private readonly string username = "username";
            private readonly string email = "user@email.com";
            private readonly string password = "passworD01";

        public ArrangementBuilder WithUser()
        {
          user = new RegiserViewModel()
          {
              Username = username,
              Email = email,
              Password = password
          };

            return this;
        }



        public ArrangementBuilder WithSuccessfulCreate()
        {
           userRepository.Setup(u => u.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new User());
           return this;
        }

        public ArrangementBuilder WithUnSuccessfulCreate()
        {
                userRepository.Setup(u => u.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((User)null);
                return this;
        }

        public ArrangementBuilder WithSuccessfulUserNameLookUp()
        {
           userRepository.Setup(u => u.DoesUsernameExist(It.IsAny<string>())).Returns(true);
           return this;
        }

        public ArrangementBuilder WithSuccessfulUserEmailLookUp()
        {
           userRepository.Setup(u => u.DoesEmailExist(It.IsAny<string>())).Returns(true);
           return this;
        }

        public ArrangementBuilder WithEmailConfirmValidIsValid()
        {
           userRepository.Setup(u => u.IsEmailConfirmationValid(It.IsAny<string>())).ReturnsAsync(true);
           return this;
        }

        public ArrangementBuilder WithCorrectLogin()
        {
           userRepository.Setup(u => u.Login(It.IsAny<string>(), It.IsAny<string>())).Returns("token");
           return this;
        }

        public ArrangementBuilder WithInCorrectLogin()
        {
           userRepository.Setup(u => u.Login(It.IsAny<string>(), It.IsAny<string>())).Returns((string)null);
           return this;
        }

        public ArrangementBuilder WithValidPasswordResetRequest()
        {
           userRepository.Setup(u => u.CreatePasswordResetToken(It.IsAny<string>())).ReturnsAsync(Guid.NewGuid());
           return this;
        }

        public ArrangementBuilder WithInValidPasswordResetRequest()
        {
           userRepository.Setup(u => u.CreatePasswordResetToken(It.IsAny<string>())).ReturnsAsync(Guid.Empty);
           return this;
        }

        public ArrangementBuilder WithValidPasswordReset()
        {
           userRepository.Setup(u => u.PasswordReset(It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(true);
           return this;
        }

        public Arrangement Build()
            {
                var logger = new Mock<ILogger<UsersController>>();
                var emailSender = new Mock<IEmail>();
                return new Arrangement(emailSender.Object, logger.Object, user, userRepository.Object);
            }
        }

        [Fact]
        public void Ctor_WithNullEmail_ShouldThrowException()
        {
            // Arrange 
            var arrangement = new ArrangementBuilder()
                              .Build();

            // Act
            var error = Record.Exception(() => new UsersController(null,
                                                                   arrangement.Logger,
                                                                   arrangement.UserRepository));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null." + Environment.NewLine + "Parameter name: email");
        }

        [Fact]
        public void Ctor_WithNullLogger_ShouldThrowException()
        {
            // Arrange 
            var arrangement = new ArrangementBuilder()
                              .Build();

            // Act
            var error = Record.Exception(() => new UsersController(arrangement.Email,
                                                                   null,
                                                                   arrangement.UserRepository));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null."+ Environment.NewLine +"Parameter name: logger");
        }

        [Fact]
        public void Ctor_WithNullUserRespository_ShouldThrowException()
        {
            // Arrange 
            var arrangement = new ArrangementBuilder()
                              .Build();

            // Act
            var error = Record.Exception(() => new UsersController(arrangement.Email,
                                                                  arrangement.Logger,
                                                                  null));

            // Assert
            error.Should().BeOfType<ArgumentNullException>();
            error.Message.Should().Be("Value cannot be null."+ Environment.NewLine +"Parameter name: userRepository");
        }

        [Fact]
        public async Task CreateUser_DatabaseOnline_ShouldReturnCreated()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithUser()
                                .WithSuccessfulCreate()
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
                                .WithUnSuccessfulCreate()
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
                                .WithSuccessfulUserNameLookUp()
                                .Build();

            // Act 
            var result = await arrangement.SUT.Register(arrangement.User);

            // Assert 
            var resultMessage = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            resultMessage.Value.Should().Be("Sorry this username has already been used");
        }

        [Fact]
        public async Task CreateUser_EmailExists_ShouldReturnBadRequest()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithUser()
                                .WithSuccessfulUserEmailLookUp()
                                .Build();

            // Act 
            var result = await arrangement.SUT.Register(arrangement.User);

            // Assert 
            var resultMessage = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            resultMessage.Value.Should().Be("Sorry this email address has already been used");
        }

        [Fact]
        public async Task RegistrationComplete_TokenIsValid_ShouldReturnOk()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithEmailConfirmValidIsValid()
                                .Build();
            // Act 
            var result = await arrangement.SUT.RegistrationComplete("token");

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task RegistrationComplete_TokenIsInValid_ShouldReturnBadRequest()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .Build();
            // Act 
            var result = await arrangement.SUT.RegistrationComplete("token");

            // Assert
            var resultMessage = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            resultMessage.Value.Should().Be("Sorry this request is invalid");
        }

        [Fact]
        public void Login_WithCorrectDetails_ShouldReturnOk()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithCorrectLogin()
                                .Build();

            var data = new LoginViewModel() { Username = "username", Password = "password" };

            // Act 
            var result = arrangement.SUT.Login(data);

            // Assert
            var resultMessage = result.Should().BeOfType<OkObjectResult>().Subject;
            resultMessage.Value.Should().Be("token");
        }

        [Fact]
        public void Login_WithInCorrectDetails_ShouldReturnBadRequest()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithInCorrectLogin()
                                .Build();

            var data = new LoginViewModel() { Username = "username", Password = "password" };

            // Act 
            var result = arrangement.SUT.Login(data);

            // Assert
            var resultMessage = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            resultMessage.Value.Should().Be("Sorry your username or password is invalid");
        }

        [Fact]
        public async Task RequestPasswordResetRequest_WithCorrectUsername_ShouldReturnOk()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithValidPasswordResetRequest()
                                .Build();

            // Act 
            var result = await arrangement.SUT.RequestPasswordRest("user");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task PasswordResetRequest_WithInCorrectUsername_ShouldReturnBadRequest()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithInValidPasswordResetRequest()
                                .Build();

            // Act 
            var result = await arrangement.SUT.RequestPasswordRest("user");

            // Assert
            var resultMessage = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            resultMessage.Value.Should().Be("Sorry this user doesn't exist");
        }

        [Fact]
        public async Task PasswordReset_WithCorrectUsername_ShouldReturnOk()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .WithValidPasswordReset()
                                .Build();

            var data = new PasswordResetViewModel() { Token = Guid.NewGuid(), Password = "password" };

            // Act 
            var result = await arrangement.SUT.PasswordRest(data);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task PasswordReset_WithInCorrectUsername_ShouldReturnBadRequest()
        {
            // Arrange
            var arrangement = new ArrangementBuilder()
                                .Build();

            var data = new PasswordResetViewModel() { Token = Guid.NewGuid(), Password = "password" };

            // Act 
            var result = await arrangement.SUT.PasswordRest(data);

            // Assert
            var resultMessage = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            resultMessage.Value.Should().Be("Sorry the password wasn't reset");
        }
    }
}
