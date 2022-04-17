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
    public class CommentControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private WebApplicationFactory<Startup> _factory;

        public CommentControllerTests(WebApplicationFactory<Startup> factory)
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

        private void SeedComment(Comment comment)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PortalDbContext>();

            _dbContext.Comments.Add(comment);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Delete_ForUnauthorizedOwner_ReturnsForbidden()
        {
            // arrange

            var comment = new Comment()
            {
                UserName = "test",
                UserId = 900
            };

            SeedComment(comment);

            // act
            var response = await _client.DeleteAsync("/api/Comment/" + comment.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ForcommentOwner_ReturnsNoContent()
        {
            // arrange

            var comment = new Comment()
            {
                UserName = "test",
                Contents = "test",
                UserId = 1
            };

            SeedComment(comment);

            // act
            var response = await _client.DeleteAsync("/api/Comment/" + comment.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ForNonExistingcomment_ReturnsNotFound()
        {
            // act

            var response = await _client.DeleteAsync("/api/Comment/987");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithValidModel_ReturnsOkResult()
        {
            // arrange

            var comment = new Comment()
            {
                UserName = "test",
                Contents = "test",
                UserId = 1
            };

            SeedComment(comment);

            var updatePost = new Comment()
            {
                UserName = "test",
                Contents = "test_Update",
                UserId = 1
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Comment/" + comment.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Update_WithBadId_ReturnsNotFound()
        {
            // arrange

            var comment = new Comment()
            {
                UserName = "test",
                Contents = "test",
                UserId = 1
            };

            SeedComment(comment);

            var updatePost = new Comment()
            {
                UserName = "test",
                Contents = "test_Update",
                UserId = 1
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Comment/0", httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithInvalidAuthorization_ReturnsForbidden()
        {
            // arrange

            var comment = new Comment()
            {
                UserName = "test",
                Contents = "test_Update",
                UserId = 2
            };

            SeedComment(comment);

            var updatePost = new Comment()
            {
                UserName = "test",
                Contents = "test_Update",
                UserId = 1
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Comment/" + comment.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange

            var comment = new Comment()
            {
                UserName = "test",
                Contents = "test",
                UserId = 1
            };

            SeedComment(comment);

            var updatePost = new Comment();

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Comment/" + comment.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }

}