namespace Genilog_WebApi.Repository.GeneralRepo
{
    public interface IUserContextService
    {
        string? GetIpAddress();
        string? GetUserAgent();
        string GetBrowserName();
        string GetDeviceName();
    }
}
