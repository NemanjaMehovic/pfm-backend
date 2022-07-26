using CsvHelper.Configuration;
using pfm.Models;

namespace pfm.Mappings;

public class TransactionMapping : ClassMap<Transaction>
{
    public TransactionMapping()
    {
        Map(t => t.id).Name("id").Default(null);
        Map(t => t.beneficiary_name).Name("beneficiary-name").Default(null);
        Map(t => t.date).Name("date");
        Map(t => t.direction).Name("direction");
        Map(t => t.amount).Name("amount");
        Map(t => t.description).Name("description").Default(null);
        Map(t => t.currency).Name("currency").Default(null);
        Map(t => t.mcc).Name("mcc").Default(null);
        Map(t => t.kind).Name("kind");
    }
}