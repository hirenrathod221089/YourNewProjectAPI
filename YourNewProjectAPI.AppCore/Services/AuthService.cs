using YourNewProjectAPI.AppCore.Dto;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.AppCore.Services;

internal sealed class AuthService(IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<AuthSuccessResponseDto?> AuthenticateUserAsync(LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.LoginId) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        /* 1. TEMPORARILY COMMENTED OUT TO AVOID TOUCHING YOUR LIVE OFFICE DATABASE SEVER:
        string? storedHash = await unitOfWork.Users.GetPasswordHashByLoginIdAsync(request.LoginId);
        if (string.IsNullOrEmpty(storedHash)) return null;
        bool isPasswordValid = request.Password == storedHash; 
        */

        // 2. SAFE IN-MEMORY SIMULATION: Intercepts mock parameters instantly inside your computer's RAM!
        bool isPasswordValid = request.LoginId == "admin" && request.Password == "password123";

        if (!isPasswordValid)
        {
            return null; // Reject bad attempts immediately with a clean 401 Unauthorized block
        }

        // 3. Return a successful simulated identity response envelope
        return new AuthSuccessResponseDto(
            AccessToken: $"Simulated_Secure_JWT_Token_{Guid.NewGuid()}",
            UserName: "Officer-Hiren",
            AssignedRole: "Chitnish" // Matches your production authorization boundaries!
        );
    }
}
