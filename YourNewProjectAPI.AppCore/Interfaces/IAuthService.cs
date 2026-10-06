using YourNewProjectAPI.AppCore.Dto;

namespace YourNewProjectAPI.AppCore.Interfaces;

public interface IAuthService
{
    // Follows your lead's strict S4261 async naming rules!
    public Task<AuthSuccessResponseDto?> AuthenticateUserAsync(LoginRequestDto request);
}
