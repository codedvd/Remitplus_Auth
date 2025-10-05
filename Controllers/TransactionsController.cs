using Microsoft.AspNetCore.Mvc;
using Remitplus_Authentication.Interface;
using Remitplus_Authentication.Models.Request;

namespace Remitplus_Authentication.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TransactionsController(ITransactionService transactions) : ControllerBase
    {
        private readonly ITransactionService _transactions = transactions;

        [HttpPost("transaction/get-all-transactions")]
        public async Task<IActionResult> GetTransactions(GetTransactionQueryReq queryReq)
        {
            return Ok(await _transactions.GetAllSystemTransactions(queryReq));
        }

        [HttpGet("transaction/get-transaction/{tranId}")]
        public async Task<IActionResult> GetTransactionById(string tranId)
        {
            var transaction = await _transactions.GetTransactionById(tranId);
            if (transaction == null)
            {
                return NotFound();
            }
            return Ok(transaction);
        }

        [HttpPost("transaction/get-user-transactions")]
        public async Task<IActionResult> GetTransactionsByUserId([FromQuery]Guid userId, GetTransactionQueryReq queryReq)
        {
            var transactionsByUser = await _transactions.GetTransactionsByUserId(userId, queryReq);
            if (transactionsByUser == null)
            {
                return NotFound();
            }
            return Ok(transactionsByUser);
        }

        [HttpGet("transaction/get-user-summary")]
        public async Task<IActionResult> GetTransactionSummary(Guid userId)
        {
            var summary = await _transactions.GetTransactionSummary(userId);
            return Ok(summary);
        }
    }
}
