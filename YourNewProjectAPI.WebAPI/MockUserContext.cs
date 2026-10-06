using YourNewProjectAPI.AppCore.Interfaces;

namespace YourNewProjectAPI.WebAPI;

// Implements the contract cleanly strictly in-memory
public sealed class MockUserContext : IUserContext
{
    public int UserId => 101;
    public string LoginId => "admin";
    public string UserName => "Officer Hiren";
    public string DCode => "24";
    public string TCode => "01";
    public string OfficeCode => "OFF-MAIN";
    public string DesignationCode => "DES-CHITNISH";
    public string UserIpAddress => "127.0.0.1";
    public string SsoLiveToken => "Mock_JWT_SSO_Token_String";

    public void SetSessionValue(string key, string value)
    {
        // Silent loop for in-memory safety checks
    }
}
