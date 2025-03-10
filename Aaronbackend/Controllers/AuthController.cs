using AaronBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace AaronBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static List<User> users = new(); // Temporary in-memory storage

        [HttpPost("signup")]
        public IActionResult Signup([FromBody] User user)
        {
            if (users.Any(u => u.Username == user.Username))
            {
                return BadRequest("User already exists.");
            }

            user.Id = users.Count + 1;
            users.Add(user);
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var existingUser = users.FirstOrDefault(u => u.Username == user.Username && u.PasswordHash == user.PasswordHash);
            if (existingUser == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Store session
            HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
            HttpContext.Session.SetString("Username", existingUser.Username);

            return Ok(new { Message = "Login successful." });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out successfully.");
        }

        [HttpGet("check-session")]
        public IActionResult CheckSession()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Session expired or not found.");
            }
            return Ok(new { Username = username });
        }
    }
}
