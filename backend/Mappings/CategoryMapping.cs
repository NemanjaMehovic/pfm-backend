using CsvHelper.Configuration;
using pfm.Models;

namespace pfm.Mappings;

public class CategoryMapping : ClassMap<Category>
{
    public CategoryMapping()
    {
        Map(c => c.code).Name("code").Default(null);
        Map(c => c.parent_code).Name("parent-code").Default(null);
        Map(c => c.name).Name("name").Default(null);
    }
}