namespace NexusFinance.Models;

/// <summary>
/// Represents a team member assigned to a project
/// </summary>
public class TeamMember
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public PaymentFrequency PaymentFrequency { get; set; } = PaymentFrequency.Monthly;
    public bool IsActive { get; set; } = true;
    public string ProjectId { get; set; } = string.Empty;
    public DateTime JoinedDate { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Calculated monthly cost regardless of payment frequency
    /// </summary>
    public decimal MonthlyCost
    {
        get
        {
            return PaymentFrequency switch
            {
                PaymentFrequency.Monthly => Salary,
                PaymentFrequency.Hourly => Salary * 160, // Assuming 160 hours/month
                PaymentFrequency.OneTime => 0, // One-time payments don't count toward monthly burn
                _ => Salary
            };
        }
    }
    
    /// <summary>
    /// Status display text
    /// </summary>
    public string StatusDisplay => IsActive ? "Active" : "Inactive";
    
    /// <summary>
    /// Status color
    /// </summary>
    public string StatusColor => IsActive ? "#00C853" : "#606060";
}

/// <summary>
/// Payment frequency types
/// </summary>
public enum PaymentFrequency
{
    Monthly,
    Hourly,
    OneTime
}
