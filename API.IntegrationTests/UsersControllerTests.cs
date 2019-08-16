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
        public async Task Endpoint_RegisterUser_ShouldReturnCreated()
        {
            // Arrange 
            var user = new RegisterUser() { Email = "user@email.com", UserName = "username", Password = "passworD01" };

            // Act
            var httpResponse = await client.PostAsJsonAsync("/api/v1/users/register", user);

            // Assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
