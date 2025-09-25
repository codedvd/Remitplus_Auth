using System.ComponentModel.DataAnnotations;

namespace Remitplus_Authentication.Model
{
    public class ApplicationUser
    {
        [Key]
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
        public string? ApiKeyId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }
    }
}
