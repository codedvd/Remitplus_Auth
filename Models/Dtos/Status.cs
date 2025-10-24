namespace Remitplus_Authentication.Models.Dtos
{
    public enum Status
    {
        Active,
        InActive,
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

    public enum UserStatus
    {
        All = 0,
        Active = 1,
        Inactive = 2,
    }

    public enum UserType
    {
        All = 0,
        Admin = 1,
        User = 2
    }
}
