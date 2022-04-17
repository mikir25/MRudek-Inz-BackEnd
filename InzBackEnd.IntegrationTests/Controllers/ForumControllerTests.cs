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
    public class ForumControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private WebApplicationFactory<Startup> _factory;

        public ForumControllerTests(WebApplicationFactory<Startup> factory)
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

        private void SeedForum(Forum forum)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PortalDbContext>();

            _dbContext.Forums.Add(forum);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Delete_ForNonPostOwner_ReturnsForbidden()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 900,
                CategorieId = 1
            };

            SeedForum(forum);

            // act
            var response = await _client.DeleteAsync("/api/Forum/" + forum.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ForPostOwner_ReturnsNoContent()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 1,
                Contents = "Test",
                CategorieId = 1
            };

            SeedForum(forum);

            // act
            var response = await _client.DeleteAsync("/api/Forum/" + forum.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ForNonExistingPost_ReturnsNotFound()
        {
            // act

            var response = await _client.DeleteAsync("/api/Forum/987");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreatePost_WithValidModel_ReturnsCreatedStatus()
        {
            // arrange
            var model = new Forum()
            {
                Contents = "TestPost",
                UserId = 1,
                CategorieId = 1
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Forum", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreatePost_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange
            var model = new Forum();

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Forum", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateComment_WithValidModel_ReturnsOkResult()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 1,
                Contents = "Test",
                CategorieId = 1
            };

            SeedForum(forum);

            var model = new CommentDto()
            {
                Contents = "Test_Comment",
                Id = forum.Id
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Forum/Comment", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateComment_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 1,
                Contents = "Test",
                CategorieId = 1
            };

            SeedForum(forum);

            var model = new CommentDto();

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Forum/Comment", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAll_WithValidModel_ReturnsOkResult()
        {
            // act
            var response = await _client.GetAsync("/api/Forum");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_WithCorrectId_ReturnsOkResult()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 1,
                Contents = "Test",
                CategorieId = 1
            };

            SeedForum(forum);

            // act
            var response = await _client.GetAsync("/api/Forum/" + forum.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_WithBadId_ReturnsNotFound()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 1,
                Contents = "Test",
                CategorieId = 1
            };

            SeedForum(forum);

            // act
            var response = await _client.GetAsync("/api/Forum/0");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithValidModel_ReturnsOkResult()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 1,
                Contents = "Test",
                CategorieId = 1
            };

            SeedForum(forum);

            var updatePost = new Forum()
            {
                UserId = 1,
                Contents = "Test_Update",
                CategorieId = 1
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Forum/" + forum.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Update_WithBadId_ReturnsNotFound()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 1,
                Contents = "Test",
                CategorieId = 1
            };

            SeedForum(forum);

            var updatePost = new Forum()
            {
                UserId = 1,
                Contents = "Test_Update",
                CategorieId = 1
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Forum/0", httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithInvalidAuthorization_ReturnsForbidden()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 2,
                Contents = "Test",
                CategorieId = 1
            };

            SeedForum(forum);

            var updatePost = new Forum()
            {
                UserId = 1,
                Contents = "Test_Update",
                CategorieId = 1
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Forum/" + forum.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange

            var forum = new Forum()
            {
                UserId = 1,
                Contents = "Test",
                CategorieId = 1
            };

            SeedForum(forum);

            var updatePost = new Forum();

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Forum/" + forum.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}