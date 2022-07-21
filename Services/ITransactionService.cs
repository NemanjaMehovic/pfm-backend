using pfm.Models;

namespace pfm.Services;

public interface ITransactionService
{
    public Task<string> InsertMultiple(IEnumerable<Transaction> transactions);
}