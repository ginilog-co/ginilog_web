namespace Genilog_WebApi.Model.AuthModel
{
    public class LoginDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Email { get; set; }
        public Guid UserId { get; set; }
        public string? UserType { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? PhoneVerified { get; set; }
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }

    public class AdminLoginDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Email { get; set; }
        public Guid UserId { get; set; }
        public string? UserType { get; set; }
        public bool? EmailVerified { get; set; }
        public bool? PhoneVerified { get; set; }
        public string? FullName { get; set; }
        public string? State { get; set; }
        public string? Locality { get; set; }
        public string? Address { get; set; }
        public string? Branch { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

    }
}
