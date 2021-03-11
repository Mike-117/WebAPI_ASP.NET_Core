using AuthenticationPlugin;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]/[action]")] // action token added to route url
    [ApiController]
    public class UsersController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        // these next two lines allow the user to access a JWT
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public UsersController(CinemaDbContext dbContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _auth = new AuthService(_configuration);
            _dbContext = dbContext;
        }

        // this method registers the user with the app
        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            var userWithSameEmail =_dbContext.Users.Where(u => u.Email == user.Email).SingleOrDefault(); // this checks all the users to see if the user.Email input is a duplicate
            if (userWithSameEmail != null)
            {
                return BadRequest("That email is associated with another user");
            }

            var userObject = new User
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password), // this hashes the password before it is sent to the db; duplicate passwords won't matter
                Role = "Users" // new registrant will automatically be assigned the role of User, never Admin
            };

            _dbContext.Users.Add(userObject); // inserts this User object inside the database
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }


        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
           var userEmail = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (userEmail == null)
            {
                return NotFound();
            }
            if (!SecurePasswordHasherHelper.Verify(user.Password, userEmail.Password)) // this will check against the hashed password of a user who already exists in the database
            {
                return Unauthorized();
            }
            // if email exists and the user's password is authentic, we will generate a JWT
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, userEmail.Role)
            };
            var token = _auth.GenerateAccessToken(claims);
            // this displays the token in JSON format
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                user_id = userEmail.Id
            });
        }

    }
}
