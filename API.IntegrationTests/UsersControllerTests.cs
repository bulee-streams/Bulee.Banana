using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using API.IntegrationTests.Models;
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
    }
}
