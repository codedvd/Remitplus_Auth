using System;
using System.Collections.Generic;

namespace Remitplus_Authentication.Models;

public partial class IpblackList
{
    public Guid Id { get; set; }

    public string Ipaddress { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime UpdatedAt { get; set; }
}
