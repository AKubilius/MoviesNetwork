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
using Bakis.Data.Migrations;

namespace Bakis.Tests
{
    public class WatchingRequestControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;
        private readonly Mock<IAuthorizationService> _mockAuthorizationService;
        public WatchingRequestControllerTests()
        {
            _mockAuthorizationService = new Mock<IAuthorizationService>();
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WatchingRequestsDatabase")
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
        public async Task GetUserRequest_ReturnsWatchingRequestForMessageId()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new WatchingRequestController(dbContext, authorizationServiceMock.Object);

            var messageId = 1;
            var friendId = "1";
            var expectedRequest = new WatchingRequest { MessageId = messageId, FriendId = friendId };
            dbContext.Add(expectedRequest);
            await dbContext.SaveChangesAsync();
            // Act
            var result = await controller.GetUserRequest(messageId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequest = Assert.IsType<WatchingRequest>(okResult.Value);
            Assert.Equal(expectedRequest, returnedRequest);
        }
        [Fact]
        public async Task Get_ReturnsWatchingRequestsForCurrentUser()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new WatchingRequestController(dbContext, authorizationServiceMock.Object);

            var currentUserId = "1";
            var UserId = "2";
            var messageId = 1;

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var expectedRequests = new List<WatchingRequest>
    {
        new WatchingRequest { FriendId = currentUserId,InvitedById=UserId, MessageId =messageId },
        new WatchingRequest { InvitedById = UserId,FriendId = currentUserId, MessageId =messageId }
    };
            foreach (var request in expectedRequests)
            {
                dbContext.WatchingRequests.Add(request);
            }
            await dbContext.SaveChangesAsync();
            // Act

            var result = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequests = Assert.IsType<List<WatchingRequest>>(okResult.Value);
            Assert.Equal(expectedRequests.Count, returnedRequests.Count); // compare the counts first
            Assert.Equal(expectedRequests, returnedRequests); // then com
        }

        [Fact]
        public async Task GetCurrentUserId_ReturnsCorrectId()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new WatchingRequestController(dbContext, authorizationServiceMock.Object);

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = controller.getCurrentUserId();  // Assuming this method is public

            // Assert
            Assert.Equal(userId, result);
        }

        [Fact]
        public async Task GetUserRequest_WithUsername_ReturnsWatchingRequestsForUser()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new WatchingRequestController(dbContext, authorizationServiceMock.Object);

            var userName = "TestUser";
            var userId = "1";
            var UserId = "2";
            var messageId = 1;


            var expectedUser = new User { UserName = userName, Id = userId };

           
            var expectedRequests = new List<WatchingRequest>
    {
       new WatchingRequest { FriendId = userId,InvitedById = UserId,MessageId =messageId },
       new WatchingRequest { InvitedById = userId,FriendId = UserId,MessageId =messageId }
    };

            dbContext.Users.Add(expectedUser);
            foreach (var request in expectedRequests)
            {
                dbContext.WatchingRequests.Add(request);
            }
            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.GetUserRequest(expectedUser.UserName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequests = Assert.IsType<List<WatchingRequest>>(okResult.Value);
            Assert.Equal(expectedRequests.Count, returnedRequests.Count); // compare the counts first
            Assert.Equal(expectedRequests, returnedRequests); // then com
        }

        [Fact]
        public async Task AcceptRequest_WithId_SetsStatusToAcceptedAndReturnsRequest()
        {
            // Arrange
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new WatchingRequestController(dbContext, authorizationServiceMock.Object);

            var requestId = 1;
            var friendId = "1";
            var expectedRequest = new WatchingRequest { MessageId = requestId, FriendId = friendId };

            dbContext.Add(expectedRequest);
            await dbContext.SaveChangesAsync();
           

            // Act
            var result = await controller.AcceptRequest(requestId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequest = Assert.IsType<WatchingRequest>(okResult.Value);
            Assert.Equal(expectedRequest, returnedRequest);
            Assert.Equal(Status.Accepted, returnedRequest.Status);
        }
        [Fact]
        public async Task DeclineRequest_WithId_SetsStatusToDeclinedAndReturnsRequest()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new WatchingRequestController(dbContext, authorizationServiceMock.Object);

            var requestId = 1;
            var friendId = "1";
            var expectedRequest = new WatchingRequest { MessageId = requestId, FriendId = friendId };

            dbContext.Add(expectedRequest);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.DeclineRequest(requestId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequest = Assert.IsType<WatchingRequest>(okResult.Value);
            Assert.Equal(expectedRequest, returnedRequest);
            Assert.Equal(Status.Declined, returnedRequest.Status);
        }

        [Fact]
        public async Task CancelRequest_WithId_RemovesRequestAndReturnsMessage()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new WatchingRequestController(dbContext, authorizationServiceMock.Object);

            var requestId = "1";
            var userId = "2";
            var expectedRequest = new WatchingRequest { FriendId = requestId, InvitedById = userId };

            dbContext.Add(expectedRequest);
            await dbContext.SaveChangesAsync();

            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

            // Act
            var result = await controller.CancelRequest(requestId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Request canceled", okResult.Value);
          
        }

    }
}
