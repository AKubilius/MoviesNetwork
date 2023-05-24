using Xunit;
using Bakis.Controllers;
using Bakis.Data;
using Bakis.Data.Models;
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
using Bakis.Auth.Model;

namespace Bakis.Tests
{
    public class FriendControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;

        public FriendControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "FriendRequestsDatabase")
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
        public async Task DeleteFriend_RemovesFriend_ReturnsDeletedFriend()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            authorizationServiceMock
                .Setup(service => service.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<Friend>(),
                    PolicyNames.ResourceOwner))
                .ReturnsAsync(AuthorizationResult.Success);

            var controller = new FriendController(dbContext, authorizationServiceMock.Object);
            var friend = new Friend { Id = 1, UserId = "dasdas-dasda-2sad5-d5s5",FriendId = "dsad5-dsad5a-5das15-asd5" };
            dbContext.Friends.Add(friend);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var deletedFriend = Assert.IsType<Friend>(okResult.Value);
            Assert.Equal(friend.Id, deletedFriend.Id);
            Assert.Equal(friend.UserId, deletedFriend.UserId);
            Assert.Equal(friend.FriendId, deletedFriend.FriendId);

            var friendInDb = await dbContext.Friends.FindAsync(1);
            Assert.Null(friendInDb);  // Verify that the friend was removed from the database
        }
        [Fact]
        public async Task GetUserFriends_ReturnsUserFriends()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new FriendController(dbContext, authorizationServiceMock.Object);

            var user1 = new User { Id = "1" };
            var user2 = new User { Id = "2" };
            var user3 = new User { Id = "3" };
            dbContext.Users.AddRange(user1, user2, user3);

            var friend1 = new Friend { UserId = "1", FriendId = "2" };
            var friend2 = new Friend { UserId = "1", FriendId = "3" };
            dbContext.Friends.AddRange(friend1, friend2);
            await dbContext.SaveChangesAsync();

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.GetUserFriends();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var friends = Assert.IsType<List<Friend>>(okResult.Value);
            Assert.Equal(2, friends.Count);
            Assert.Contains(friends, f => f.FriendId == friend1.FriendId);
            Assert.Contains(friends, f => f.FriendId == friend2.FriendId);
        }

        [Fact]
        public async Task GetUserFriends_NoFriends_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new FriendController(dbContext, authorizationServiceMock.Object);

            var user1 = new User { Id = "1" };
            dbContext.Users.Add(user1);

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.GetUserFriends();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("User has no friends", notFoundResult.Value);
        }
        [Fact]
        public async Task CreateFriendship_CreatesNewFriendship()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new FriendController(dbContext, authorizationServiceMock.Object);

            var user1 = new User { Id = "1" };
            var user2 = new User { Id = "2" };
            dbContext.Users.AddRange(user1, user2);
            await dbContext.SaveChangesAsync();

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var friend = new Friend { FriendId = "2" }; // UserId will be set in the controller

            // Act
            var result = await controller.CreateFriendship(friend);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var friends = Assert.IsType<List<Friend>>(okResult.Value);
            Assert.Single(friends);
            Assert.Equal(userId, friends[0].UserId);
            Assert.Equal(friend.FriendId, friends[0].FriendId);
        }

    }
}
