using Bakis.Auth.Model;
using Bakis.Data;
using Bakis.Data.Migrations;
using Bakis.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bakis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchingRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IAuthorizationService _authorizationService;
        public WatchingRequestController(ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _databaseContext = context;
            _authorizationService = authorizationService;
        }
        public string getCurrentUserId()
        {
            return User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        }

        [HttpGet]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<WatchingRequest>>> Get()
        {
            var List = await _databaseContext.WatchingRequests.Where(e => e.FriendId == getCurrentUserId() || e.InvitedById == getCurrentUserId()).ToListAsync(); 
            return Ok(List);
        }

        [HttpGet("chat/{id}")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<WatchingRequest>> GetUserRequest(int id)
        {
            var watchingRequest = _databaseContext.WatchingRequests.SingleOrDefault(e => e.MessageId == id);
            return Ok(watchingRequest);
        }

        [HttpGet("{username}")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<WatchingRequest>>> GetUserRequest(string username)
        {
            var user = await _databaseContext.Users.SingleOrDefaultAsync(u => u.UserName == username);
            var List =  await _databaseContext.WatchingRequests.Where(e => e.FriendId == user.Id || e.InvitedById == user.Id).ToListAsync();
            return Ok(List);
        }

        [HttpPut("accept/{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<WatchingRequest>> AcceptRequest(int id)
        {
            var watchingRequest =  _databaseContext.WatchingRequests.SingleOrDefault(e => e.MessageId == id);
            watchingRequest.Status = Status.Accepted;
            await _databaseContext.SaveChangesAsync();
            return Ok(watchingRequest);
        }

        [HttpPut("decline/{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<WatchingRequest>> DeclineRequest(int id)
        {
            var watchingRequest = _databaseContext.WatchingRequests.SingleOrDefault(e => e.MessageId == id);
            watchingRequest.Status = Status.Declined;
            await _databaseContext.SaveChangesAsync();
            return Ok(watchingRequest);
        }

        // gal idesim.
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<WatchingRequest>>> CancelRequest(string id)
        {
            string myId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var List = _databaseContext.WatchingRequests.SingleOrDefault(e => e.FriendId == id && e.InvitedById == myId);

            if (List == null)
                return BadRequest("Request not found");

            _databaseContext.WatchingRequests.Remove(List);
            await _databaseContext.SaveChangesAsync();
            return Ok("Request canceled");
        }
    }
}
