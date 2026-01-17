using System;
using System.Collections.Generic;
using System.Linq;

namespace FinancialPlanner.Models
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProjectStatus Status { get; set; } = ProjectStatus.Planned;
        public decimal Budget { get; set; }
        public decimal ReceivedAmount { get; set; } = 0;
        public decimal ExpectedIncome { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public DateTime? Deadline { get; set; }
        public string Client { get; set; } = string.Empty;
        public string Currency { get; set; } = "RUB";
        public int Priority { get; set; } = 3; // 1-5
        public List<ProjectMilestone> Milestones { get; set; } = new List<ProjectMilestone>();
        public List<ProjectExpense> Expenses { get; set; } = new List<ProjectExpense>();
        public string Notes { get; set; } = string.Empty;
        public decimal Profit => ReceivedAmount - TotalExpenses;
        public decimal TotalExpenses => Expenses.Sum(e => e.Amount);
        public decimal CompletionPercentage => Budget > 0 ? (ReceivedAmount / Budget) * 100 : 0;
    }

    public class ProjectMilestone
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PaymentAmount { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int XP { get; set; } = 50; // XP за выполнение этапа
    }

    public class ProjectExpense
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Category { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }

    public enum ProjectStatus
    {
        Planned,        // Запланирован
        InProgress,     // В работе
        OnHold,         // Приостановлен
        Completed,      // Завершен
        Cancelled       // Отменен
    }
}
