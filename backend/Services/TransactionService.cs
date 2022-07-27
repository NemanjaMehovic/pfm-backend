using System.Globalization;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using pfm.Database.Entities;
using pfm.Database.Repositories;
using pfm.Models;
using pfm.Controllers;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace pfm.Services;

public class TransactionService : ITransactionService
{

    private static List<MCC> MCCs;
    private static List<Rule> Rules;
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
        using (StreamReader r = new StreamReader("rules.json"))
        {
            string json = r.ReadToEnd();
            Rules = JsonSerializer.Deserialize<List<Rule>>(json);
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
        try
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
        catch (Exception e)
        {
            return "Internal error";
        }
    }

    public async Task<string> Categorize(string transactionId, string categoryId)
    {
        try
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
        catch (Exception e)
        {
            return "Internal error";
        }
    }

    public async Task<string> Split(string transactionId, IEnumerable<Split> splits)
    {
        try
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
            foreach (var split in splits)
            {
                TransactionSplitsEntity tmp = new TransactionSplitsEntity();
                tmp.categoryId = split.catcode;
                tmp.transactionId = transactionId;
                tmp.amount = split.amount;
                splitsEntities.Add(tmp);
            }
            await repositorySplit.InsertMultiple(splitsEntities);
            transaction.categoryId = "Z";
            await repositoryTransaction.Update(transaction);
            return null;
        }
        catch (Exception e)
        {
            return "Internal error";
        }
    }

    public async Task<TransactionsList> GetTransactions(TransactionKind? kind, DateTime? startTime, DateTime? endTime, string? sortBy, uint page, uint pageSize, SortOrderC order)
    {
        try
        {
            IEnumerable<TransactionEntity> transactions = await repositoryTransaction.SelectAll();
            if (kind is not null)
                transactions = transactions.Where(t => t.kind == kind);
            if (startTime is not null)
                transactions = transactions.Where(t => DateTime.Compare(t.date, (DateTime)startTime) >= 0);
            if (endTime is not null)
                transactions = transactions.Where(t => DateTime.Compare(t.date, (DateTime)endTime) <= 0);
            if (sortBy is not null)
            {
                switch (sortBy)
                {
                    case "id":
                        transactions = order == SortOrderC.asc ? transactions.OrderBy(t => t.id) : transactions.OrderByDescending(t => t.id);
                        break;
                    case "beneficiary-name":
                        transactions = order == SortOrderC.asc ? transactions.OrderBy(t => t.beneficiary_name) : transactions.OrderByDescending(t => t.beneficiary_name);
                        break;
                    case "date":
                        transactions = order == SortOrderC.asc ? transactions.OrderBy(t => t.date) : transactions.OrderByDescending(t => t.date);
                        break;
                    case "direction":
                        transactions = order == SortOrderC.asc ? transactions.OrderBy(t => t.direction) : transactions.OrderByDescending(t => t.direction);
                        break;
                    case "amount":
                        transactions = order == SortOrderC.asc ? transactions.OrderBy(t => t.amount) : transactions.OrderByDescending(t => t.amount);
                        break;
                    case "description":
                        transactions = order == SortOrderC.asc ? transactions.OrderBy(t => t.description) : transactions.OrderByDescending(t => t.description);
                        break;
                    case "currency":
                        transactions = order == SortOrderC.asc ? transactions.OrderBy(t => t.currency) : transactions.OrderByDescending(t => t.currency);
                        break;
                    case "mcc":
                        transactions = order == SortOrderC.asc ? transactions.OrderBy(t => t.mcc) : transactions.OrderByDescending(t => t.mcc);
                        break;
                    case "kind":
                        transactions = order == SortOrderC.asc ? transactions.OrderBy(t => t.kind) : transactions.OrderByDescending(t => t.kind);
                        break;
                    default:
                        return null;
                }
            }
            var totalPages = (int)Math.Ceiling(transactions.Count() * 1.0 / pageSize);
            if (page > totalPages && totalPages != 0)
                return null;
            transactions = transactions.Skip(((int)page - 1) * (int)pageSize).Take((int)pageSize);
            List<TransactionsToSendBack> list = new List<TransactionsToSendBack>();
            foreach (var t in transactions)
            {
                await repositoryTransaction.GetContext().Entry(t).Collection(t => t.splits).LoadAsync();
                list.Add(mapper.Map<TransactionEntity, TransactionsToSendBack>(t));
            }

            return new TransactionsList
            {
                page_size = pageSize,
                page = page,
                total_count = totalPages,
                sort_by = sortBy,
                sort_order = order,
                items = list
            };
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<Analytics> GetAnalytics(TransactionDirection? direction, DateTime? startTime, DateTime? endTime, string? catcode)
    {
        try
        {
            var transactions = await repositoryTransaction.SelectAll();
            var categories = await repositoryCategory.SelectAll();
            if (startTime is not null)
                transactions = transactions.Where(t => DateTime.Compare(t.date, (DateTime)startTime) >= 0);
            if (endTime is not null)
                transactions = transactions.Where(t => DateTime.Compare(t.date, (DateTime)endTime) <= 0);
            if (direction is not null)
                transactions = transactions.Where(t => t.direction == direction);
            if (catcode is not null)
                categories = categories.Where(c => c.code == catcode);

            Dictionary<string, CategoriesAnalytics> map = new Dictionary<string, CategoriesAnalytics>();
            foreach (var c in categories)
                map[c.code] = new CategoriesAnalytics
                {
                    catcode = c.code,
                    amount = 0,
                    count = 0
                };

            foreach (var t in transactions)
            {
                if (t.categoryId is null)
                    continue;
                bool flag;
                if (t.categoryId == "Z")
                {
                    await repositoryTransaction.GetContext().Entry(t).Collection(t => t.splits).LoadAsync();
                    foreach (var split in t.splits)
                    {
                        await repositorySplit.GetContext().Entry(split).Reference(s => s.category).LoadAsync();
                        flag = await FillAnalytics(map, split.amount, split.category);
                        if (!flag)
                            throw new Exception();
                    }
                }
                await repositoryTransaction.GetContext().Entry(t).Reference(t => t.category).LoadAsync();
                flag = await FillAnalytics(map, t.amount, t.category);
                if (!flag)
                    throw new Exception();
            }
            List<CategoriesAnalytics> categoriesAnalytics = new List<CategoriesAnalytics>();
            foreach (var pair in map)
                categoriesAnalytics.Add(pair.Value);
            return new Analytics
            {
                groups = categoriesAnalytics
            };
        }
        catch (Exception e)
        {
            return null;
        }
    }

    private async Task<bool> FillAnalytics(Dictionary<string, CategoriesAnalytics> map, double amount, CategoryEntity category)
    {
        try
        {
            CategoryEntity tmpCat = category;
            while (tmpCat != null)
            {
                CategoriesAnalytics tmpAnaly = null;
                if (map.TryGetValue(tmpCat.code, out tmpAnaly))
                {
                    tmpAnaly.count++;
                    tmpAnaly.amount += amount;
                }
                if (tmpCat.parent_code is not null)
                {
                    await repositoryCategory.GetContext().Entry(tmpCat).Reference(c => c.parentCategory).LoadAsync();
                    tmpCat = tmpCat.parentCategory;
                }
                else
                    tmpCat = null;
            }
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAll()
    {
        try
        {
            var tmp = await repositoryTransaction.SelectAll();
            await repositoryTransaction.DeleteMultiple(tmp);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public async Task<bool> AutoCategorize()
    {
        try
        {
            var tmpList = await repositoryCategory.SelectAll();
            foreach (var rule in Rules)
            {
                if (!tmpList.Any(c => c.code == rule.catcode))
                    return false;
            }
            foreach (var rule in Rules)
            {
                var list = await repositoryTransaction.GetContext().transactions.FromSqlRaw("Select * from transactions where transactions.\"categoryId\" is null and ( " + rule.predicate + " )").ToListAsync();
                foreach (var transaction in list)
                    transaction.categoryId = rule.catcode;
                if (list.Count() > 0)
                    await repositoryTransaction.UpdateMultiple(list);
            }
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}