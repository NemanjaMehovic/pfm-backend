using pfm.Models;

namespace pfm.Services;

public interface ITransactionService
{
    public Task<string> InsertMultiple(IEnumerable<Transaction> transactions);

    public Task<string> Categorize(string transactionId, string categoryId);

    public Task<string> Split(string transactionId, IEnumerable<Split> splits);
}