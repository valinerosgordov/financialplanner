namespace NexusFinance.Models;

public class Investment
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Stock"; // Stock, Crypto, Bond, ETF
    public decimal Amount { get; set; }
    public decimal CurrentValue { get; set; }
    public DateTime PurchaseDate { get; set; } = DateTime.Now;
    
    public decimal Return => CurrentValue - Amount;
    public decimal ReturnPercent => Amount > 0 ? Math.Round(Return / Amount * 100, 2) : 0;
}
