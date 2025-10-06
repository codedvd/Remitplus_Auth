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
        all = 0,
        pending = 1,
        completed = 2,
        failed = 3,
        cancelled = 4,
    }

    public enum TransactionType
    {
        all = 0,
        payment = 1,
        refund = 2,
        transfer = 3,
        withdrawal = 4
    }
}
