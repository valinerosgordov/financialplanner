namespace NexusFinance.Models;

public class Category
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Expense"; // Income, Expense
    public string Icon { get; set; } = "💰";
}
