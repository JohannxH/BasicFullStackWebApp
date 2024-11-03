using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Authorize] // This will protect all actions in this controller by default using JWT bearer
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

    [HttpPost("login")]
    [AllowAnonymous]
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
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        string expiresString = "Token expires in 5 minutes at UTC: " + DateTime.UtcNow.AddMinutes(5).ToString();

        // Log the token in the database
        var tokenEntity = new Token
        {
            TokenString = tokenString,
            UserId = user.Id,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = tokenDescriptor.Expires.Value
        };

        _context.Tokens.Add(tokenEntity);
        _context.SaveChanges();

        return Ok(new { token = tokenString, expires = expiresString, role = user.Role });
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public IActionResult Logout()
    {
        // Invalidate the token on the client side by removing it from the client storage
        return Ok("User logged out successfully. " +
            "\n" +
            "\nNote:" +
            "\n JWT tokens are stateless and cannot be invalidated on the server once issued - the actual logout process " +
            "(i.e., invalidating the token) is typically handled on the client side using js");
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public IActionResult RefreshToken()
    {
        Debug.WriteLine("RefreshToken called");

        var userId = User.FindFirst("id")?.Value;
        if (userId == null)
        {
            return Unauthorized("Invalid token.");
        }

        var user = _context.Users.SingleOrDefault(u => u.Id == int.Parse(userId));
        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWT_KEY"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        string expiresString = "Token expires in 5 minutes at UTC: " + DateTime.UtcNow.AddMinutes(5).ToString();

        // Log the new token in the database
        var tokenEntity = new Token
        {
            TokenString = tokenString,
            UserId = user.Id,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = tokenDescriptor.Expires.Value
        };

        _context.Tokens.Add(tokenEntity);
        _context.SaveChanges();

        return Ok(new { token = tokenString, expires = expiresString, role = user.Role });
    }
}

public class UserLogin
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}
