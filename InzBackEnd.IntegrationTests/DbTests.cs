using FluentAssertions;
using InzBackEnd.IntegrationTests.Helpers;
using InzBackEnd.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InzBackEnd.IntegrationTests
{
    public class DbTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        public DbTests()
        {
            var factory = new WebApplicationFactory<Startup>();
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ForRegisteredUser_ReturnsOk()
        {
            // arrange


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

        [Theory]
        [InlineData("USER@test.com", "test")]
        [InlineData("User@test.com", "test")]
        [InlineData("useR@test.com", "test")]
        [InlineData("user@test.com", "test1")]
        public async Task Login_ForBadUser_ReturnsBadRequest(string email, string password)
        {
            // arrange


            var loginDto = new LoginDto()
            {
                Email = email,
                Password = password
            };

            var httpContent = loginDto.ToJsonHttpContent();

            // act

            var response = await _client.PostAsync("/api/Account/login", httpContent);

            // assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    
    }


}
