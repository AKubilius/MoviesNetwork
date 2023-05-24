using Bakis.Auth.Model;
using Bakis.Data.Models;
using Bakis.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Bakis.Data.Models.DTOs;

namespace Bakis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IAuthorizationService _authorizationService;
        public CommentController(ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _databaseContext = context;
            _authorizationService = authorizationService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<Comment>>> Get(int id)
        {
            var List = await _databaseContext.Posts.FindAsync(id);
            if (List == null)
                return NotFound();

            var Comments = await _databaseContext.Comments.Include(m => m.User)
        .Take(50)
        .ToListAsync();

            if (Comments == null)
                return NoContent();

            var PostComments = Comments.Where(s => s.PostId == id).ToList();
            if (PostComments.Count == 0)
                return NoContent();      

            return Ok(PostComments);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<Comment>>> Delete(int id)
        {
            var List = await _databaseContext.Comments.FindAsync(id);
            if (List == null)
                return BadRequest("List not found");
            _databaseContext.Comments.Remove(List);
            await _databaseContext.SaveChangesAsync();
            return Ok(List);
        }

        [HttpPost]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<Comment>>> Post(Comment comment) //STRINGAS ID MOVIE???? perdaryk
        {
            comment.UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            comment.User = await _databaseContext.Users.FindAsync(comment.UserId);

            _databaseContext.Comments.Add(comment);
            await _databaseContext.SaveChangesAsync();
            return Ok(await _databaseContext.Comments.ToListAsync());
        }
    }
}
