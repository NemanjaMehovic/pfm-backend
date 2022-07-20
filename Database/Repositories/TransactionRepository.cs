using Microsoft.EntityFrameworkCore;
using pfm.Database.Entities;

namespace pfm.Database.Repositories;

public class TransactionRepository : IRepository<TransactionEntity, string>
{
    private PFMDbContext context;

    public TransactionRepository(PFMDbContext context)
    {
        this.context = context;
    }

    public async Task<TransactionEntity> Insert(TransactionEntity item)
    {
        context.transactions.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<TransactionEntity> Update(TransactionEntity item)
    {
        context.transactions.Update(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<TransactionEntity> Select(string id)
    {
        return await context.transactions.FirstOrDefaultAsync(t => t.id == id);
    }

    public async Task<bool> Delete(string id)
    {
        TransactionEntity transaction = await Select(id);
        if(transaction is null)
            return false;
        context.transactions.Remove(transaction);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<TransactionEntity>> SelectAll()
    {
        return await context.transactions.ToListAsync();
    }

    public async Task<IEnumerable<TransactionEntity>> InsertMultiple(IEnumerable<TransactionEntity> items)
    {
        foreach(var transaction in items)
            context.transactions.Add(transaction);
        await context.SaveChangesAsync();
        return items;
    }
}