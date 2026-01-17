using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FinancialPlanner.Models;
using Newtonsoft.Json;

namespace FinancialPlanner.Services
{
    public class DataService
    {
        private readonly string _dataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FinancialPlanner");

        private readonly string _transactionsFile;
        private readonly string _dailyEntriesFile;
        private readonly string _habitsFile;
        private readonly string _budgetsFile;
        private readonly string _categoriesFile;
        private readonly string _levelSystemFile;
        private readonly string _achievementsFile;
        private readonly string _recurringTransactionsFile;

        public DataService()
        {
            Directory.CreateDirectory(_dataFolder);
            _transactionsFile = Path.Combine(_dataFolder, "transactions.json");
            _dailyEntriesFile = Path.Combine(_dataFolder, "daily_entries.json");
            _habitsFile = Path.Combine(_dataFolder, "habits.json");
            _budgetsFile = Path.Combine(_dataFolder, "budgets.json");
            _categoriesFile = Path.Combine(_dataFolder, "categories.json");
            _levelSystemFile = Path.Combine(_dataFolder, "level_system.json");
            _achievementsFile = Path.Combine(_dataFolder, "achievements.json");
            _recurringTransactionsFile = Path.Combine(_dataFolder, "recurring_transactions.json");
        }

        public List<Transaction> LoadTransactions()
        {
            if (!File.Exists(_transactionsFile))
                return new List<Transaction>();

            var json = File.ReadAllText(_transactionsFile);
            return JsonConvert.DeserializeObject<List<Transaction>>(json) ?? new List<Transaction>();
        }

        public void SaveTransactions(List<Transaction> transactions)
        {
            var json = JsonConvert.SerializeObject(transactions, Formatting.Indented);
            File.WriteAllText(_transactionsFile, json);
        }

        public List<DailyEntry> LoadDailyEntries()
        {
            if (!File.Exists(_dailyEntriesFile))
                return new List<DailyEntry>();

            var json = File.ReadAllText(_dailyEntriesFile);
            return JsonConvert.DeserializeObject<List<DailyEntry>>(json) ?? new List<DailyEntry>();
        }

        public void SaveDailyEntries(List<DailyEntry> entries)
        {
            var json = JsonConvert.SerializeObject(entries, Formatting.Indented);
            File.WriteAllText(_dailyEntriesFile, json);
        }

        public List<string> LoadHabits()
        {
            if (!File.Exists(_habitsFile))
                return new List<string>();

            var json = File.ReadAllText(_habitsFile);
            return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
        }

        public void SaveHabits(List<string> habits)
        {
            var json = JsonConvert.SerializeObject(habits, Formatting.Indented);
            File.WriteAllText(_habitsFile, json);
        }

        public List<Budget> LoadBudgets()
        {
            if (!File.Exists(_budgetsFile))
                return new List<Budget>();

            var json = File.ReadAllText(_budgetsFile);
            return JsonConvert.DeserializeObject<List<Budget>>(json) ?? new List<Budget>();
        }

        public void SaveBudgets(List<Budget> budgets)
        {
            var json = JsonConvert.SerializeObject(budgets, Formatting.Indented);
            File.WriteAllText(_budgetsFile, json);
        }

        public List<string> LoadCategories()
        {
            if (!File.Exists(_categoriesFile))
                return new List<string> { "Еда", "Транспорт", "Развлечения", "Здоровье", "Одежда", "Жилье", "Образование", "Прочее" };

            var json = File.ReadAllText(_categoriesFile);
            return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
        }

        public void SaveCategories(List<string> categories)
        {
            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);
            File.WriteAllText(_categoriesFile, json);
        }

        public LevelSystem LoadLevelSystem()
        {
            if (!File.Exists(_levelSystemFile))
                return new LevelSystem();

            var json = File.ReadAllText(_levelSystemFile);
            return JsonConvert.DeserializeObject<LevelSystem>(json) ?? new LevelSystem();
        }

        public void SaveLevelSystem(LevelSystem levelSystem)
        {
            var json = JsonConvert.SerializeObject(levelSystem, Formatting.Indented);
            File.WriteAllText(_levelSystemFile, json);
        }

        public List<Achievement> LoadAchievements()
        {
            if (!File.Exists(_achievementsFile))
                return InitializeDefaultAchievements();

            var json = File.ReadAllText(_achievementsFile);
            return JsonConvert.DeserializeObject<List<Achievement>>(json) ?? InitializeDefaultAchievements();
        }

        public void SaveAchievements(List<Achievement> achievements)
        {
            var json = JsonConvert.SerializeObject(achievements, Formatting.Indented);
            File.WriteAllText(_achievementsFile, json);
        }

        private List<Achievement> InitializeDefaultAchievements()
        {
            return new List<Achievement>
            {
                new Achievement { Title = "Новичок", Description = "Достигните 5 уровня", Icon = "🌟", Type = AchievementType.Level, TargetValue = 5 },
                new Achievement { Title = "Опытный", Description = "Достигните 10 уровня", Icon = "⭐", Type = AchievementType.Level, TargetValue = 10 },
                new Achievement { Title = "Мастер", Description = "Достигните 20 уровня", Icon = "💫", Type = AchievementType.Level, TargetValue = 20 },
                new Achievement { Title = "Трудолюбивый", Description = "Выполните 10 задач", Icon = "✅", Type = AchievementType.TasksCompleted, TargetValue = 10 },
                new Achievement { Title = "Неутомимый", Description = "Выполните 50 задач", Icon = "🔥", Type = AchievementType.TasksCompleted, TargetValue = 50 },
                new Achievement { Title = "Привычка", Description = "Выполняйте привычку 7 дней подряд", Icon = "📅", Type = AchievementType.HabitsStreak, TargetValue = 7 },
                new Achievement { Title = "Финансист", Description = "Добавьте 20 транзакций", Icon = "💰", Type = AchievementType.TransactionsCount, TargetValue = 20 },
                new Achievement { Title = "Экономист", Description = "Сэкономьте 10000 в бюджете", Icon = "💎", Type = AchievementType.BudgetSaved, TargetValue = 10000 },
                new Achievement { Title = "Активный", Description = "Используйте приложение 30 дней", Icon = "📊", Type = AchievementType.DaysActive, TargetValue = 30 }
            };
        }

        public List<RecurringTransaction> LoadRecurringTransactions()
        {
            if (!File.Exists(_recurringTransactionsFile))
                return new List<RecurringTransaction>();

            var json = File.ReadAllText(_recurringTransactionsFile);
            return JsonConvert.DeserializeObject<List<RecurringTransaction>>(json) ?? new List<RecurringTransaction>();
        }

        public void SaveRecurringTransactions(List<RecurringTransaction> transactions)
        {
            var json = JsonConvert.SerializeObject(transactions, Formatting.Indented);
            File.WriteAllText(_recurringTransactionsFile, json);
        }
    }
}
