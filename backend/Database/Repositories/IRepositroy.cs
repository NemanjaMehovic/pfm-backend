using pfm.Database.Entities;

namespace pfm.Database.Repositories;

public interface IRepository<T,V>
{
    Task<T> Insert(T item);

    Task<T> Update(T item);

    Task<T> Select(V id);

    Task<bool> Delete(V id);

    Task<IEnumerable<T>> SelectAll();

    Task<IEnumerable<T>> InsertMultiple(IEnumerable<T> items);

    Task<IEnumerable<T>> UpdateMultiple(IEnumerable<T> items);

    Task<bool> DeleteMultiple(IEnumerable<T> items);

    PFMDbContext GetContext();
}