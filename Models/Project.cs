namespace NexusFinance.Models;

public class Project
{
    public string Name { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public decimal Cost { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public decimal Profit => Revenue - Cost;
    public decimal ProfitMargin => Revenue > 0 ? Math.Round(Profit / Revenue * 100, 1) : 0;
}
