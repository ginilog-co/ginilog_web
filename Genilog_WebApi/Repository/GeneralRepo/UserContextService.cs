namespace Genilog_WebApi.Repository.GeneralRepo
{
    public class UserContextService(IHttpContextAccessor httpContextAccessor) : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public string? GetIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;

            var ip = context?.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrEmpty(ip))
            {
                return ip.Split(',').FirstOrDefault();
            }

            return context?.Connection.RemoteIpAddress?.ToString();
        }

        public string? GetUserAgent()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
        }

        public string GetBrowserName()
        {
            var userAgent = GetUserAgent();

            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown Browser";

            if (userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase))
                return "Microsoft Edge";

            if (userAgent.Contains("OPR", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Opera", StringComparison.OrdinalIgnoreCase))
                return "Opera";

            if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                return "Google Chrome";

            if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase))
                return "Mozilla Firefox";

            if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase) &&
                !userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                return "Safari";

            return "Unknown Browser";
        }

        public string GetDeviceName()
        {
            var userAgent = GetUserAgent();

            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown Device";

            if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                return "Windows PC";

            if (userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
                return "Mac";

            if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
                return "Android Device";

            if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
                return "iPhone";

            if (userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                return "iPad";

            if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
                return "Linux Device";

            return "Unknown Device";
        }
    }
}