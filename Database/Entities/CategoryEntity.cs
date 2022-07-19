namespace pfm.Database.Entities;

public class CategoryEntity
{
    public string code { get; set; }
    public string parent_code { get; set; }
    public string name { get; set; }
    public IEnumerable<TransactionEntity> transactions { get; set; }
    public IEnumerable<TransactionSplitsEntity> splits { get; set; }
}