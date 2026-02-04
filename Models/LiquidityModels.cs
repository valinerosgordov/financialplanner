namespace NexusFinance.Models;

/// <summary>
/// Represents money the user owes to others (liability)
/// </summary>
public class Payable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CreditorName { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Computed urgency level based on due date
    /// </summary>
    public string UrgencyLevel
    {
        get
        {
            if (IsPaid) return "Paid";
            
            var daysUntilDue = (DueDate - DateTime.Now).Days;
            
            if (daysUntilDue < 0) return "Overdue";
            if (daysUntilDue <= 3) return "Critical";
            if (daysUntilDue <= 7) return "High";
            if (daysUntilDue <= 14) return "Medium";
            return "Low";
        }
    }
    
    /// <summary>
    /// Color for urgency indicator (Monochrome + functional red)
    /// </summary>
    public string UrgencyColor
    {
        get
        {
            return UrgencyLevel switch
            {
                "Overdue" => "#D50000",
                "Critical" => "#FF5252",
                "High" => "#909090",
                "Medium" => "#606060",
                "Low" => "#404040",
                "Paid" => "#00C853",
                _ => "#808080"
            };
        }
    }
}

/// <summary>
/// Represents money owed to the user (asset)
/// </summary>
public class Receivable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string DebtorName { get; set; } = string.Empty;
    public string? ProjectId { get; set; }
    public DateTime ExpectedDate { get; set; }
    public ProbabilityLevel Probability { get; set; } = ProbabilityLevel.Likely;
    public bool IsReceived { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Adjusted amount based on probability
    /// </summary>
    public decimal WeightedAmount
    {
        get
        {
            if (IsReceived) return Amount;
            
            return Probability switch
            {
                ProbabilityLevel.Confirmed => Amount,
                ProbabilityLevel.Likely => Amount * 0.75m,
                ProbabilityLevel.Uncertain => Amount * 0.4m,
                _ => Amount * 0.5m
            };
        }
    }
    
    /// <summary>
    /// Color for probability indicator
    /// </summary>
    public string ProbabilityColor
    {
        get
        {
            if (IsReceived) return "#00C853";
            
            return Probability switch
            {
                ProbabilityLevel.Confirmed => "#C0C0C0",
                ProbabilityLevel.Likely => "#808080",
                ProbabilityLevel.Uncertain => "#505050",
                _ => "#404040"
            };
        }
    }
    
    /// <summary>
    /// Display text for probability
    /// </summary>
    public string ProbabilityText
    {
        get
        {
            if (IsReceived) return "Received";
            return Probability.ToString();
        }
    }
}

/// <summary>
/// Probability levels for receivables
/// </summary>
public enum ProbabilityLevel
{
    Confirmed,   // 100% - contract signed, work delivered
    Likely,      // 75% - verbal agreement
    Uncertain    // 40% - speculative, proposal stage
}
