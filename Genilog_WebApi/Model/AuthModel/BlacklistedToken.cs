using System.ComponentModel.DataAnnotations;

namespace Genilog_WebApi.Model.AuthModel
{
    public class BlacklistedToken
    {
        [Key]
        public int Id { get; set; }
        public string Jti { get; set; } = null!;
        public DateTime Expiry { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
