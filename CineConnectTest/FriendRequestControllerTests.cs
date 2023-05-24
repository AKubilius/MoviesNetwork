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

namespace Bakis.Tests
{
    public class FriendRequestControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;

        public FriendRequestControllerTests()
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
        public async Task GetUsersRequests_ReturnsCorrectFriendRequests()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new FriendRequestController(dbContext, authorizationServiceMock.Object);

            var user1 = new User { Id = "1" };
            var user2 = new User { Id = "2" };
            dbContext.Users.AddRange(user1, user2);
            await dbContext.SaveChangesAsync();

            var friendRequest1 = new FriendRequest { Id = 1, InvitedBy = user1.Id, FriendId = user2.Id };
            var friendRequest2 = new FriendRequest { Id = 2, InvitedBy = user1.Id, FriendId = user1.Id }; // This request is not for user2
            dbContext.FriendRequests.AddRange(friendRequest1, friendRequest2);
            await dbContext.SaveChangesAsync();

         
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user2.Id),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.GetUsersRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var users = Assert.IsType<List<User>>(okResult.Value);
            Assert.Single(users);
            Assert.Equal(user1.Id, users[0].Id);
        }
        [Fact]
        public async Task InviteFriend_AddsFriendRequest_ReturnsOk()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new FriendRequestController(dbContext, authorizationServiceMock.Object);

            var user = new User { Id = "1", UserName = "Test User" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var claim = new Claim(JwtRegisteredClaimNames.Sub, user.Id);
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            var friendRequest = new FriendRequest { FriendId = "2" };

            // Act
            var result = await controller.InviteFriend(friendRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Invite was sent", okResult.Value);

         
            var friendRequestInDb = await dbContext.FriendRequests
                .SingleOrDefaultAsync(e => e.FriendId == friendRequest.FriendId && e.InvitedBy == user.Id);
            Assert.NotNull(friendRequestInDb);
        }
        [Fact]
        public async Task CancelRequest_RemovesRequest_ReturnsOk()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new FriendRequestController(dbContext, authorizationServiceMock.Object);


            var user = new User { Id = "1", UserName = "Test User" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

           
            var friendRequest = new FriendRequest { Id = 1, FriendId = "2", InvitedBy = user.Id };
            dbContext.FriendRequests.Add(friendRequest);
            await dbContext.SaveChangesAsync();

            var claim = new Claim(JwtRegisteredClaimNames.Sub, user.Id);
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            // Act
            var result = await controller.CancelRequest(friendRequest.FriendId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Request canceled", okResult.Value);

          
            var friendRequestInDb = await dbContext.FriendRequests
                .SingleOrDefaultAsync(e => e.FriendId == friendRequest.FriendId && e.InvitedBy == user.Id);
            Assert.Null(friendRequestInDb);
        }

        [Fact]
        public async Task AcceptRequest_WhenRequestExists_AcceptsRequestAndReturnsOk()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new FriendRequestController(dbContext, authorizationServiceMock.Object);

            var user1 = new User { Id = "1", UserName = "Test User 1" };
            dbContext.Users.Add(user1);


            var user2 = new User { Id = "2", UserName = "Test User 2" };
            dbContext.Users.Add(user2);


            var friendRequest = new FriendRequest { Id = 1, FriendId = user2.Id, InvitedBy = user1.Id };
            dbContext.FriendRequests.Add(friendRequest);

            await dbContext.SaveChangesAsync();


            var claim = new Claim(JwtRegisteredClaimNames.Sub, user1.Id);
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            // Act
            var result = await controller.AcceptRequest(friendRequest.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(friendRequest, okResult.Value);

            var friendRequestInDb = await dbContext.FriendRequests.FindAsync(friendRequest.Id);
            Assert.Null(friendRequestInDb);
 
            var friendInDb = await dbContext.Friends.SingleOrDefaultAsync(f => f.UserId == user1.Id && f.FriendId == user2.Id);
            Assert.NotNull(friendInDb);

            var roomInDb = await dbContext.Rooms.SingleOrDefaultAsync(r => r.Name == $"Room for {user2.Id} and {user1.Id}");
            Assert.NotNull(roomInDb);

            var userRoomsInDb = await dbContext.UserRooms.Where(ur => ur.RoomId == roomInDb.Id).ToListAsync();
            Assert.Equal(2, userRoomsInDb.Count);
            Assert.Contains(userRoomsInDb, ur => ur.UserId == user1.Id);
            Assert.Contains(userRoomsInDb, ur => ur.UserId == user2.Id);
        }
        [Fact]
        public async Task GetUserRequests_NoRequests_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new FriendRequestController(dbContext, authorizationServiceMock.Object);

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.GetUserRequests();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("There are no friend requests", notFoundResult.Value);
        }

        [Fact]
        public async Task GetUserRequests_HasRequests_ReturnsOk()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new FriendRequestController(dbContext, authorizationServiceMock.Object);

            var user1 = new User { Id = "1" };
            var user2 = new User { Id = "2" };
            dbContext.Users.AddRange(user1, user2);

            var friendRequest = new FriendRequest { Id = 1, FriendId = "1", InvitedBy = "2" };
            dbContext.FriendRequests.Add(friendRequest);
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
            var result = await controller.GetUserRequests();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var requests = Assert.IsType<List<FriendRequest>>(okResult.Value);
            Assert.Single(requests);
            Assert.Equal(friendRequest.Id, requests[0].Id);
        }
    }
}
