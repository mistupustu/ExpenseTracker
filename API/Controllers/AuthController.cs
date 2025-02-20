using API.Database;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public ActionResult<LoginResponse> LogIn(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
                return BadRequest(new { message = "Username and password are required." });

            var user = _context.Users.FirstOrDefault(u => u.Username == loginRequest.Username);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Invalid username or password." });

            var token = GenerateJwtToken(user);

            return Ok(new LoginResponse
            {
                Token = token
            });
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public ActionResult Register(RegisterRequest registerRequest)
        {
            var validity = IsValidUser(registerRequest);
            if (validity is not OkObjectResult)
                return validity;
            User user = new User { 
                Username = registerRequest.Username,
                Email = registerRequest.Email,
                Role = registerRequest.Role,
                Password = registerRequest.Password
            };
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully." });
        }

        private ActionResult IsValidUser(RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Username) || string.IsNullOrWhiteSpace(registerRequest.Password))
                return BadRequest(new { message = "Username and password are required." });

            if (registerRequest.Username.Length > 50 || registerRequest.Username.Length < 5)
                return BadRequest(new { message = "Username must be between 5 and 50 characters." });

            if (registerRequest.Password.Length < 6)
                 return BadRequest(new { message = "Password must be at least 6 characters long." });

            if (_context.Users.Any(u => u.Username == registerRequest.Username))
                return Conflict(new { message = "Username already taken." });

            if (!Enum.IsDefined(typeof(UserRole), registerRequest.Role))
                return BadRequest(new { message = "Role must be ADMIN or USER." });

            return Ok(new { message = "User is valid." });
        }

        // Generate JWT Token
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expireHours = int.Parse(_config["Jwt:ExpireHours"] ?? "5");

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expireHours),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
