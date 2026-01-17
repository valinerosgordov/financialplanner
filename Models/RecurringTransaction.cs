using System;

namespace FinancialPlanner.Models
{
    public class RecurringTransaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Currency { get; set; } = "RUB";
        public RecurrenceType Recurrence { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int DayOfMonth { get; set; } = 1; // Для месячных транзакций
    }

    public enum RecurrenceType
    {
        Daily,
        Weekly,
        Monthly,
        Yearly
    }
}
