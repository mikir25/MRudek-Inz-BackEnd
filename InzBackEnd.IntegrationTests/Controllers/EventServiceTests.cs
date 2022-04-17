using FluentAssertions;
using InzBackEnd.Entities;
using InzBackEnd.IntegrationTests.Helpers;
using InzBackEnd.Models;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace InzBackEnd.IntegrationTests.Controllers
{
    public class EventServiceTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private WebApplicationFactory<Startup> _factory;

        public EventServiceTests(WebApplicationFactory<Startup> factory)
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

        private void SeedEvent(Event _event)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<PortalDbContext>();

            _dbContext.Events.Add(_event);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Delete_ForNonPostOwner_ReturnsForbidden()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 900
            };

            SeedEvent(_event);

            // act
            var response = await _client.DeleteAsync("/api/Event/" + _event.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ForPostOwner_ReturnsNoContent()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 1,
                Contents = "Test",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            SeedEvent(_event);

            // act
            var response = await _client.DeleteAsync("/api/Event/" + _event.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ForNonExistingPost_ReturnsNotFound()
        {
            // act

            var response = await _client.DeleteAsync("/api/Event/987");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreatePost_WithValidModel_ReturnsCreatedStatus()
        {
            // arrange
            var model = new Event()
            {
                Contents = "TestPost",
                UserId = 1,
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Event", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreatePost_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange
            var model = new Event();

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Event", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateComment_WithValidModel_ReturnsOkResult()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 1,
                Contents = "Test",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            SeedEvent(_event);

            var model = new CommentDto()
            {
                Contents = "Test_Comment",
                Id = _event.Id,
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Event/Comment", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateComment_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 1,
                Contents = "Test",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            SeedEvent(_event);

            var model = new CommentDto();

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/Event/Comment", httpContent);

            // arrange

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAll_WithValidModel_ReturnsOkResult()
        {
            // act
            var response = await _client.GetAsync("/api/Event");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_WithCorrectId_ReturnsOkResult()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 1,
                Contents = "Test",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            SeedEvent(_event);

            // act
            var response = await _client.GetAsync("/api/Event/" + _event.Id);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_WithBadId_ReturnsNotFound()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 1,
                Contents = "Test",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            SeedEvent(_event);

            // act
            var response = await _client.GetAsync("/api/Event/0");

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithValidModel_ReturnsOkResult()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 1,
                Contents = "Test",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            SeedEvent(_event);

            var updatePost = new Event()
            {
                UserId = 1,
                Contents = "Test_Update",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Event/" + _event.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Update_WithBadId_ReturnsNotFound()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 1,
                Contents = "Test",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            SeedEvent(_event);

            var updatePost = new Event()
            {
                UserId = 1,
                Contents = "Test_Update",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Event/0", httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WithInvalidAuthorization_ReturnsForbidden()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 2,
                Contents = "Test",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            SeedEvent(_event);

            var updatePost = new Event()
            {
                UserId = 1,
                Contents = "Test_Update",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Event/" + _event.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Update_WithInvalidModel_ReturnsBadRequest()
        {
            // arrange

            var _event = new Event()
            {
                UserId = 1,
                Contents = "Test",
                DateOfEvent = new DateTime(),
                PlaceOfEvent = "Test"
            };

            SeedEvent(_event);

            var updatePost = new Event();

            var httpContent = updatePost.ToJsonHttpContent();

            // act
            var response = await _client.PutAsync("/api/Event/" + _event.Id, httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}