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

namespace Bakis.Tests
{
    public class LikeControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;

        public LikeControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "LikesDatabase")
                .Options;
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

        private IAuthorizationService CreateStubAuthorizationService()
        {
            var stubAuthorizationService = new Mock<IAuthorizationService>();
            stubAuthorizationService.Setup(service => service.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .ReturnsAsync(AuthorizationResult.Success);

            return stubAuthorizationService.Object;
        }

        [Fact]
        public async Task GetLikesCount_ReturnsNotFound_WhenPostNotFound()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new LikeController(dbContext, authorizationServiceMock.Object);

            // Act
            var result = await controller.GetLikesCount(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetIsLiked_ReturnsCorrectBoolean()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new LikeController(dbContext, authorizationServiceMock.Object);

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var post = new Post { Id = 1, Body="Patiko", ImageUrl="/loap.jpg" };
            dbContext.Posts.Add(post);

            var user = new User { Id = userId };
            dbContext.Users.Add(user);

            var like = new Like { UserId = userId, PostId = 1 };
            dbContext.Likes.Add(like);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.GetIsLiked(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)okResult.Value);

            // Clean up
            dbContext.Likes.Remove(like);
            await dbContext.SaveChangesAsync();

            // Act
            result = await controller.GetIsLiked(1);

            // Assert
            okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.False((bool)okResult.Value);
        }
        [Fact]
        public async Task LikePost_AddsLikeToPost()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new LikeController(dbContext, authorizationServiceMock.Object);

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var post = new Post { Id = 1, Body = "Patiko", ImageUrl = "/loap.jpg" };
            dbContext.Posts.Add(post);
            await dbContext.SaveChangesAsync();

            var like = new Like { PostId = 1 };

            // Act
            var result = await controller.LikePost(like);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var likes = Assert.IsType<List<Like>>(okResult.Value);
            Assert.Contains(likes, l => l.UserId == userId && l.PostId == post.Id);

            // Clean up
            dbContext.Likes.Remove(like);
            await dbContext.SaveChangesAsync();
        }
        [Fact]
        public async Task UnlikePost_RemovesLikeFromPost()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new LikeController(dbContext, authorizationServiceMock.Object);

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var post = new Post { Id = 1, Body = "Patiko", ImageUrl = "/loap.jpg" };
            dbContext.Posts.Add(post);

            var like = new Like { PostId = 1, UserId = userId };
            dbContext.Likes.Add(like);

            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.UnlikePost(post.Id);

            // Assert
            Assert.IsType<OkResult>(result.Result);
            Assert.DoesNotContain(dbContext.Likes, l => l.UserId == userId && l.PostId == post.Id);

            // Clean up
            dbContext.Posts.Remove(post);
            await dbContext.SaveChangesAsync();
        }
    }
}