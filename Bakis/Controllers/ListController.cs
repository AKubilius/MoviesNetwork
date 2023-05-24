using Bakis.Auth.Model;
using Bakis.Data;
using Bakis.Data.Models;
using Bakis.Data.Models.DTOs;
using IO.Ably;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Bakis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListController : ControllerBase
    {
        private const string TmdbApiBase = "https://api.themoviedb.org/3/movie/";
        private readonly ApplicationDbContext _databaseContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly HttpClient _httpClient;
        private readonly string API_KEY;
        public ListController(IHttpClientFactory httpClientFactory, ApplicationDbContext context,  IAuthorizationService authorizationService, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _databaseContext = context;
            _authorizationService = authorizationService;
            API_KEY = configuration["TMDB:ApiKey"];
        }

        private async Task<User> getCurrentUser()
        {
            return await _databaseContext.Users.FindAsync(User.FindFirstValue(JwtRegisteredClaimNames.Sub));
        }

        private string getCurrentUserId()
        {
            return User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        }
        [HttpGet("Calculate")]
        //[Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<MyList>>> Calculate()
        {
            CalculateCompatibilityWithAllUsersAndStore(User.FindFirstValue(JwtRegisteredClaimNames.Sub));

            return Ok();
        }

        [HttpGet("Mylist")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<MyList>>> Get()
        {
            var allList = await _databaseContext.Lists.ToListAsync();
            if (allList.Count == 0)
                return BadRequest("User has nothing in list");
            var List = allList.Where(s => s.UserId == getCurrentUserId()).ToList();//cia ne tiap
            return Ok(List);
        }

        [HttpGet("Mylist/{userName}")]
        public async Task<ActionResult<List<MyList>>> GetUserList(string userName)
        {
            var user = await _databaseContext.Users.SingleOrDefaultAsync(u => u.UserName == userName);
            var allList = await _databaseContext.Lists.ToListAsync();
            if (allList.Count == 0)
                return NotFound("There are nothing in Lists database");

            var List = allList.Where(s => s.UserId == user.Id).ToList();//cia ne tiap
            return Ok(List);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<List<MyList>>> Delete(int id)
        {

            var movieList = _databaseContext.Lists.SingleOrDefault(e => e.UserId == User.FindFirstValue(JwtRegisteredClaimNames.Sub) && e.MovieID == id);

            _databaseContext.Lists.Remove(movieList);
            await _databaseContext.SaveChangesAsync();
            return Ok(movieList);
        }

        public async Task<ActionResult> AddMovieToList(int movieId)
        {

            var movie = await FetchMovieDetails(movieId);

            if (movie == null)
            {

                return BadRequest("Could not fetch movie details from TMDB.");
            }

            var allChallenges = await _databaseContext.UserChallenges.ToListAsync();

            if (allChallenges.Count == 0)
                return BadRequest("No challenges");

            

            var activeUserChallenges = allChallenges.Where(s => s.UserId == User.FindFirstValue(JwtRegisteredClaimNames.Sub)).ToList();

            if (activeUserChallenges.Count == 0)
                return BadRequest("User has no active challenges");

            
            foreach (var userChallenge in activeUserChallenges)
            {

                var challenge = await _databaseContext.Challenges
                     .Include(c => c.Conditions)
                    .FirstOrDefaultAsync(c => c.Id == userChallenge.ChallengeId);

                if (challenge != null)
                {

                    bool allConditionsMet = true;
                    foreach (var condition in challenge.Conditions)
                    {
                        if (!CheckMovieCondition(movie, condition))
                        {
                            allConditionsMet = false;
                            break;
                        }
                    }

                    if (allConditionsMet)
                    {

                        userChallenge.Progress++;

                        if (userChallenge.Progress >= challenge.Count)
                        {
                            userChallenge.Completed = true;
                        }
                    }
                }
            }

            await _databaseContext.SaveChangesAsync();

            return Ok();
        }

        public virtual async Task<MovieDto> FetchMovieDetails(int movieId)
        {
           
            string url = $"{TmdbApiBase+movieId}?api_key={API_KEY}&language=lt-LT";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var movie = JsonConvert.DeserializeObject<MovieDto>(jsonResponse);
                    return movie;
                }
            }

            return null;
        }


        private bool CheckMovieCondition(MovieDto movie, ChallengeCondition condition)
        {
            bool conditionMet = false;

            if (condition.Type == "Runtime")
            {
                int requiredRuntime = Int32.Parse(condition.Value);

                if (movie.Runtime >= requiredRuntime)
                {
                    conditionMet = true;
                }
            }

            if (condition.Type == "Genre")
            {

                if (movie.Genres.Any(g => g.Name == condition.Value))
                {
                    conditionMet = true;
                }
            }

            if (condition.Type == "Any")
            {
                    conditionMet = true;
            }

            return conditionMet;
        }

        //STRINGAS ID MOVIE???? perdaryk
        [HttpPost("Mylist")]
        [Authorize(Roles = Roles.User)]
        public async Task<ActionResult<List<MyList>>> AddMovieToList(MyList List) 
        {
            List.UserId = getCurrentUserId();

            var movieList = await _databaseContext.Lists
               .Where(list => list.MovieID == List.MovieID && list.UserId == List.UserId)
               .ToListAsync();

            if (movieList.Count != 0)
            {
                return Ok("Movie already in list");
            }
            _databaseContext.Lists.Add(List);
            await _databaseContext.SaveChangesAsync();

            var movieLista = await _databaseContext.Lists.ToListAsync();

            await AddMovieToList(List.MovieID);

            return Ok(await _databaseContext.Lists.ToListAsync());
        }

        [HttpGet("isListed/{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<Boolean>> GetIsListedInPosts(int id)
        {
            var List = await _databaseContext.Posts.FindAsync(id);
            if (List == null)
                return NotFound();
            var UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var inList = _databaseContext.Lists.SingleOrDefault(e => e.UserId == UserId && e.MovieID == List.MovieId);
            if (inList == null)
                return Ok(false);
            return Ok(true);
        }

        [HttpGet("listedMovie/{id}")]
        [Authorize(Roles = Roles.User + "," + Roles.Admin)]
        public async Task<ActionResult<Boolean>> GetIsListedInMovies(int id)
        {
            var UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var inList = _databaseContext.Lists.SingleOrDefault(e => e.UserId == UserId && e.MovieID == id);
            if (inList == null)
                return Ok(false);
            return Ok(true);
        }


        public async Task<List<int>> GetUserMovieIdsAsync(string userId)
        {
            var movieList = await _databaseContext.Lists
                .Where(list => list.UserId == userId)
                .ToListAsync();

            return movieList.Select(list => list.MovieID).ToList();
        }

        public async Task<double> CalculateCompatibility(string userId1, string userId2)
        {
            // Retrieve movie IDs from the database
            var user1Movies = await GetUserMovieIdsAsync(userId1);
            var user2Movies = await GetUserMovieIdsAsync(userId2);

            // Fetch movie criteria for both users
            var user1Criteria = new HashSet<string>();
            var user2Criteria = new HashSet<string>();

            foreach (var movieId in user1Movies)
            {
                var criteria = await GetMovieCriteriaAsync(movieId);
                user1Criteria.UnionWith(criteria);
            }

            foreach (var movieId in user2Movies)
            {
                var criteria = await GetMovieCriteriaAsync(movieId);
                user2Criteria.UnionWith(criteria);
            }

            // Calculate the Jaccard similarity index
            double similarity = CalculateJaccardSimilarity(user1Criteria, user2Criteria);
            double compatibilityPercentage = similarity * 100;


            var existingRecord = await _databaseContext.UserCompatibilities
               .FirstOrDefaultAsync(uc => (uc.UserId1 == userId1 && uc.UserId2 == userId2) || (uc.UserId1 == userId2 && uc.UserId2 == userId1));

            if (existingRecord != null)
            {
                existingRecord.Compatibility = compatibilityPercentage;
                existingRecord.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                var newUserCompatibility = new UserCompatibility
                {
                    UserId1 = userId1,
                    UserId2 = userId2,
                    Compatibility = compatibilityPercentage,
                    LastUpdated = DateTime.UtcNow
                };
                _databaseContext.UserCompatibilities.Add(newUserCompatibility);
            }
            await _databaseContext.SaveChangesAsync();

            return compatibilityPercentage;
        }

        [HttpGet("compatibility/{userName}")]
        public async Task<IActionResult> GetCompatibility(string userName)
        {

            var user = await _databaseContext.Users.SingleOrDefaultAsync(u => u.UserName == userName);

            var userId2 = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var compatibility = await CalculateCompatibility(user.Id, userId2);
            return Ok(new { Compatibility = compatibility });
        }

        public async Task<HashSet<string>> GetMovieCriteriaAsync(int movieId)
        {
            var genres = await GetMovieGenresAsync(movieId);
            var directors = await GetMovieDirectorsAsync(movieId);
            var actors = await GetMovieActorsAsync(movieId);

            genres.UnionWith(directors);
            genres.UnionWith(actors);

            return genres;
        }

        static double CalculateJaccardSimilarity(HashSet<string> setA, HashSet<string> setB)
        {
            var intersection = setA.Intersect(setB).Count();
            var union = setA.Union(setB).Count();

            if (union == 0) return 0.0;

            return (double)intersection / union;
        }

        public async Task<HashSet<string>> GetMovieGenresAsync(int movieId)
        {
            var genres = new HashSet<string>();

            var url = $"https://api.themoviedb.org/3/movie/{movieId}?api_key={API_KEY}&language=en-US";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            dynamic json = JObject.Parse(content);
            foreach (var genre in json.genres)
            {
                genres.Add("genre_" + (string)genre.name); // Prefix to differentiate from other criteria
            }

            return genres;
        }

        public async Task<HashSet<string>> GetMovieDirectorsAsync(int movieId)
        {
            var directors = new HashSet<string>();

            var url = $"https://api.themoviedb.org/3/movie/{movieId}/credits?api_key={API_KEY}";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            dynamic json = JObject.Parse(content);
            foreach (var crew in json.crew)
            {
                if (crew.job == "Director")
                {
                    directors.Add("director_" + (string)crew.name); // Prefix to differentiate from other criteria
                }
            }

            return directors;
        }

        public async Task<HashSet<string>> GetMovieActorsAsync(int movieId)
        {
            var actors = new HashSet<string>();

            var url = $"https://api.themoviedb.org/3/movie/{movieId}/credits?api_key={API_KEY}";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            dynamic json = JObject.Parse(content);
            foreach (var cast in json.cast)
            {
                actors.Add("actor_" + (string)cast.name); // Prefix to differentiate from other criteria
            }

            return actors;
        }

        public async Task StoreOrUpdateCompatibilityAsync(string userId1, string userId2, double compatibility)
        {
            var existingRecord = await _databaseContext.UserCompatibilities
                .FirstOrDefaultAsync(uc => (uc.UserId1 == userId1 && uc.UserId2 == userId2) || (uc.UserId1 == userId2 && uc.UserId2 == userId1));

            if (existingRecord != null)
            {
                existingRecord.Compatibility = compatibility;
                existingRecord.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                var newUserCompatibility = new UserCompatibility
                {
                    UserId1 = userId1,
                    UserId2 = userId2,
                    Compatibility = compatibility,
                    LastUpdated = DateTime.UtcNow
                };
                _databaseContext.UserCompatibilities.Add(newUserCompatibility);
            }

            await _databaseContext.SaveChangesAsync();
        }

        public async Task CalculateCompatibilityWithAllUsersAndStore(string userId)
        {
            var allUsers = await GetAllUsersAsync();

            foreach (var user in allUsers)
            {
                if (user.Id != userId)
                {
                    double compatibility = await CalculateCompatibility(userId, user.Id);
                    await StoreOrUpdateCompatibilityAsync(userId, user.Id, compatibility);
                }
            }
        }

        public async Task<List<User>> GetSortedCompatibilitiesFromDatabase(string currentUserId)
        {
            var compatibilities = await _databaseContext.UserCompatibilities
                .Where(uc => uc.UserId1 == currentUserId || uc.UserId2 == currentUserId)
                .ToListAsync();

            var sortedCompatibilities = compatibilities
                .Select(uc => new KeyValuePair<string, double>(uc.UserId1 == currentUserId ? uc.UserId2 : uc.UserId1, uc.Compatibility))
                .OrderByDescending(kv => kv.Value)
                .ToList();

            var users = await _databaseContext.Users.ToListAsync();
            List<User> users1 = new List<User>();

            var friendList = await _databaseContext.Friends
                .Where(f => f.UserId == currentUserId || f.FriendId == currentUserId)
                .ToListAsync();

            HashSet<string> friendIds = new HashSet<string>();
            foreach (var friend in friendList)
            {
                friendIds.Add(friend.UserId == currentUserId ? friend.FriendId : friend.UserId);
            }

            foreach (var item in sortedCompatibilities)
            {
                if (item.Value > 0)
                {
                    var user = users.FirstOrDefault(s => s.Id == item.Key);
                    if (!friendIds.Contains(user.Id))
                    {
                        users1.Add(user);
                    }
                }
            }


            return users1;
        }

        [HttpGet("sorted-compatibility")]
        public async Task<IActionResult> GetSortedCompatibility()
        {
            var currentUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var sortedCompatibilities = await GetSortedCompatibilitiesFromDatabase(currentUserId);

            return Ok(new { SortedCompatibility = sortedCompatibilities });
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _databaseContext.Users.ToListAsync();
        }

    }
}
