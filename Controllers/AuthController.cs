using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using RaceDayAPI.Data;
using RaceDayAPI.Models;



namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayAPI.Repositories.IUserRepository _userRepository;

        public AuthController(RaceDayAPI.Repositories.IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public IActionResult Register(string name, string email, string password, string role)
        {
            // Hash password before saving using ASP.NET Core Identity hasher
            var tempUser = new User { Name = name, Email = email, Role = role };
            var hasher = new PasswordHasher<User>();
            string hashedPassword = hasher.HashPassword(tempUser, password);

            var user = new User { Name = name, Email = email, Password = hashedPassword, Role = role };
            var added = _userRepository.Add(user);
            return Ok(added);
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null)
                return Unauthorized();

            var hasher = new PasswordHasher<User>();
            var verify = hasher.VerifyHashedPassword(user, user.Password, password);
            if (verify != PasswordVerificationResult.Success)
                return Unauthorized();

            // Store role in session (simplified for now)
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return Ok(new { message = "Login successful", role = user.Role });
        }
    }
}



