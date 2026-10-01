namespace YourNewProjectAPI.AppCore.Interfaces;

public interface IUserContext
{
    public int UserId { get; }
    public string LoginId { get; }
    public string UserName { get; }
    public string DCode { get; }
    public string TCode { get; }
    public string OfficeCode { get; }
    public string DesignationCode { get; }
    public string UserIpAddress { get; }
    public string SsoLiveToken { get; }
    public void SetSessionValue(string key, string value);
}
