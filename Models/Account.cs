namespace NexusFinance.Models;

public class Account
{
    public string Name { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Type { get; set; } = "Checking"; // Checking, Savings, Cash
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
