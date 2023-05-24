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
    public class CommentControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options;

        public CommentControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "CommentsDatabase")
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
        public async Task GetCommentsForPost_ReturnsCorrectComments()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new CommentController(dbContext, authorizationServiceMock.Object);

            var post = new Post { Id = 1, Body = "Tobuliausias filmas", ImageUrl = "/ldwqdq54daas2.jpg" };
            dbContext.Posts.Add(post);

            var user = new User { Id = "1" };
            dbContext.Users.Add(user);

            var comment1 = new Comment { Id = 1, Body = "Comment 1", UserId = "1", PostId = 1 };
            var comment2 = new Comment { Id = 2, Body = "Comment 2", UserId = "1", PostId = 1 };
            dbContext.Comments.AddRange(comment1, comment2);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comments = Assert.IsType<List<Comment>>(okResult.Value);
            Assert.Equal(2, comments.Count);
            Assert.Contains(comments, c => c.Id == comment1.Id);
            Assert.Contains(comments, c => c.Id == comment2.Id);

            // Add additional checks
            result = await controller.Get(2); // Non-existing post
            Assert.IsType<NotFoundResult>(result.Result);

            dbContext.Comments.RemoveRange(comment1, comment2);
            await dbContext.SaveChangesAsync();

            result = await controller.Get(1); // Post without comments
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async Task DeleteComment_DeletesCommentSuccessfully()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new CommentController(dbContext, authorizationServiceMock.Object);

            var user = new User { Id = "1" };
            dbContext.Users.Add(user);

            var post = new Post { Id = 1, Body = "Awesome Movie", ImageUrl = "/ldwqdq54das2.jpg" };
            dbContext.Posts.Add(post);

            var comment1 = new Comment { Id = 1, Body = "Comment 1", UserId = "1", PostId = 1 };
            dbContext.Comments.Add(comment1);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await controller.Delete(comment1.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedComment = Assert.IsType<Comment>(okResult.Value);
            Assert.Equal(comment1.Id, returnedComment.Id);

            var deletedComment = await dbContext.Comments.FindAsync(comment1.Id);
            Assert.Null(deletedComment);
        }
        [Fact]
        public async Task PostComment_CreatesNewComment()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var authorizationServiceMock = new Mock<IAuthorizationService>();
            var controller = new CommentController(dbContext, authorizationServiceMock.Object);

            var user = new User { Id = "1" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var post = new Post { Id = 1, Body="Tobulas filmas", ImageUrl="/ldwqdq54das2.jpg" };
            dbContext.Posts.Add(post);
            await dbContext.SaveChangesAsync();

            var comment = new Comment { Body = "Test comment", PostId = 1 };

            // Set up the user claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.Post(comment);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var comments = Assert.IsType<List<Comment>>(okResult.Value);
            Assert.Single(comments);
            Assert.Equal(comment.Body, comments[0].Body);
            Assert.Equal(comment.PostId, comments[0].PostId);
            Assert.Equal(comment.UserId, comments[0].UserId);
        }
    }
}
