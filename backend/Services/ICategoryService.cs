using pfm.Models;

namespace pfm.Services;

public interface ICategoryService
{
    public Task<string> InsertMultiple(IEnumerable<Category> categories);

    public Task<IEnumerable<Category>> SelectAll(string? parent_id);

    public Task<bool> DeleteAll();
}