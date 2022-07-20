using System.ComponentModel.DataAnnotations;
using pfm.Database.Entities;

namespace pfm.Models;

public class Transaction
{
    [Required]
    public string id { get; set; }
    public string beneficiary_name { get; set; }
    [Required]
    public DateTime date { get; set; }
    [Required]
    public TransactionDirection direction { get; set; }
    [Required]
    public double amount { get; set; }
    public string description { get; set; }
    [Required]
    public string currency { get; set; }
    public string mcc { get; set; }
    [Required]
    public TransactionKind kind { get; set; }
}