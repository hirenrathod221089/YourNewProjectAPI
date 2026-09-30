using Microsoft.AspNetCore.Mvc;

namespace YourNewProjectAPI.WebAPI.Controllers;

// Enforces explicit internal visibility rules matching your team's .editorconfig rules
[ApiController]
[Route("v{v:apiVersion}/[controller]")]
internal abstract class ApiBaseController : ControllerBase
{
    // Centrally managed wrapper helper properties can live here in the future
}
