using System.Text.Json.Serialization;
using pfm.Database.Entities;

namespace pfm.Models;

public class TransactionsToSendBack : Transaction
{
    public string? catcode { get; set; }
    public IEnumerable<Split> splits { get; set; }
}