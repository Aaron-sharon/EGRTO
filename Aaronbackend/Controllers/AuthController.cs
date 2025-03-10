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
        private static List<User> users = new(); // Temporary in-memory storage
        public AuthController(DBclass context)
        {
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    return BadRequest("User already exists.");
                }

                _context.Users.Add(model);
                await _context.SaveChangesAsync();

                Console.WriteLine("User registered successfully."); // ✅ Log success
                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // ✅ Log error
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginRequest)
        {
            try
            {
                // Check if user exists
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginRequest.Email && u.PasswordHash == loginRequest.PasswordHash);

                if (user == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                // Store user info in session
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                HttpContext.Session.SetString("Username", user.Username);

                return Ok(new { Message = "Login successful.", UserId = user.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // ✅ Clears all session data
            return Ok(new { Message = "User logged out successfully." });
        }

        [HttpGet("check-session")]
        public IActionResult CheckSession()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Session expired or user not logged in.");
            }

            return Ok(new { Message = "User is logged in.", UserId = userId });
        }
    }
}
