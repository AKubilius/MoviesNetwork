using Bakis.Controllers;
using Bakis.Data;
using Bakis.Data.Models.DTOs;
using Bakis.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CineConnectTest
{
    public class RecommendationControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly Mock<IAuthorizationService> _mockAuthorizationService;

        public RecommendationControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockAuthorizationService = new Mock<IAuthorizationService>();
        }

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .UseInternalServiceProvider(serviceProvider);

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        [Fact]
        public async Task GetRecommendations_ReturnsOkResult_WhenCalledWithValidUserIds()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHttpClient = new Mock<HttpClient>();
            mockHttpClientFactory.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(mockHttpClient.Object);

            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var controller = new RecommendationController(mockHttpClientFactory.Object, dbContext, authorizationServiceMock.Object, mockConfiguration.Object);
 

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
    new Claim(ClaimTypes.NameIdentifier, "1"),
                // ... additional claims as needed ...
            }));
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };


            var userIds = new List<string> { "user1", "user2" };
            int page = 1;

            // Act
            var result = await controller.GetRecommendations(userIds, page);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

    }
}
