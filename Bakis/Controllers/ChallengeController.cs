using Bakis.Auth.Model;
using Bakis.Data;
using Bakis.Data.Migrations;
using Bakis.Data.Models;
using Bakis.Data.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bakis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly ApplicationDbContext _databaseContext;
        private readonly IAuthorizationService _authorizationService;
        public ChallengeController(ApplicationDbContext context, IAuthorizationService authorizationService)
        {
            _databaseContext = context;

            _authorizationService = authorizationService;
        }

        private async Task<User> getCurrentUser()
        {
            return await _databaseContext.Users.FindAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        }
        private string getCurrentUserId()
        {
            return User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<Challenge>> CreateChallenge(ChallengeCreateDto challengeCreateDto)
        {
            var challenge = new Challenge
            {
                Name = challengeCreateDto.Name,
                Description = challengeCreateDto.Description,
                Count = challengeCreateDto.Count,
            };

            _databaseContext.Challenges.Add(challenge);
            await _databaseContext.SaveChangesAsync();


            foreach (var condition in challengeCreateDto.Conditions)
            {
                condition.ChallengeId = challenge.Id;
                _databaseContext.ChallengeConditions.Add(condition);
            }

            await _databaseContext.SaveChangesAsync();

            return Ok(challenge);
        }

        [HttpGet("MyChallenges")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<UserChallenge>>> Get()
        {
            var allList = await _databaseContext.UserChallenges
    .Include(uc => uc.Challenge)
    .ToListAsync();
            if (allList.Count == 0)
                return BadRequest("User has nothing in list");
            var List = allList.Where(s => s.UserId == getCurrentUserId()).ToList();//cia ne tiap

            var sortedChallenges = List.OrderByDescending(challenge =>
            {
                int progressPercentage = (challenge.Progress * 100) / challenge.Challenge.Count;
                return progressPercentage;
            }).ToList();

            return Ok(sortedChallenges);
        }

        [HttpGet("MyChallenges/{userName}")]
        public async Task<ActionResult<List<UserChallenge>>> GetUserList(string userName)
        {
            var user = await _databaseContext.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            var allList = await _databaseContext.UserChallenges
     .Include(uc => uc.Challenge)
     .ToListAsync();
            if (allList.Count == 0)
                return NotFound("There are nothing in Lists database");

            var List = allList.Where(s => s.UserId == user.Id).ToList();//cia ne tiap

            var sortedChallenges = List.OrderByDescending(challenge =>
            {
                int progressPercentage = (challenge.Progress * 100) / challenge.Challenge.Count;
                return progressPercentage;
            }).ToList();

            return Ok(sortedChallenges);
        }

        // GET api/challenges
        [HttpGet("notJoined")]
        public async Task<ActionResult<List<Challenge>>> GetChallengesNotJoined()
        {
            var currentUserId = getCurrentUserId();
            var userChallenges = await _databaseContext.UserChallenges
                .Where(uc => uc.UserId == currentUserId)
                .Select(uc => uc.ChallengeId)
                .ToListAsync();
            var challenges = await _databaseContext.Challenges
                .Where(c => !userChallenges.Contains(c.Id))
                .ToListAsync();

            if (challenges.Count == 0)
                return NoContent();

            return Ok(challenges);
        }
        [HttpGet]
        public async Task<ActionResult<List<Challenge>>> GetChallenges()
        {
            return await _databaseContext.Challenges.ToListAsync();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<Challenge>>> EditChallenge(int id, ChallengeEditDto request)
        {
            var challenge = await _databaseContext.Challenges.FindAsync(id);
            if (challenge == null)
                return BadRequest("Challenge not found");

            challenge.Name = request.Name ?? challenge.Name; ;
            challenge.Description = request.Description ?? challenge.Description; ;
            challenge.Count = request.Count ?? challenge.Count; ;
            challenge.Conditions = request.Conditions ?? challenge.Conditions;
            await _databaseContext.SaveChangesAsync();
            return Ok(challenge);
        }


        [HttpPost("UserChallenge")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<UserChallenge>>> JoinChallenge(UserChallenge challenge)
        {
            challenge.UserId = getCurrentUserId();

            var userChallenges = await _databaseContext.UserChallenges
   .Where(userC => userC.ChallengeId == challenge.ChallengeId && userC.UserId == challenge.UserId)
   .ToListAsync();

            if (userChallenges.Count != 0)
            {
                return Ok("Challenge already active");
            }

            _databaseContext.UserChallenges.Add(challenge);
            await _databaseContext.SaveChangesAsync();
            return Ok(await _databaseContext.UserChallenges.ToListAsync());
        }
    }
}
