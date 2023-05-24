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

namespace CineConnectTest
{
    public class MessageControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly Mock<IAuthorizationService> _mockAuthorizationService;

        public MessageControllerTests()
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
        public async Task Get_ReturnsListOfMessages()
        {

            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new MessageController(dbContext, authorizationServiceMock.Object);
            var user1 = new User { Id = "1", UserName = "User 1" };
            dbContext.Messages.Add(new Message { Id = 1, Body = "Test message", DateTime = DateTime.Now, RoomId = 1, SenderId="2",Sender=user1 });
            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var messages = Assert.IsType<List<MessageDto>>(okResult.Value);
            Assert.Single(messages);
        }

        [Fact]
        public async Task GetRoomId_ReturnsCommonRoomId()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new MessageController(dbContext, authorizationServiceMock.Object);

            var user1 = new User { Id = "1", UserName = "User 1" };
            var user2 = new User { Id = "2", UserName = "User 2" };
            dbContext.Users.AddRange(user1, user2);
            var room = new Room { Id = 1,Name="kamabarys" };
            dbContext.Rooms.Add(room);
            dbContext.UserRooms.AddRange(
                new UserRoom { UserId = user1.Id, RoomId = room.Id },
                new UserRoom { UserId = user2.Id, RoomId = room.Id });
            await dbContext.SaveChangesAsync();

            // Set up user identity for controller
            var claim = new Claim(JwtRegisteredClaimNames.Sub, user1.Id);
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            // Act
            var result = await controller.GetRoomId(user2.Id);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(room.Id, okResult.Value);

        }
    }

}
