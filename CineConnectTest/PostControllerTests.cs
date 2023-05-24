using Xunit;
using Bakis.Controllers;
using Bakis.Data;
using Bakis.Data.Models;
using Bakis.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Bakis.Data.Models.DTOs;
using Bakis.Data.Migrations;
using Post = Bakis.Data.Models.Post;

namespace Bakis.Tests
{
    public class PostControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private readonly Mock<IAuthorizationService> _mockAuthorizationService;

        public PostControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for each test
                .Options;

            _mockAuthorizationService = new Mock<IAuthorizationService>();
        }

        private ApplicationDbContext GetInMemoryDbContext(string dbName)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName) // Pass unique name here
                .UseInternalServiceProvider(serviceProvider);

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        private IAuthorizationService CreateStubAuthorizationService()
        {
            var stubAuthorizationService = new Mock<IAuthorizationService>();
            stubAuthorizationService.Setup(service => service.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success);

            return stubAuthorizationService.Object;
        }

        [Fact]
        public async Task GetPostsCount_ReturnsNotFound_WhenPostNotFound()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext("Test1");
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new PostController(dbContext, authorizationServiceMock.Object);

            // Create a user who will cancel a friend request
            var post = new Post { Id = 1, Body = "Geras postas", CreatedDate = new DateTime(), ImageUrl ="/lloaoappapas",MovieId=1480 };
            var post2 = new Post { Id = 2, Body = "Ger post", CreatedDate = new DateTime(), ImageUrl ="/lloa",MovieId=140 };
            dbContext.Posts.Add(post);
            dbContext.Posts.Add(post2);
            await dbContext.SaveChangesAsync();

            var claim = new Claim(JwtRegisteredClaimNames.Sub, "1");
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };
            // Act
            var result = await controller.GetPostsCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(2, okResult.Value);
        }

        [Fact]
        public async Task Get_ReturnsPaginatedPosts()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext("Test2");
            var controller = new PostController(dbContext, _mockAuthorizationService.Object);
            dbContext.Posts.AddRange(
                new Post { Id = 1, Body = "Geras postas", CreatedDate = new DateTime(), ImageUrl = "/lloaoappapas", MovieId = 1480, User = new User { Id = "1", UserName = "User 1" } },
                new Post { Id = 2, Body = "Geras postas2", CreatedDate = new DateTime(), ImageUrl = "/ppapas", MovieId = 1485, User = new User { Id = "2", UserName = "User 2" } },
                new Post { Id = 3, Body = "Geras postas3", CreatedDate = new DateTime(), ImageUrl = "/lloaoa", MovieId = 1490, User = new User { Id = "3", UserName = "User 3" } });
            await dbContext.SaveChangesAsync();


            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            // Act
            var result = await controller.Get(1, 2); // Page 1, pageSize 2

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var posts = Assert.IsType<List<Post>>(okResult.Value);
            Assert.Equal(2, posts.Count); // Check if 2 posts are returned as per pageSize
        }
        // Add more test methods for other test cases
    }
}
