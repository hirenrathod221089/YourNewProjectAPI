using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YourNewProjectAPI.WebAPI.Security;

// A custom authentication handler that executes at the absolute beginning of the pipeline
public class SimulatedAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // 1. Read your security feature configurations straight from authsettings.json
        bool isSimulated = configuration.GetValue<bool>("AuthenticationSettings:SimulationMode");
        string mockUser = configuration.GetValue<string>("AuthenticationSettings:DefaultSimulatedUser") ?? "Officer-Hiren";
        string mockRole = configuration.GetValue<string>("AuthenticationSettings:DefaultSimulatedRole") ?? "Chitnish";

        if (isSimulated)
        {
            // 2. Build secure .NET Identity Claims tracking your designated user role rules
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, mockUser),
                new Claim(ClaimTypes.Role, mockRole)
            };

            var identity = new ClaimsIdentity(claims, "SimulatedAuth");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "SimulatedAuth");

            // 3. Return a successful authentication pass package directly to the engine
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        return Task.FromResult(AuthenticateResult.Fail("Simulation mode is disabled."));
    }
}
