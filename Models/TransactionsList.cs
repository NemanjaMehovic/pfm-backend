using System.Text.Json.Serialization;

namespace pfm.Models;

public class TransactionsList
{
    [JsonPropertyName("page-size")]
    public uint page_size { get; set; }
    public uint page { get; set; }
    [JsonPropertyName("total-count")]
    public int total_count { get; set; }
    [JsonPropertyName("sort-by")]
    public string? sort_by { get; set; }
    [JsonPropertyName("sort-order")]
    public SortOrderC sort_order { get; set; }
    public IEnumerable<TransactionsToSendBack> items { get; set; }
}