using System;

namespace FinancialPlanner.Models
{
    public class Achievement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "🏆";
        public bool IsUnlocked { get; set; }
        public DateTime? UnlockedDate { get; set; }
        public AchievementType Type { get; set; }
        public int TargetValue { get; set; }
        public int CurrentValue { get; set; }
    }

    public enum AchievementType
    {
        Level,
        TasksCompleted,
        HabitsStreak,
        TransactionsCount,
        BudgetSaved,
        DaysActive
    }
}
