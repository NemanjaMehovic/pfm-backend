using pfm.Database.Entities;
using pfm.Models;
using pfm.Controllers;

namespace pfm.Services;

public interface ITransactionService
{
    public Task<string> InsertMultiple(IEnumerable<Transaction> transactions);

    public Task<string> Categorize(string transactionId, string categoryId);

    public Task<string> Split(string transactionId, IEnumerable<Split> splits);

    public Task<TransactionsList> GetTransactions(TransactionKind? kind, DateTime? startTime, DateTime? endTime, string? sortBy, uint page, uint pageSize, SortOrderC order);

    public Task<Analytics> GetAnalytics(TransactionDirection? direction, DateTime? startTime, DateTime? endTime, string? catcode);

    public Task<bool> DeleteAll();

    public Task<bool> AutoCategorize();
}