using pfm.Database.Entities;

namespace pfm.Models;

public class Transaction
{
    public string id {get; set;}
    public string beneficiary_name {get; set;}
    public DateTime date{get; set;}
    public TransactionDirection direction {get; set;}
    public double amount {get; set;}
    public string description {get; set;}
    public string currency {get; set;}
    public string mcc {get; set;}
    public TransactionKind kind {get; set;}
}