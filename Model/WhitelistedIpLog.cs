using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Remitplus_Authentication.Model
{
    public class WhitelistedIpLog
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string ApiRoute { get; set; } = string.Empty;
        public string? HttpMethod { get; set; }
        public string? StatusCode { get; set; }
        public string? RequestPayload { get; set; }
        public string? ResponsePayload { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
