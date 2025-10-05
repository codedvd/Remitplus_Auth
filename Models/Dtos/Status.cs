namespace Remitplus_Authentication.Models.Dtos
{
    public enum Status
    {
        Active,
        Inactive,
        Pending,
        Suspended,
        Deleted
    }

    public enum TransactionStatuses
    {
        All = 0,
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4,
    }

    public enum TransactionType
    {
        All = 0,
        Payment = 1,
        Refund = 2,
        Transfer = 3,
        Withdrawal = 4
    }
}
