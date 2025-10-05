namespace Remitplus_Authentication.Models.Dtos;

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