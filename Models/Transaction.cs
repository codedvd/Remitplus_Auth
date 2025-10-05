using System;
using System.Collections.Generic;

namespace Remitplus_Authentication.Models;

public partial class Transaction
{
    public Guid TransactionId { get; set; }

    public string AppId { get; set; } = null!;

    public string AuditId { get; set; } = null!;

    public string BankReference { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string TransactionType { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Narration { get; set; } = null!;

    public string? SourceAccount { get; set; }

    public string? DestinationAccount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string TransactionReference { get; set; } = null!;

    public Guid UserId { get; set; }

    public virtual ICollection<FailedRequest> FailedRequests { get; set; } = new List<FailedRequest>();
}
