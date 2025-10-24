using Microsoft.EntityFrameworkCore;
using Remitplus_Authentication.Helper;
using Remitplus_Authentication.Models;
using Remitplus_Authentication.Models.Dtos;
using Remitplus_Authentication.Models.Request;

namespace Remitplus_Authentication.Interface
{
    public interface ITransactionService
    {
        Task<ApiResponse> GetAllSystemTransactions(GetTransactionQueryReq queryReq);
        Task<ApiResponse> GetCurrentSaleRates(string email);
        Task<ApiResponse> GetTransactionById(string tranId);
        Task<ApiResponse> GetTransactionsByUserId(Guid userId, GetTransactionQueryReq queryReq);
        Task<ApiResponse> GetTransactionSummary(Guid userId);
    }

    public class TransactionService(RemitplusDatabaseContext context, IRestClient apiCall, IConfiguration configuration, IEncryptionHandler encrypt) : ITransactionService
    {
        private readonly RemitplusDatabaseContext _context = context;
        private readonly IRestClient _apiCall = apiCall;
        private readonly IConfiguration _configuration = configuration;
        private readonly IEncryptionHandler _encrypt = encrypt;

        public async Task<ApiResponse> GetAllSystemTransactions(GetTransactionQueryReq queryReq)
        {
            var transactions = await (from trnx in _context.Transactions
                                      where trnx.BankReference != null
                                      select new
                                      {
                                          id = trnx.TransactionReference,
                                          userId = trnx.UserId,
                                          type = trnx.TransactionType,
                                          status = trnx.Status,
                                          amount = trnx.Amount,
                                          currency = trnx.Currency,
                                          description = trnx.Narration,
                                          referenceId = trnx.BankReference,
                                          createdAt = trnx.CreatedAt,
                                          updatedAt = trnx.UpdatedAt
                                      }).ToListAsync();

            if (transactions == null || transactions.Count == 0)
            {
                return ApiResponse.Failed("No transactions found.");
            }
            queryReq.SearchTerm = queryReq.SearchTerm.ToLower();
            if (!string.IsNullOrEmpty(queryReq.SearchTerm))
            {
                transactions = [.. transactions.Where(x => x.id.Contains(queryReq.SearchTerm, StringComparison.CurrentCultureIgnoreCase)
                || x.referenceId.Contains(queryReq.SearchTerm, StringComparison.CurrentCultureIgnoreCase)
                || x.status.Contains(queryReq.SearchTerm, StringComparison.CurrentCultureIgnoreCase) || x.description.Contains(queryReq.SearchTerm, StringComparison.CurrentCultureIgnoreCase))];
            }

            if (queryReq.TransactionType != TransactionType.all)
            {
                transactions = [.. transactions.Where(x => x.type.ToLower() == queryReq.TransactionType.ToString())];
            }

            if (queryReq.TransactionStatus != TransactionStatuses.all)
            {
                transactions = [.. transactions.Where(x => x.status.ToLower() == queryReq.TransactionStatus.ToString())];
            }

            if (!string.IsNullOrEmpty(queryReq.StartDate) && !string.IsNullOrEmpty(queryReq.EndDate))
            {
                transactions = [.. transactions.Where(x => x.createdAt >= DateTime.Parse(queryReq.StartDate) && x.createdAt <= DateTime.Parse(queryReq.EndDate))];
            }
            return ApiResponse.Success("Transactions retrieved successfully", transactions);
        }

        public async Task<ApiResponse> GetCurrentSaleRates(string email)
        {
            var getUser = await (from u in _context.Users
                                 join k in _context.UserApiKeys on u.UserId equals k.ApplicationUserId
                                 select k).FirstOrDefaultAsync();
            if (getUser == null)
                return ApiResponse.Failed("Failed to get user data.");
            var key = _encrypt.AESDecryptData(getUser.ApiKeyHash);

            var makeRequest = _apiCall.MakeApiCallAsync(
                url: $"{_configuration["BaseUrl"]}api/v1/Payments/getFTRate",
                method: HttpMethod.Get,
                headers: new Dictionary<string, string>
                {
                    { "XApiKey", key ?? string.Empty }
                }
            ).GetAwaiter().GetResult();

            return ApiResponse.Success("Rate returned successful", new
            {
                BuyRate = "1451",
                SellRate = "1475"
            });
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

            if (queryReq.TransactionType != TransactionType.all)
            {
                transactions = [.. transactions.Where(x => x.Type == queryReq.TransactionType.ToString())];
            }

            if (queryReq.TransactionStatus != TransactionStatuses.all)
            {
                transactions = [.. transactions.Where(x => x.Status == queryReq.TransactionStatus.ToString())];
            }

            if (!String.IsNullOrEmpty(queryReq.StartDate) && !String.IsNullOrEmpty(queryReq.EndDate))
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
            var pending = transactions.Count(x => x.Status == TransactionStatuses.pending.ToString());
            var completed = transactions.Count(x => x.Status == TransactionStatuses.completed.ToString());

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
