using System;
using System.Collections.Generic;

namespace Remitplus_Authentication.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastLoginAt { get; set; }

    public string? Status { get; set; }

    public Guid? RoleId { get; set; }

    public virtual ICollection<IpWhitelist> IpWhitelists { get; set; } = new List<IpWhitelist>();

    public virtual ICollection<UserApiKey> UserApiKeys { get; set; } = new List<UserApiKey>();

    public virtual ICollection<WhitelistedIpLog> WhitelistedIpLogs { get; set; } = new List<WhitelistedIpLog>();
}
