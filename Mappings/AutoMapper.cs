using AutoMapper;
using pfm.Database.Entities;
using pfm.Models;

namespace pfm.Mappings;

public class AutoMapper:Profile
{
    public AutoMapper()
    {
        CreateMap<Category, CategoryEntity>();

        CreateMap<CategoryEntity, Category>();

        CreateMap<Transaction, TransactionEntity>();

        CreateMap<TransactionEntity, Transaction>();
    }
}