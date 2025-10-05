using Remitplus_Authentication.Models.Dtos;

namespace Remitplus_Authentication.Models.Request
{
    public class GetTransactionQueryReq
    {
        public string SearchTerm { get; set; } = string.Empty;
        public TransactionStatuses TransactionStatus { get; set; }
        public TransactionType TransactionType { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }
}
