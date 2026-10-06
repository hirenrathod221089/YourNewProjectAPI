namespace YourNewProjectAPI.AppCore.Interfaces;

public interface IUserRepository
{
    // Adheres to your lead's strict S4261 async naming rules!
    public Task<string?> GetPasswordHashByLoginIdAsync(string loginId);
}
