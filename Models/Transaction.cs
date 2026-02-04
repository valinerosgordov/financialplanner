namespace NexusFinance.Models;

public class Transaction
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Category { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public bool IsIncome { get; set; }
}
