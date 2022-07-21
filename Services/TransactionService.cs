using System.Globalization;
using System.Linq.Expressions;
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
    private IRepository<TransactionEntity, string> repositoryTransaction;
    private IRepository<CategoryEntity, string> repositoryCategory;
    private IRepository<TransactionSplitsEntity, Tuple<string, string>> repositorySplit;

    public TransactionService(IRepository<TransactionEntity, string> repositoryTransaction, IRepository<CategoryEntity, string> repositoryCategory, IRepository<TransactionSplitsEntity, Tuple<string, string>> repositorySplit, IMapper mapper)
    {
        this.mapper = mapper;
        this.repositoryTransaction = repositoryTransaction;
        this.repositoryCategory = repositoryCategory;
        this.repositorySplit = repositorySplit;
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
        foreach (var item in transactions)
        {
            if (!MCCs.Any(m => item.mcc is null || m.code == item.mcc))
                return "MCC with code " + item.mcc + " doesn't exist";
            if (items.Any(t => t.id == item.id))
                return "Transaction with id " + item.id + " already exists";
            else
                items.Add(item);
        }
        var listTransactionEntity = await repositoryTransaction.SelectAll();
        foreach (var item in transactions)
            if (listTransactionEntity.Any(t => t.id == item.id))
                return "Transaction with id " + item.id + " already exists";
        listTransactionEntity = mapper.Map<IEnumerable<Transaction>, IEnumerable<TransactionEntity>>(transactions);
        await repositoryTransaction.InsertMultiple(listTransactionEntity);
        return null;
    }

    public async Task<string> Categorize(string transactionId, string categoryId)
    {
        var transaction = await repositoryTransaction.Select(transactionId);
        if (transaction is null)
            return "Transaction with id " + transactionId + " does not exist";
        var category = await repositoryCategory.Select(categoryId);
        if (category is null)
            return "Category with id " + categoryId + " does not exist";
        transaction.categoryId = category.code;
        await repositoryTransaction.Update(transaction);
        return null;
    }

    public async Task<string> Split(string transactionId, IEnumerable<Split> splits)
    {
        var transaction = await repositoryTransaction.Select(transactionId);
        if (transaction is null)
            return "Transaction with id " + transactionId + " does not exist";
        var amount = transaction.amount;
        foreach (var split in splits)
            amount -= split.amount;
        if (amount > 0.0000001 || amount < -0.0000001)
            return "Transaction amount and splits amount are not the same";
        var categories = await repositoryCategory.SelectAll();
        foreach (var split in splits)
            if (!categories.Any(c => c.code == split.catcode))
                return "Category " + split.catcode + " doesn't exist";
        await repositoryTransaction.GetContext().Entry(transaction).Collection(t => t.splits).LoadAsync();
        if (transaction.splits.Count() > 0)
            await repositorySplit.DeleteMultiple(transaction.splits);
        List<TransactionSplitsEntity> splitsEntities = new List<TransactionSplitsEntity>();
        foreach(var split in splits)
        {
            TransactionSplitsEntity tmp = new TransactionSplitsEntity();
            tmp.categoryId = split.catcode;
            tmp.transactionId = transactionId;
            tmp.amount = split.amount;
            splitsEntities.Add(tmp);
        }
        await repositorySplit.InsertMultiple(splitsEntities);
        return null;
    }
}