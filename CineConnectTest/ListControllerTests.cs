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
using Moq.Protected;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Bakis.Tests
{
    public class ListControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;

        public ListControllerTests()
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
        public async Task AddMovieToList_WhenMovieMeetsConditions_UpdatesProgressAndReturnsOk()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockConfiguration = new Mock<IConfiguration>();
            var handlerMock = new Mock<HttpMessageHandler>();

            mockConfiguration.Setup(conf => conf["TMDB:ApiKey"]).Returns("c9154564bc2ba422e5e0dede6af7f89b");

            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var controller = new ListController(
    mockHttpClientFactory.Object,
    dbContext,
    mockAuthorizationService.Object,
    mockConfiguration.Object);


            var user = new User { Id = "1", UserName = "Test User" };
            dbContext.Users.Add(user);


            var challenge = new Challenge { Id = 1, Count = 1, Description="Įvykdyk peržiūrant filmą ilgesnį, negu 120min", Name="Peržiūrėti ilgą filmą" };
            dbContext.Challenges.Add(challenge);


            var condition = new ChallengeCondition { ChallengeId = challenge.Id, Type = "Runtime", Value = "120" };
            dbContext.ChallengeConditions.Add(condition);

            var userChallenge = new UserChallenge { UserId = user.Id, ChallengeId = challenge.Id };
            dbContext.UserChallenges.Add(userChallenge);

            await dbContext.SaveChangesAsync();

            var userId = "1";
            var claims = new List<Claim> { new Claim(JwtRegisteredClaimNames.Sub, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var mockController = new Mock<ListController>(mockHttpClientFactory.Object, dbContext, mockAuthorizationService.Object, mockConfiguration.Object)
            {
                CallBase = true
            };
            mockController.Object.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            mockController
                .Setup(x => x.FetchMovieDetails(It.IsAny<int>()))
                .ReturnsAsync(new MovieDto { Runtime = 120, Genres = new List<GenreDto>() });

            var movieDto = new MovieDto { Runtime = 120, Genres = new List<GenreDto>() };
            var jsonResponse = JsonConvert.SerializeObject(movieDto);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"key\":\"value\"}"), // or whatever your API returns
            };

            handlerMock
    .Protected()
    .Setup<Task<HttpResponseMessage>>(
        "SendAsync",
        ItExpr.IsAny<HttpRequestMessage>(),
        ItExpr.IsAny<CancellationToken>())
    .ReturnsAsync(response)
    .Verifiable();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);
            var httpClient = new HttpClient(handlerMock.Object);


            // Act
            var result = await mockController.Object.AddMovieToList(1);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            var updatedUserChallenge1 = await dbContext.UserChallenges.FirstOrDefaultAsync(u => u.UserId == userChallenge.UserId && u.ChallengeId == userChallenge.ChallengeId);
            Assert.True(updatedUserChallenge1.Completed);
        }
        [Fact]
        public async Task GetIsListedInMovies_ReturnsCorrectValue()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockConfiguration = new Mock<IConfiguration>();
            var handlerMock = new Mock<HttpMessageHandler>();

          
            var controller = new ListController(
    mockHttpClientFactory.Object,
    dbContext,
    mockAuthorizationService.Object,
    mockConfiguration.Object);

            var userId = "1";
            var movieId = 1;
            var user = new User { Id = userId, UserName = "User 1" };
            var myList = new MyList { UserId = userId, MovieID = movieId };

            dbContext.Users.Add(user);
            dbContext.Lists.Add(myList);
            await dbContext.SaveChangesAsync();

            var claim = new Claim(JwtRegisteredClaimNames.Sub, userId);
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            // Act
            var result = await controller.GetIsListedInMovies(movieId);

            // Assert
            Assert.True(result.Value);
        }
        [Fact]
        public async Task GetIsListedInMovies_ReturnsFalseIfNotListed()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockConfiguration = new Mock<IConfiguration>();
            var handlerMock = new Mock<HttpMessageHandler>();


            var controller = new ListController(
    mockHttpClientFactory.Object,
    dbContext,
    mockAuthorizationService.Object,
    mockConfiguration.Object);
            var userId = "1";
            var movieId = 1;
            var user = new User { Id = userId, UserName = "User 1" };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();


            var claim = new Claim(JwtRegisteredClaimNames.Sub, userId);
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            // Act
            var result = await controller.GetIsListedInMovies(movieId);

            // Assert
            Assert.False(result.Value);
        }
        [Fact]
        public async Task GetIsListedInPosts_ReturnsCorrectValue()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockConfiguration = new Mock<IConfiguration>();
            var handlerMock = new Mock<HttpMessageHandler>();


            var controller = new ListController(
    mockHttpClientFactory.Object,
    dbContext,
    mockAuthorizationService.Object,
    mockConfiguration.Object);
            var userId = "1";
            var movieId = 1;
            var postId = 1;
            var user = new User { Id = userId, UserName = "User 1" };
            var myList = new MyList { UserId = userId, MovieID = movieId };
            var post = new Post { Id = postId, MovieId = movieId, Body = "Meow", ImageUrl = "/odo.jpg" };

            dbContext.Users.Add(user);
            dbContext.Lists.Add(myList);
            dbContext.Posts.Add(post);
            await dbContext.SaveChangesAsync();

            // Set up user identity for controller
            var claim = new Claim(JwtRegisteredClaimNames.Sub, userId);
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            // Act
            var result = await controller.GetIsListedInPosts(postId);

            // Assert
            Assert.True(result.Value);
        }

        [Fact]
        public async Task GetIsListedInPosts_ReturnsFalseIfNotListed()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockConfiguration = new Mock<IConfiguration>();
            var handlerMock = new Mock<HttpMessageHandler>();


            var controller = new ListController(
    mockHttpClientFactory.Object,
    dbContext,
    mockAuthorizationService.Object,
    mockConfiguration.Object);
            var userId = "1";
            var movieId = 1;
            var postId = 1;
            var user = new User { Id = userId, UserName = "User 1" };
            var post = new Post { Id = postId, MovieId = movieId,Body="Meow",ImageUrl="/odo.jpg" };

            dbContext.Users.Add(user);
            dbContext.Posts.Add(post);
            await dbContext.SaveChangesAsync();

            // Set up user identity for controller
            var claim = new Claim(JwtRegisteredClaimNames.Sub, userId);
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            // Act
            var result = await controller.GetIsListedInPosts(postId);

            // Assert
            Assert.False(result.Value);
        }

        [Fact]
        public async Task GetIsListedInPosts_ReturnsNotFoundIfPostDoesNotExist()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockConfiguration = new Mock<IConfiguration>();
            var handlerMock = new Mock<HttpMessageHandler>();


            var controller = new ListController(
    mockHttpClientFactory.Object,
    dbContext,
    mockAuthorizationService.Object,
    mockConfiguration.Object);
            var userId = "1";
            var postId = 1;

            // Set up user identity for controller
            var claim = new Claim(JwtRegisteredClaimNames.Sub, userId);
            var identity = new ClaimsIdentity(new[] { claim });
            var principal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = principal }
            };

            // Act
            var result = await controller.GetIsListedInPosts(postId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.NotNull(notFoundResult);
        }

    }


}

