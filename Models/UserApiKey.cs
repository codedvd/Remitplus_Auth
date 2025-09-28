using System;
using System.Collections.Generic;

namespace Remitplus_Authentication.Models;

public partial class UserApiKey
{
    public int Id { get; set; }

    public Guid ApplicationUserId { get; set; }

    public string ApiKeyHash { get; set; } = null!;

    public bool IsValid { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime ExpiryDate { get; set; }

    public string CreatedById { get; set; } = null!;

    public virtual User ApplicationUser { get; set; } = null!;
}
