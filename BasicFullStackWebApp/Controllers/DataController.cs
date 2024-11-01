using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class DataController : ControllerBase
{
    [HttpGet("public-data")]
    public IActionResult GetPublicData()
    {
        return Ok(new { Data = "This is public data." });
    }

    [Authorize]
    [HttpGet("protected-data")]
    public IActionResult GetProtectedData()
    {
        return Ok(new { Data = "This is protected data - thus the user needs to be logged in / authorized" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-data")]
    public IActionResult GetAdminData()
    {
        return Ok(new { Data = "This is admin data and require Admin Roles" });
    }
}
