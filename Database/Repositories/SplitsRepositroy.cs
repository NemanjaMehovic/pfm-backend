using Microsoft.EntityFrameworkCore;
using pfm.Database.Entities;

namespace pfm.Database.Repositories;

public class SplitsRepository : IRepository<TransactionSplitsEntity, Tuple<string, string>>
{
    private PFMDbContext context;

    public SplitsRepository(PFMDbContext context)
    {
        this.context = context;
    }

    public async Task<TransactionSplitsEntity> Insert(TransactionSplitsEntity item)
    {
        context.transaction_splits.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<TransactionSplitsEntity> Update(TransactionSplitsEntity item)
    {
        context.transaction_splits.Update(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<TransactionSplitsEntity> Select(Tuple<string, string> id)
    {
        return await context.transaction_splits.FirstOrDefaultAsync(s => s.transactionId == id.Item1 && s.categoryId == id.Item2);
    }

    public async Task<bool> Delete(Tuple<string, string> id)
    {
        TransactionSplitsEntity split = await Select(id);
        if (split is null)
            return false;
        context.transaction_splits.Remove(split);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TransactionSplitsEntity>> SelectAll()
    {
        return await context.transaction_splits.ToListAsync();
    }

    public async Task<IEnumerable<TransactionSplitsEntity>> InsertMultiple(IEnumerable<TransactionSplitsEntity> items)
    {
        foreach (var split in items)
            context.transaction_splits.Add(split);
        await context.SaveChangesAsync();
        return items;
    }
}