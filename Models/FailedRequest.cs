using System;
using System.Collections.Generic;

namespace Remitplus_Authentication.Models;

public partial class FailedRequest
{
    public Guid FailId { get; set; }

    public Guid TransactionId { get; set; }

    public string Endpoint { get; set; } = null!;

    public string RequestPayload { get; set; } = null!;

    public string ErrorMessage { get; set; } = null!;

    public string? ResponsePayload { get; set; }

    public int RetryCount { get; set; }

    public int MaxRetries { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastRetryAt { get; set; }

    public DateTime? NextRetryAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual Transaction Transaction { get; set; } = null!;
}
