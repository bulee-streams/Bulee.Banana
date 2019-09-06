using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using API.IntegrationTests.Models;
using API.Models.ViewModels;
using FluentAssertions;
using Xunit;

namespace API.IntegrationTests
{
    public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient client;

        public UsersControllerTests(CustomWebApplicationFactory<Startup> factory) {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task Endpoint_RegisterUserCorecct_ShouldReturnCreated()
        {
            // Arrange 
            var user = new RegisterUser() { UserName = "username", Email = "username@email.com", Password = "passworD01" };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/v1/users/register", user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Endpoint_RegisterUserUsedUsername_ShouldReturnBadRequest()
        {
            // Arrange 
            var user = new RegisterUser() { UserName = "user", Email = "username1@email.com", Password = "passworD01" };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/v1/users/register", user);
            var responseMessage = await httpResponse.Content.ReadAsStringAsync();

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseMessage.Should().Be("Sorry this username has already been used");
        }

        [Fact]
        public async Task Endpoint_RegisterUserUsedEmail_ShouldReturnBadRequest()
        {
            // Arrange 
            var user = new RegisterUser() { UserName = "username2", Email = "user@email.com", Password = "passworD01" };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/v1/users/register", user);
            var responseMessage = await httpResponse.Content.ReadAsStringAsync();

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseMessage.Should().Be("Sorry this email address has already been used");
        }

        [Fact]
        public async Task Endpoint_ConfirmEmailCorrectToken_ShouldReturnOk()
        {
            // Act
            var httpResponse = await client.GetAsync("api/v1/users/registration-complete/A6A46A35-5165-4AB5-9E19-12764CFC2144");

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Endpoint_ConfirmEmailCorrectToken_ShouldReturnBadRequest()
        {
            // Act
            var httpResponse = await client.GetAsync("api/v1/users/registration-complete/" + Guid.NewGuid());
            var responseMessage = await httpResponse.Content.ReadAsStringAsync();

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseMessage.Should().Be("Sorry this request is invalid");
        }

        [Fact]
        public async Task Endpoint_LoginWithCorrectUsername_ShouldReturnOkWithToken()
        {
            // Arrange
            var user = new LoginViewModel() { Username = "user", Password = "password" };

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/v1/users/login", user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Endpoint_LoginWithCorrectEmail_ShouldReturnOkWithToken()
        {
            // Arrange
            var user = new LoginViewModel() { Username = "user@email.com", Password = "password" };

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/v1/users/login", user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Endpoint_LoginWithInCorrectUsername_ShouldReturnBadRequest()
        {
            // Arrange
            var user = new LoginViewModel() { Username = "user1", Password = "password" };

            // Act
            var httpResponse = await client.PostAsJsonAsync("api/v1/users/login", user);
            var responseMessage = await httpResponse.Content.ReadAsStringAsync();

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseMessage.Should().Be("Sorry your username or password is invalid");
        }

        [Fact]
        public async Task Endpoint_RequestPasswordResetWithExistingUsername_ShouldReturnOk()
        {

            // Act
            var httpResponse = await client.GetAsync("api/v1/users/request-password-reset/user");

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Endpoint_RequestPasswordResetWithExistingEmail_ShouldReturnOk()
        {
            // Act
            var httpResponse = await client.GetAsync("api/v1/users/request-password-reset/user@email.com");

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Endpoint_RequestPasswordResetWithNonExistingUsername_ShouldReturnBadRequest()
        {
            // Act
            var httpResponse = await client.GetAsync("api/v1/users/request-password-reset/user1");
            var responseMessage = await httpResponse.Content.ReadAsStringAsync();

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            responseMessage.Should().Be("Sorry this user doesn't exist");
        }

        [Fact]
        public async Task Endpoint_PasswordResetWithCorrectToken_ShouldReturnOk()
        {
            // Assert
            var newPassword = "NewPassword";

            var httpGetToken = await client.GetAsync("api/v1/users/request-password-reset/user");
            var responseToken = await httpGetToken.Content.ReadAsStringAsync();

            var passwordToken = Guid.Parse(responseToken);

            var password = new PasswordResetViewModel() { Token = passwordToken, Password = newPassword };
            var user = new LoginViewModel() { Username = "user", Password = newPassword };

            // Act
            var passwordHttpResponse = await client.PostAsJsonAsync("api/v1/users/password-reset", password);
            var loginhttpResponse = await client.PostAsJsonAsync("api/v1/users/login", user);


            // Assert
            passwordHttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            loginhttpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Endpoint_PasswordResetWithInCorrectToken_ShouldReturnBadRequest()
        {
            // Assert
            var newPassword = "NewPassword";

            var password = new PasswordResetViewModel() { Token = Guid.NewGuid(), Password = newPassword };
            var user = new LoginViewModel() { Username = "user", Password = newPassword };

            // Act
            var passwordHttpResponse = await client.PostAsJsonAsync("api/v1/users/password-reset", password);
            var passwordResponseMessage = await passwordHttpResponse.Content.ReadAsStringAsync();

            var loginhttpResponse = await client.PostAsJsonAsync("api/v1/users/login", user);


            // Assert
            passwordHttpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            passwordResponseMessage.Should().Be("Sorry the password wasn't reset");

            loginhttpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
