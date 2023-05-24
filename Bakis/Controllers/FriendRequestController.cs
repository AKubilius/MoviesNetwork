using Bakis.Auth.Model;
using Bakis.Data.Models;
using Bakis.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Bakis.Data.Migrations;

namespace Bakis.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FriendRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IAuthorizationService _authorizationService;
        public FriendRequestController(ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _databaseContext = context;
            _authorizationService = authorizationService;
        }
        

        [HttpGet]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<FriendRequest>>> GetUserRequests() 
        {
            var allList = await _databaseContext.FriendRequests.ToListAsync();
            if (allList.Count == 0)
                return NotFound("There are no friend requests");

            var Requests = allList.Where(s => s.FriendId == User.FindFirstValue(JwtRegisteredClaimNames.Sub)).ToList();
            if (Requests.Count == 0)
                return NotFound("User has no friend requests");
            return Ok(Requests);
        }

        [HttpGet("/users")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<FriendRequest>>> GetUsersRequests() 
        {      
            var allList = await _databaseContext.FriendRequests.ToListAsync();
            if (allList.Count == 0)
                return NotFound("There are no friend requests");

            var Requests = allList.Where(s => s.FriendId == User.FindFirstValue(JwtRegisteredClaimNames.Sub)).ToList();
            if (Requests.Count == 0)
                return NotFound("User has no friend requests");

            var Users = await _databaseContext.Users.ToListAsync();

            List<User> users = new List<User>();
            foreach (var request in Requests)
            {
                foreach (var user in Users)
                {
                    if (request.InvitedBy == user.Id)
                    {
                        users.Add(user);
                    }
                }
            }
            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<FriendRequest>>> InviteFriend(FriendRequest friend)
        {
            var Users = await _databaseContext.Users.FindAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
            friend.InvitedBy = Users.Id;
            friend.Name = Users.UserName;
            
            var List =  _databaseContext.FriendRequests.SingleOrDefault(e => e.FriendId == friend.FriendId && e.InvitedBy == friend.InvitedBy);
            if (List !=null)
            {
                return BadRequest("Friend request already exist");
            }
            _databaseContext.FriendRequests.Add(friend);
            
            await _databaseContext.SaveChangesAsync();
            var Created =_databaseContext.FriendRequests.SingleOrDefault(e => e.FriendId == friend.FriendId && e.InvitedBy == friend.InvitedBy);// cia paziurek veliau ar tvarkyt, nes lyg nesjanas nuo piramis

            return Ok("Invite was sent");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<FriendRequest>>> CancelRequest(string id)
        {
            string myId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var List = _databaseContext.FriendRequests.SingleOrDefault(e => e.FriendId == id && e.InvitedBy == myId);// cia same apdaryt ka ten dariau

            if (List == null)
                return BadRequest("Request not found");

            _databaseContext.FriendRequests.Remove(List);
            await _databaseContext.SaveChangesAsync();
            return Ok("Request canceled");
        }

        [HttpPost("accept/{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<FriendRequest>>> AcceptRequest(int id)
        {
            var List = await _databaseContext.FriendRequests.FindAsync(id);
            if (List == null)
                return BadRequest("List not found");

            Friend a = new Friend();
            a.FriendId = List.FriendId;
            a.UserId = List.InvitedBy;
            _databaseContext.Friends.Add(a);

            Room newRoom = new Room
            {
                Name = "Room for " + List.FriendId + " and " + List.InvitedBy
            };
            _databaseContext.Rooms.Add(newRoom);

            await _databaseContext.SaveChangesAsync();

            UserRoom userRoom1 = new UserRoom
            {
                UserId = List.FriendId,
                RoomId = newRoom.Id
            };
            UserRoom userRoom2 = new UserRoom
            {
                UserId = List.InvitedBy,
                RoomId = newRoom.Id
            };

            _databaseContext.UserRooms.Add(userRoom1);
            _databaseContext.UserRooms.Add(userRoom2);

            _databaseContext.FriendRequests.Remove(List);
            await _databaseContext.SaveChangesAsync();
            return Ok(List);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<FriendRequest>>> DeleteRequest(int id)
        {
            var List = await _databaseContext.FriendRequests.FindAsync(id);
            if (List == null)
                return NoContent();
            _databaseContext.FriendRequests.Remove(List);
            await _databaseContext.SaveChangesAsync();
            return Ok(List);
        }

    }
}
