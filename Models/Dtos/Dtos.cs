namespace Remitplus_Authentication.Models.Dtos;

public class RateData
{
    public string? CcY1 { get; set; }
    public string? CcY2 { get; set; }
    public decimal BuyRate { get; set; }
    public decimal SaleRate { get; set; }
    public decimal MidRate { get; set; }
    public string? RateDate { get; set; }
}

public class RateResponse
{
    public RateData? Data { get; set; }
    public int ErrorCode { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }
}

public class TransactionSummary
{
    public int TotalTransactions { get; set; }
    public decimal TotalAmount { get; set; }
    public int Pending { get; set; }
    public int Completed { get; set; }
    public List<RecentTransaction> RecentTransaction { get; set; } = [];
}

public class RecentTransaction
{
    public string TransactionRef { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}

public class GetKeyResponse
{
    public int Id { get; set; }
    public string ApiKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Expiry { get; set; } = string.Empty;
    public string LastUsed { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}