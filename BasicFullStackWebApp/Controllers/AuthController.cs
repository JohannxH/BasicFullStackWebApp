using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

[Authorize] // This will protect all actions in this controller by default
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous] // This allows unauthenticated access to the login action
    public IActionResult Login([FromBody] UserLogin userLogin)
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == userLogin.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
        {
            return Unauthorized("Invalid username or password.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWT_KEY"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role) // Add role claim
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { token = tokenString });
    }

    [HttpGet("protected-data")]
    public IActionResult GetProtectedData()
    {
        return Ok(new { Data = "This is protected data." });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] UserLogin userLogin)
    {
        // Check if the user already exists
        if (_context.Users.Any(u => u.Username == userLogin.Username))
        {
            return BadRequest("User already exists.");
        }

        // Hash the password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userLogin.Password);

        // Create a new user
        var user = new User
        {
            Username = userLogin.Username,
            Password = hashedPassword, // Store the hashed password
            Role = "User" // Default role
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok("User registered successfully.");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-data")]
    public IActionResult GetAdminData()
    {
        return Ok(new { Data = "This is admin data." });
    }
}

public class UserLogin
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
