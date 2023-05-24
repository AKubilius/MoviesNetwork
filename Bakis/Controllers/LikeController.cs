using Bakis.Auth.Model;
using Bakis.Data;
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
    public class LikeController : ControllerBase
    {

        private readonly ApplicationDbContext _databaseContext;
        private readonly IAuthorizationService _authorizationService;
        public LikeController(ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _databaseContext = context;

            _authorizationService = authorizationService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<Like>>> GetLikesCount(int id)
        {
            var List = await _databaseContext.Posts.FindAsync(id);
            if (List == null)
                return  NotFound();

            var Likes = await _databaseContext.Likes.ToListAsync();
            if (Likes == null)
                return NoContent();

            var PostLikes = Likes.Where(s => s.PostId == id).ToList();
            if (PostLikes.Count == 0)
                return NoContent();
            return Ok(PostLikes.Count);
        }


        [HttpGet("isliked/{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<Like>>> GetIsLiked(int id)
        {
            var List = await _databaseContext.Posts.FindAsync(id);
            if (List == null)
                return NotFound();

            var UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            var PostLike = _databaseContext.Likes.SingleOrDefault(e => e.UserId == UserId && e.PostId == id);

            if (PostLike == null)
                return Ok(false);
            return Ok(true);
        }


        [HttpPost]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<Like>>> LikePost(Like Like) 
        {
            Like.UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            _databaseContext.Likes.Add(Like);
            await _databaseContext.SaveChangesAsync();
            return Ok(await _databaseContext.Likes.ToListAsync());
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<Like>>> UnlikePost(int id)
        {
            var _User = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var PostLike = _databaseContext.Likes.SingleOrDefault(e => e.UserId == _User && e.PostId == id);
            _databaseContext.Likes.Remove(PostLike);
            await _databaseContext.SaveChangesAsync();

            return Ok();
        }
    }
}
