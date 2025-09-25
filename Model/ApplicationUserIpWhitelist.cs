using System.ComponentModel.DataAnnotations;

namespace Remitplus_Authentication.Model
{
    public class ApplicationUserIpWhitelist
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
