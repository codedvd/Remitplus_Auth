using System;
using System.Collections.Generic;

namespace Remitplus_Authentication.Models;

public partial class IpWhitelist
{
    public Guid Id { get; set; }

    public Guid ApplicationUserId { get; set; }

    public string IpAddress { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? CreatedById { get; set; }

    public virtual User ApplicationUser { get; set; } = null!;
}
