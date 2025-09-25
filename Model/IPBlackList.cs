using System.ComponentModel.DataAnnotations;

namespace Remitplus_Authentication.Model
{
    public class IPBlackList
    {
        [Key]
        public Guid Id { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public DateTime UpdatedAt { get; set; }
    }
}
