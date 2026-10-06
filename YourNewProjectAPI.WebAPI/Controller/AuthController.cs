using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourNewProjectAPI.AppCore.Dto;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.WebAPI.Controllers;

[ApiController]
[Route("[controller]")] // Public route maps cleanly to: /Auth
[AllowAnonymous] // ◄ MANDATORY SHIELD: Allows anyone to access the login gate without a token!
internal sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // 1. Process credentials via your new secure business validation layer
        var authResult = await authService.AuthenticateUserAsync(request);

        if (authResult == null)
        {
            // 2. Security Best Practice: Return a generic 401 Unauthorized without disclosing too much detail!
            return Unauthorized(new { Success = false, Message = "Invalid Login ID or Password credentials dropped." });
        }

        // 3. Return your secure token envelope back to your frontline web portals
        return Ok(new
        {
            Success = true,
            Message = "Authentication verified successfully!",
            Result = authResult
        });
    }
}
