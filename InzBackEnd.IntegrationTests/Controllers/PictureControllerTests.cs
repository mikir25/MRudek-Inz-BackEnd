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
    public class PictureControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private WebApplicationFactory<Startup> _factory;

        public PictureControllerTests(WebApplicationFactory<Startup> factory)
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

        private void SeedPicture(Picture picture)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PortalDbContext>();

            _dbContext.Pictures.Add(picture);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Delete_ForPostOwner_ReturnsNoContent()
        {
            // arrange

            var picture = new Picture()
            {
                Name = "test",
                picture = "test"
            };

            SeedPicture(picture);

            // act
            var response = await _client.DeleteAsync("/api/Picture/" + picture.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ForNonExistingPost_ReturnsNotFound()
        {
            // act

            var response = await _client.DeleteAsync("/api/Picture/987");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreatePost_WithValidModel_ReturnsCreatedStatus()
        {
            // arrange
            var model = new Picture()
            {
                Name = "test",
                picture = "test"
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Picture", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreatePost_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange
            var model = new Picture();

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Picture", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

    }
}
