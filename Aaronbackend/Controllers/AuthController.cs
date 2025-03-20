using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Aaronbackend;
using AaronBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AaronBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DBclass _context;
        private readonly IConfiguration _configuration; // Added for JWT settings

        public AuthController(DBclass context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; // Injected configuration
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
            var hashedPassword = HashPassword(user.PasswordHash);
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == user.Username && u.PasswordHash == hashedPassword);

            if (existingUser == null)
            {
                return Unauthorized("Uh Oh, Invalid User or Password");
            }

            // 🔹 Generate JWT Token
            var token = GenerateJwtToken(existingUser);

            // 🔹 Store session (temporary until we fully switch to JWT)
            HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
            HttpContext.Session.SetString("Username", existingUser.Username);

            return Ok(new { Message = "Login successful.", Token = token });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logged out successfully.");
        }

        [Authorize]
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

        // 🔹 JWT Token Generation Method
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "Admin") // Optional Role Assignment
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Token expiry
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
