using System.ComponentModel.DataAnnotations;

namespace Remitplus_Authentication.Model
{
    public class ApplicationUserApiKeys
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public string ApiKeyHash { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime RevokedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string CreatedById { get; set; } = string.Empty;
        // Navigation property
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
