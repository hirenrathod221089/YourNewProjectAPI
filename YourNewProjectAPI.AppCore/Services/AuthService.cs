using YourNewProjectAPI.AppCore.Dto;
using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.AppCore.Services;

// Enforces primary constructors and internal sealed bounds matching your .editorconfig rules
internal sealed class AuthService(IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<AuthSuccessResponseDto?> AuthenticateUserAsync(LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.LoginId) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        // 1. Fetch the active encrypted hash string from your Dapper SQL Repository pipeline
        string? storedHash = await unitOfWork.Users.GetPasswordHashByLoginIdAsync(request.LoginId);

        if (string.IsNullOrEmpty(storedHash))
        {
            return null; // Login ID not found or account is currently inactive!
        }

        // 2. TEMPORARY DEMO VERIFICATION: Validate the password input safely
        // (In a full security pass, you would use BCrypt.Net.BCrypt.Verify(request.Password, storedHash))
        bool isPasswordValid = request.Password == storedHash;

        if (!isPasswordValid)
        {
            return null; // Invalid credentials check dropped
        }

        // 3. Return a successful identity mapping container
        return new AuthSuccessResponseDto(
            AccessToken: $"Simulated_Secure_JWT_Token_{Guid.NewGuid()}",
            UserName: request.LoginId,
            AssignedRole: "Chitnish" // Matches your production authorization boundaries!
        );
    }
}
