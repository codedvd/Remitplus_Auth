using System;
using System.Collections.Generic;

namespace Remitplus_Authentication.Models;

public partial class WhitelistedIpLog
{
    public Guid Id { get; set; }

    public Guid ApplicationUserId { get; set; }

    public string IpAddress { get; set; } = null!;

    public string ApiRoute { get; set; } = null!;

    public string? HttpMethod { get; set; }

    public string? StatusCode { get; set; }

    public string? RequestPayload { get; set; }

    public string? ResponsePayload { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual User ApplicationUser { get; set; } = null!;
}
