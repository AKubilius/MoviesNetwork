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
    [Route("[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IAuthorizationService _authorizationService;
        public FriendController(ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _databaseContext = context;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<Friend>>> GetUserFriends()
        {

            var allList = await _databaseContext.Friends.ToListAsync();
            if (allList.Count == 0)
                return NotFound("User has no friends");

            var List = allList.Where(s => s.UserId == User.FindFirstValue(JwtRegisteredClaimNames.Sub)).ToList();
            if (List.Count == 0)
                return BadRequest("User has no access");
            return Ok(List);
        }

        [HttpPost]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<Friend>>> CreateFriendship(Friend friend)
        {
            friend.UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            
            _databaseContext.Friends.Add(friend);
            await _databaseContext.SaveChangesAsync();
            return Ok(await _databaseContext.Friends.ToListAsync());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<Friend>>> Delete(int id)
        {
            var List = await _databaseContext.Friends.FindAsync(id);
            if (List == null)
                return BadRequest("List not found");

            var authResult = await _authorizationService.AuthorizeAsync(User, List, PolicyNames.ResourceOwner);
            if (!authResult.Succeeded)
            {
                return BadRequest("No permissions");
            }

            _databaseContext.Friends.Remove(List);
            await _databaseContext.SaveChangesAsync();
            return Ok(List);
        }
    }
}
