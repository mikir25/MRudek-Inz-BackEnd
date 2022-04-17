using FluentAssertions;
using InzBackEnd.Entities;
using InzBackEnd.IntegrationTests.Helpers;
using InzBackEnd.Models;
using InzBackEnd.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace InzBackEnd.IntegrationTests.Controllers
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();

        public AccountControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory
                  .WithWebHostBuilder(builder =>
                  {
                      builder.ConfigureServices(services =>
                      {
                          var dbContextOptions = services
                              .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<PortalDbContext>));

                          services.Remove(dbContextOptions);

                          services.AddSingleton<IAccountService>(_accountServiceMock.Object);

                          services
                           .AddDbContext<PortalDbContext>(options => options.UseInMemoryDatabase("AppDbConnection"));
                      });
                  })
                .CreateClient();
        }

        [Fact]
        public async Task Login_ForRegisteredUser_ReturnsOk()
        {
            // arrange

            _accountServiceMock
                .Setup(e => e.GenerateJwt(It.IsAny<LoginDto>()))
                .Returns(new Token() { Value = "jwt" });

            var loginDto = new LoginDto()
            {
                Email = "user@test.com",
                Password = "test"
            };

            var httpContent = loginDto.ToJsonHttpContent();

            // act

            var response = await _client.PostAsync("/api/Account/login", httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterUser_ForValidModel_ReturnsOk()
        {
            // arrange

            var registerUser = new RegisterUserDto()
            {
                Name = "test",
                Email = "test@test.com",
                Password = "password123",
                ConfirmPassword = "password123"
            };

            var httpContent = registerUser.ToJsonHttpContent();

            // act

            var response = await _client.PostAsync("/api/Account/register", httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterUser_ForInvalidModel_ReturnsBadRequest()
        {
            // arrange

            var registerUser = new RegisterUserDto()
            {
                Password = "password123",
                ConfirmPassword = "123"
            };

            var httpContent = registerUser.ToJsonHttpContent();

            // act

            var response = await _client.PostAsync("/api/Account/register", httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

    }
}