using Bakis.Auth.Model;
using Bakis.Data;
using Bakis.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bakis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IAuthorizationService _authorizationService;
        public UserController(ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _databaseContext = context;

            _authorizationService = authorizationService;
        }

        private async Task<User> getCurrentUser()
        {
            return  await _databaseContext.Users.FindAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        }

        private string getCurrentUserId()
        {
            return User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        }

        private async Task<List<User>> GetCurrentUsersFriends()
        {
            var Friends = await _databaseContext.Friends.ToListAsync();
            var UserId = await getCurrentUser();
            var Users = await _databaseContext.Users.ToListAsync();

            List<User> friends = new List<User>();
            foreach (var friend in Friends)
            {
                if (friend.UserId == UserId.Id)
                    friends.Add(Users.SingleOrDefault(e => e.Id == friend.FriendId));

                if (friend.FriendId == UserId.Id)
                    friends.Add(Users.SingleOrDefault(e => e.Id == friend.UserId));
            }

            return friends;
        }


        [HttpGet("friends")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<User>>> GetFriends()
        {
            var friends = await GetCurrentUsersFriends();

            return Ok(friends);
        }

        [HttpGet]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            var currentUser = await getCurrentUser();
            var allUsers = await _databaseContext.Users.ToListAsync();
            var friends = await GetCurrentUsersFriends();

            if (allUsers.Count == 0)
                return BadRequest("There are no users available");

            allUsers.Remove(currentUser);
            allUsers.RemoveAll(user => friends.Contains(user));

            return Ok(allUsers);
        }

        [HttpGet("allUsers")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var allUsers = await _databaseContext.Users.ToListAsync();

            if (allUsers.Count == 0)
                return BadRequest("There are no users available");

            return Ok(allUsers);
        }



        [HttpGet("currentImage")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<User>>> GetCurrentUserImage()
        {
            var Users = await getCurrentUser();
            if (Users == null)
                return BadRequest("There are no users available");
            return Ok(Users.ProfileImageBase64);
        }

        [HttpGet("current")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var Users = await getCurrentUser();
            if (Users == null)
                return BadRequest("There are no users available");
            return Ok(Users);
        }

        [HttpGet("{userName}")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<User>>> GetCurrentUser(string userName)
        {
            var user = await _databaseContext.Users.SingleOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
                return BadRequest("There are no user available");
            return Ok(user);
        }

        [HttpPut]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<User>>> Update(User request)
        {
            var Users = await getCurrentUser();
            if (Users == null)
                return BadRequest("User was not found");

            Users.Name = request.Name;
            Users.Surname = request.Surname;
            await _databaseContext.SaveChangesAsync();
            return Ok(Users);
        }

        [HttpPost("upload-image"), DisableRequestSizeLimit]
        public async Task<ActionResult<List<User>>> UploadImage([FromForm] IFormFile imageFile)
        {
            var userId = getCurrentUserId();

            var user = await _databaseContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            user.ProfileImageBase64 = ConvertImageToBase64String(imageFile);
            await _databaseContext.SaveChangesAsync();

            return Ok();
        }
        public static string ConvertImageToBase64String(IFormFile imageFile)
        {
            using var ms = new MemoryStream();
            imageFile.CopyTo(ms);
            var fileBytes = ms.ToArray();
            return Convert.ToBase64String(fileBytes);
        }
    }
}
