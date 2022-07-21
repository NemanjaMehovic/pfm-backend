using System.Globalization;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using pfm.Database.Entities;
using pfm.Database.Repositories;
using pfm.Models;

namespace pfm.Services;

public class TransactionService : ITransactionService
{

    private static List<MCC> MCCs;
    private IMapper mapper;
    private IRepository<TransactionEntity, string> repository;

    public TransactionService(IRepository<TransactionEntity, string> repository, IMapper mapper)
    {
        this.mapper = mapper;
        this.repository = repository;
    }

    static TransactionService()
    {
        using (var reader = new StreamReader("mmc_codes.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<MCCMap>();
            MCCs = csv.GetRecords<MCC>().ToList();
        }
    }

    private class MCCMap : ClassMap<MCC>
    {
        public MCCMap()
        {
            Map(m => m.code).Name("code").Default(null);
            Map(m => m.merchant_type).Name("merchant-type").Default(null);
        }
    }

    private class MCC
    {
        public string? code { get; set; }
        public string? merchant_type { get; set; }
    }

    public async Task<string> InsertMultiple(IEnumerable<Transaction> transactions)
    {
        List<Transaction> items = new List<Transaction>();
        foreach(var item in transactions)
        {
            if(!MCCs.Any(m => item.mcc is null || m.code == item.mcc))
                return "MCC with code " + item.mcc + " doesn't exist";
            if(items.Any(t => t.id == item.id))
                return "Transaction with id " + item.id + " already exists";
            else
                items.Add(item);
        }
        var listTransactionEntity = await repository.SelectAll();
        foreach(var item in transactions)
            if(listTransactionEntity.Any(t => t.id == item.id))
                return "Transaction with id " + item.id + " already exists";
        listTransactionEntity = mapper.Map<IEnumerable<Transaction>, IEnumerable<TransactionEntity>>(transactions);
        await repository.InsertMultiple(listTransactionEntity);
        return null;
    }
}