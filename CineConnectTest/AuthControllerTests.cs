using Bakis.Auth.Model;
using Bakis.Auth;
using Bakis.Controllers;
using Bakis.Data.Models;
using Bakis.Data;
using Bakis.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace CineConnectTest
{
    public class AuthControllerTests
    {
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<IJwtTokenService> _jwtTokenServiceMock;
        private Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IAuthorizationService> _mockAuthorizationService;

        public AuthControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _emailServiceMock = new Mock<IEmailService>();
        }

        [Fact]
        public async Task Register_ReturnsCreatedAtAction_WhenUserIsSuccessfullyCreated()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthenticationService>();

            authServiceMock
                .Setup(m => m.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(m => m.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var registerUserDto = new RegisterUserDto("TestUser", "test@test.com", "Test123!");
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var controller = new AuthController(_emailServiceMock.Object, _userManagerMock.Object, _jwtTokenServiceMock.Object);

            // Act
            var result = await controller.Register(registerUserDto);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<UserDto>(actionResult.Value);
            Assert.Equal(registerUserDto.UserName, returnValue.UserName);
            Assert.Equal(registerUserDto.Email, returnValue.Email);
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WhenUserIsValid()
        {
            // Arrange
            var serviceProviderMock = new Mock<IServiceProvider>();
            var mockJwtTokenService = new Mock<IJwtTokenService>();
           
            var mockEmailService = new Mock<IEmailService>();

            var user = new User { UserName = "TestUser", Id = "TestUserId" };
            _userManagerMock.Setup(x => x.FindByNameAsync("TestUser"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "TestPassword"))
                .ReturnsAsync(true);
            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });

            

            // Create a HttpContext and assign it to the ControllerContext
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
            };
            // Initialize your controller with the mocked HttpContext
            var controller = new AuthController(mockEmailService.Object, _userManagerMock.Object, mockJwtTokenService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                },
            };

            var loginDto = new LoginDto( "TestUser","TestPassword" );

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
