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

namespace Bakis.Tests
{
    public class ChallengeControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;

        public ChallengeControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "ChallengesDatabase")
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
        public async Task GetMyChallenges_ReturnsNotFound_WhenUserHasNoChallenges()
        {
            // Arrange
            using (var dbContext = GetInMemoryDbContext())
            {
                var authorizationService = CreateStubAuthorizationService();
                var controller = new ChallengeController(dbContext, authorizationService);

                // Set up user claims
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, "1"),
                };
                var identity = new ClaimsIdentity(claims);
                var claimsPrincipal = new ClaimsPrincipal(identity);

                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = claimsPrincipal }
                };

                // Act
                var result = await controller.Get();

                // Assert
                Assert.IsType<BadRequestObjectResult>(result.Result);
            }
        }

        [Fact]
        public async Task JoinChallenge_CreatesUserChallenge_WhenChallengeNotAlreadyActive()
        {
            // Arrange
            using (var dbContext = GetInMemoryDbContext())
            {
                var authorizationService = CreateStubAuthorizationService();
                var controller = new ChallengeController(dbContext, authorizationService);

                // Set up user claims
                var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, "1"),
        };
                var identity = new ClaimsIdentity(claims);
                var claimsPrincipal = new ClaimsPrincipal(identity);

                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = claimsPrincipal }
                };

                // Create a challenge
                var challenge = new Challenge
                {
                    Name = "Test Challenge",
                    Description = "Test challenge description",
                    Count = 10,
                };

                dbContext.Challenges.Add(challenge);
                await dbContext.SaveChangesAsync();

                var userChallenge = new UserChallenge
                {
                    ChallengeId = challenge.Id,
                };

                // Act
                var result = await controller.JoinChallenge(userChallenge);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var userChallenges = Assert.IsType<List<UserChallenge>>(okResult.Value);
                Assert.Single(userChallenges);
                Assert.Equal(challenge.Id, userChallenges[0].ChallengeId);
                Assert.Equal(claimsPrincipal.FindFirstValue(JwtRegisteredClaimNames.Sub), userChallenges[0].UserId);
            }
        }

        [Fact]
        public async Task EditChallenge_UpdatesChallenge_WhenValidInput()
        {
            // Arrange
            using (var dbContext = GetInMemoryDbContext())
            {
                var authorizationService = CreateStubAuthorizationService();
                var controller = new ChallengeController(dbContext, authorizationService);

                // Create a challenge
                var challenge = new Challenge
                {
                    Name = "Original Challenge",
                    Description = "Original challenge description",
                    Count = 5,
                };

                dbContext.Challenges.Add(challenge);
                await dbContext.SaveChangesAsync();

                // Prepare the edit request
                var challengeEditRequest = new ChallengeEditDto
                {
                    Name = "Updated Challenge",
                    Description = "Updated challenge description",
                    Count = 10,
                };

                // Act
                var result = await controller.EditChallenge(challenge.Id, challengeEditRequest);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var updatedChallenge = Assert.IsType<Challenge>(okResult.Value);
                Assert.Equal(challenge.Id, updatedChallenge.Id);
                Assert.Equal(challengeEditRequest.Name, updatedChallenge.Name);
                Assert.Equal(challengeEditRequest.Description, updatedChallenge.Description);
                Assert.Equal(challengeEditRequest.Count, updatedChallenge.Count);
            }
        }

        [Fact]
        public async Task GetUserList_ReturnsUserChallenges_WhenUserNameExists()
        {
            // Arrange
            using (var dbContext = GetInMemoryDbContext())
            {
                var authorizationService = CreateStubAuthorizationService();
                var controller = new ChallengeController(dbContext, authorizationService);

                // Create a user and user challenges
                var user = new User { UserName = "testUser" };
                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();

                var challenge = new Challenge { Name = "Challenge 1", Description ="Watch billion Comedies",Count=65 };
                dbContext.Challenges.Add(challenge);
                await dbContext.SaveChangesAsync();

                var userChallenge = new UserChallenge { UserId = user.Id, ChallengeId = challenge.Id };
                dbContext.UserChallenges.Add(userChallenge);
                await dbContext.SaveChangesAsync();

                // Act
                var result = await controller.GetUserList(user.UserName);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var userChallenges = Assert.IsType<List<UserChallenge>>(okResult.Value);
                Assert.Single(userChallenges);
                Assert.Equal(user.Id, userChallenges[0].UserId);
                Assert.Equal(challenge.Id, userChallenges[0].ChallengeId);
            }
        }

        [Fact]
        public async Task GetChallengesNotJoined_ReturnsChallengesNotJoinedByCurrentUser()
        {
            // Arrange
            using (var dbContext = GetInMemoryDbContext())
            {
                var authorizationService = CreateStubAuthorizationService();
                var controller = new ChallengeController(dbContext, authorizationService);

                // Create challenges and user challenges
                var challenge1 = new Challenge { Name = "Challenge 1", Description = "Challenge 1 Description", Count= 25 };
                var challenge2 = new Challenge { Name = "Challenge 2", Description = "Challenge 2 Description",Count= 10 };
                dbContext.Challenges.AddRange(challenge1, challenge2);
                await dbContext.SaveChangesAsync();

                // Assume the current user ID is 1
                var currentUserChallenge = new UserChallenge { UserId = "1", ChallengeId = challenge1.Id };
                dbContext.UserChallenges.Add(currentUserChallenge);
                await dbContext.SaveChangesAsync();

                // Set the current user ID in the controller
                controller.ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Sub, "1") }))
                    }
                };

                // Act
                var result = await controller.GetChallengesNotJoined();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var notJoinedChallenges = Assert.IsType<List<Challenge>>(okResult.Value);
                Assert.Single(notJoinedChallenges);
                Assert.Equal(challenge2.Id, notJoinedChallenges[0].Id);
            }
        }

        [Fact]
        public async Task CreateChallenge_CreatesNewChallengeWithConditions()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationService = CreateStubAuthorizationService();
            var controller = new ChallengeController(dbContext, authorizationService);


            var claims = new List<Claim> { new Claim(ClaimTypes.Role, Roles.Admin) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var challengeCreateDto = new ChallengeCreateDto
            {
                Name = "Test Challenge",
                Description = "This is a test challenge",
                Count = 5,
                Conditions = new List<ChallengeCondition>
        {
            new ChallengeCondition { Type = "Any",Value="Any" },
            new ChallengeCondition { Type = "Any",Value="Any" },
        }
            };

            // Act
            var result = await controller.CreateChallenge(challengeCreateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var challenge = Assert.IsType<Challenge>(okResult.Value);
            Assert.Equal(challengeCreateDto.Name, challenge.Name);
            Assert.Equal(challengeCreateDto.Description, challenge.Description);
            Assert.Equal(challengeCreateDto.Count, challenge.Count);
            Assert.Equal(challengeCreateDto.Conditions.Count, dbContext.ChallengeConditions.Count(c => c.ChallengeId == challenge.Id));

            // Clean up
            dbContext.Challenges.Remove(challenge);
            await dbContext.SaveChangesAsync();
        }
        // Add more test methods for other test cases
    }
}
