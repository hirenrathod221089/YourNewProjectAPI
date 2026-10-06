namespace YourNewProjectAPI.AppCore.Dto;

// Sharp, lightweight record schemas that prevent mass assignment data leaks
public record LoginRequestDto(string LoginId, string Password);

public record AuthSuccessResponseDto(string AccessToken, string UserName, string AssignedRole);
