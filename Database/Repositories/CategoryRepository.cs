using Microsoft.EntityFrameworkCore;
using pfm.Database.Entities;

namespace pfm.Database.Repositories;

public class CategoryRepository : IRepository<CategoryEntity, string>
{
    private PFMDbContext context;

    public CategoryRepository(PFMDbContext context)
    {
        this.context = context;
    }

    public async Task<CategoryEntity> Insert(CategoryEntity item)
    {
        context.categories.Add(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<CategoryEntity> Update(CategoryEntity item)
    {
        context.categories.Update(item);
        await context.SaveChangesAsync();
        return item;
    }

    public async Task<CategoryEntity> Select(string id)
    {
        return await context.categories.FirstOrDefaultAsync(c => c.code == id);
    }

    public async Task<bool> Delete(string id)
    {
        CategoryEntity category = await Select(id);
        if(category is null)
            return false;
        context.categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CategoryEntity>> SelectAll()
    {
        return await context.categories.ToListAsync();
    }

    public async Task<IEnumerable<CategoryEntity>> InsertMultiple(IEnumerable<CategoryEntity> items)
    {
        foreach(var category in items)
            context.categories.Add(category);
        await context.SaveChangesAsync();
        return items;
    }
}