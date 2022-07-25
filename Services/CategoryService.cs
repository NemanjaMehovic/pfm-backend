using System.Text;
using AutoMapper;
using pfm.Database.Entities;
using pfm.Database.Repositories;
using pfm.Models;

namespace pfm.Services;

public class CategoryService : ICategoryService
{

    private IRepository<CategoryEntity, string> repository;
    private IMapper mapper;

    public CategoryService(IRepository<CategoryEntity, string> repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    public async Task<string> InsertMultiple(IEnumerable<Category> categories)
    {
        try
        {
            List<Category> items = new List<Category>();
            foreach (var item in categories)
            {
                if (items.Any(c => c.code == item.code))
                {
                    var tmp = items.First(c => c.code == item.code);
                    tmp.parent_code = item.parent_code;
                    tmp.name = item.name;
                }
                else
                    items.Add(item);
            }

            var listOfEntities = await repository.SelectAll();
            var checkList = items.Where(c => c.parent_code is null || items.Any(c2 => c2.code == c.parent_code) || listOfEntities.Any(ce => ce.code == c.parent_code));
            checkList = items.Except(checkList);
            if (checkList.Count() > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var tmp in checkList)
                    builder.Append(tmp.parent_code).Append(" can't be parent code because ").Append(tmp.parent_code).Append(" doesn't exist as a category").AppendLine();
                return builder.ToString();
            }

            var listToUpdate = listOfEntities.Where(ce => items.Any(c => c.code == ce.code));
            foreach (var item in listToUpdate)
            {
                var tmp = items.First(c => c.code == item.code);
                item.name = tmp.name;
                item.parent_code = tmp.parent_code;
                items = items.Where(c => c.code != tmp.code).ToList();
            }
            if (listToUpdate.Count() > 0)
                await repository.UpdateMultiple(listToUpdate);
            if (items.Count() > 0)
                await repository.InsertMultiple(mapper.Map<IEnumerable<Category>, IEnumerable<CategoryEntity>>(items));
            return null;
        }
        catch (Exception e)
        {
            return "Internal error";
        }
    }

    public async Task<IEnumerable<Category>> SelectAll(string? parent_id)
    {
        try
        {
            var listOfEntities = await repository.SelectAll();
            if (parent_id is not null)
                listOfEntities = listOfEntities.Where(c => c.parent_code == parent_id).AsEnumerable();
            var list = mapper.Map<IEnumerable<CategoryEntity>, IEnumerable<Category>>(listOfEntities);
            foreach (var item in list)
                item.parent_code = item.parent_code ?? "";
            return list;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAll()
    {
        try
        {
            var tmp = await repository.SelectAll();
            await repository.DeleteMultiple(tmp);
            return true;
        }
        catch(Exception e)
        {
            return false;
        }
    }
}