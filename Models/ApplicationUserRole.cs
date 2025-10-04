using System;
using System.Collections.Generic;

namespace Remitplus_Authentication.Models;

public partial class ApplicationUserRole
{
    public Guid RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? RoleDescription { get; set; }

    public DateTime CreatedDate { get; set; }
}
