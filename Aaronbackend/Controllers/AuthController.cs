using System.Security.Cryptography;
using System.Text;
using Aaronbackend;
using AaronBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AaronBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DBclass _context;
        public AuthController(DBclass context)
        {
            _context = context;
        }

        private static List<User> users = new(); // Temporary in-memory storage

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] User user)
        {
            if (_context.Users.Any(u => u.Username == user.Username))
            {
                return BadRequest("User already exists.");
            }

            user.PasswordHash = HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var hashedPassword = HashPassword(user.PasswordHash); // Hash the incoming password
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username && u.PasswordHash == hashedPassword);

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
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}