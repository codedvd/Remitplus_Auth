using Microsoft.EntityFrameworkCore;
using Remitplus_Authentication.Models;
using Remitplus_Authentication.Models.Dtos;
using Remitplus_Authentication.Models.Request;

namespace Remitplus_Authentication.Interface
{
    public interface ITransactionService
    {
        Task<ApiResponse> GetAllSystemTransactions(GetTransactionQueryReq queryReq);
        Task<ApiResponse> GetTransactionById(string tranId);
        Task<ApiResponse> GetTransactionsByUserId(Guid userId, GetTransactionQueryReq queryReq);
        Task<ApiResponse> GetTransactionSummary(Guid userId);
    }

    public class TransactionService(RemitplusDatabaseContext context) : ITransactionService
    {
        private readonly RemitplusDatabaseContext _context = context;

        public async Task<ApiResponse> GetAllSystemTransactions(GetTransactionQueryReq queryReq)
        {
            var transactions = await (from trnx in _context.Transactions
                                      where trnx.BankReference != null
                                      select trnx).ToListAsync();

            if (transactions == null || transactions.Count == 0)
            {
                return ApiResponse.Failed("No transactions found.");
            }

            if (queryReq.TransactionType != TransactionType.All)
            {
                transactions = [.. transactions.Where(x => x.TransactionType == queryReq.TransactionType.ToString())];
            }

            if (queryReq.TransactionStatus != TransactionStatuses.All)
            {
                transactions = [.. transactions.Where(x => x.Status == queryReq.TransactionStatus.ToString())];
            }

            if (!String.IsNullOrEmpty(queryReq.StartDate) && !String.IsNullOrEmpty(queryReq.EndDate))
            {
                transactions = [.. transactions.Where(x => x.CreatedAt >= DateTime.Parse(queryReq.StartDate) && x.CreatedAt <= DateTime.Parse(queryReq.EndDate))];
            }
            return ApiResponse.Success("Transactions retrieved successfully", transactions);
        }

        public async Task<ApiResponse> GetTransactionById(string tranId)
        {
            var transaction = await (from transc in _context.Transactions
                                     where transc.TransactionReference == tranId
                                     select new TransactionDetail
                                     {
                                         Status = transc.Status,
                                         Amount = transc.Amount,
                                         CreatedAt = transc.CreatedAt,
                                         Currency = transc.Currency,
                                         Description = transc.Narration,
                                         ReferenceId = transc.TransactionReference,
                                         TransactionReference = transc.TransactionReference,
                                         UpdatedAt = transc.UpdatedAt,
                                         UserId = transc.UserId.ToString(),
                                         Type = transc.TransactionType

                                     }).FirstOrDefaultAsync();
            if (transaction == null)
            {
                return ApiResponse.Failed("Transaction not found");
            }
            return ApiResponse.Success("Transaction retrieved successfully", transaction);
        }

        public async Task<ApiResponse> GetTransactionsByUserId(Guid userId, GetTransactionQueryReq queryReq)
        {
            var transactions = await (from transc in _context.Transactions
                               where transc.UserId == userId
                               select new TransactionDetail
                               {
                                   Status = transc.Status,
                                   Amount = transc.Amount,
                                   CreatedAt = transc.CreatedAt,
                                   Currency = transc.Currency,
                                   Description = transc.Narration,
                                   ReferenceId = transc.TransactionReference,
                                   TransactionReference = transc.TransactionReference,
                                   UpdatedAt = transc.UpdatedAt,
                                   UserId = transc.UserId.ToString(),
                                   Type = transc.TransactionType
                               }).ToListAsync();
            if (transactions == null || transactions.Count == 0)
            {
                return ApiResponse.Failed("No transactions found for the user");
            }

            if(queryReq.TransactionType != TransactionType.All)
            {
                transactions = [.. transactions.Where(x => x.Type == queryReq.TransactionType.ToString())];
            }

            if(queryReq.TransactionStatus != TransactionStatuses.All)
            {
                transactions = [.. transactions.Where(x => x.Status == queryReq.TransactionStatus.ToString())];
            }

            if(!String.IsNullOrEmpty(queryReq.StartDate) && !String.IsNullOrEmpty(queryReq.EndDate))
            {
                transactions = [.. transactions.Where(x => x.CreatedAt >= DateTime.Parse(queryReq.StartDate) && x.CreatedAt <= DateTime.Parse(queryReq.EndDate))];
            }
            return ApiResponse.Success("Transactions retrieved successfully", transactions);
        }

        public async Task<ApiResponse> GetTransactionSummary(Guid UserId)
        {
            var transactions = await (from t in _context.Transactions
                                      where t.UserId == UserId
                                      select t).ToListAsync();

            var totalTransactions = transactions.Count;
            var totalAmount = transactions.Sum(x => x.Amount);
            var pending  = transactions.Count(x => x.Status == TransactionStatuses.Pending.ToString());
            var completed = transactions.Count(x => x.Status == TransactionStatuses.Completed.ToString());

            var recentTransactions = (from transc in transactions
                                      orderby transc.CreatedAt descending
                                      select new RecentTransaction
                                      {
                                          Status = transc.Status,
                                          Amount = transc.Amount,
                                          CreatedAt = transc.CreatedAt,
                                          Currency = transc.Currency,
                                          TransactionRef = transc.TransactionReference,
                                          Name = transc.DestinationAccount ?? ""
                                      }).Take(5).ToList();

            var summary = new TransactionSummary
            {
                TotalAmount = totalAmount,
                Completed = completed,
                Pending = pending,
                TotalTransactions = totalTransactions,
                RecentTransaction = recentTransactions
            };
            return ApiResponse.Success("Transaction summary retrieved successfully", summary);
        }
    }
}
