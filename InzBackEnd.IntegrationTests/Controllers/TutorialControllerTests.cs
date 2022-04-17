using FluentAssertions;
using InzBackEnd.Entities;
using InzBackEnd.IntegrationTests.Helpers;
using InzBackEnd.Models;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace InzBackEnd.IntegrationTests.Controllers
{
    public class TutorialControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private WebApplicationFactory<Startup> _factory;

        public TutorialControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services
                            .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<PortalDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                        services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));

                        services
                         .AddDbContext<PortalDbContext>(options => options.UseInMemoryDatabase("AppDbConnection"));
                    });
                });

            _client = _factory.CreateClient();
        }

        private void SeedTutorial(Tutorial tutorial)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PortalDbContext>();

            _dbContext.Tutorials.Add(tutorial);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Delete_ForNonPostOwner_ReturnsForbidden()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 900
            };

            SeedTutorial(tutorial);

            // act
            var response = await _client.DeleteAsync("/api/Tutorial/" + tutorial.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ForPostOwner_ReturnsNoContent()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 1,
                Contents = "Test"
            };

            SeedTutorial(tutorial);

            // act
            var response = await _client.DeleteAsync("/api/Tutorial/" + tutorial.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ForNonExistingPost_ReturnsNotFound()
        {
            // act

            var response = await _client.DeleteAsync("/api/Tutorial/987");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreatePost_WithValidModel_ReturnsCreatedStatus()
        {
            // arrange
            var model = new Tutorial()
            {
                Contents = "TestPost",
                UserId = 1
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Tutorial", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreatePost_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange
            var model = new Tutorial();

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Tutorial", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateComment_WithValidModel_ReturnsOkResult()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 1,
                Contents = "Test"
            };

            SeedTutorial(tutorial);

            var model = new CommentDto()
            {
                Contents = "Test_Comment",
                Id = tutorial.Id
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Tutorial/Comment", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateComment_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 1,
                Contents = "Test"
            };

            SeedTutorial(tutorial);

            var model = new CommentDto();

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Tutorial/Comment", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAll_WithValidModel_ReturnsOkResult()
        {
            // act
            var response = await _client.GetAsync("/api/Tutorial");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_WithCorrectId_ReturnsOkResult()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 1,
                Contents = "Test"
            };

            SeedTutorial(tutorial);

            // act
            var response = await _client.GetAsync("/api/Tutorial/" + tutorial.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_WithBadId_ReturnsNotFound()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 1,
                Contents = "Test"
            };

            SeedTutorial(tutorial);

            // act
            var response = await _client.GetAsync("/api/Tutorial/0");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithValidModel_ReturnsOkResult()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 1,
                Contents = "Test"
            };

            SeedTutorial(tutorial);

            var updatePost = new Tutorial()
            {
                UserId = 1,
                Contents = "Test_Update"
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Tutorial/" + tutorial.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Update_WithBadId_ReturnsNotFound()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 1,
                Contents = "Test"
            };

            SeedTutorial(tutorial);

            var updatePost = new Tutorial()
            {
                UserId = 1,
                Contents = "Test_Update"
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Tutorial/0", httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithInvalidAuthorization_ReturnsForbidden()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 2,
                Contents = "Test"
            };

            SeedTutorial(tutorial);

            var updatePost = new Tutorial()
            {
                UserId = 1,
                Contents = "Test_Update"
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Tutorial/" + tutorial.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange

            var tutorial = new Tutorial()
            {
                UserId = 1,
                Contents = "Test"
            };

            SeedTutorial(tutorial);

            var updatePost = new Tutorial();

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Tutorial/" + tutorial.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
