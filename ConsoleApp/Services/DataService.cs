using System;
using System.Collections.Generic;
using System.IO;
using FinancialPlanner.Models;
using Newtonsoft.Json;

namespace FinancialPlanner.ConsoleApp.Services
{
    public class DataService
    {
        private readonly string _dataFolder;

        public DataService()
        {
            _dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FinancialPlanner");
            Directory.CreateDirectory(_dataFolder);
        }

        private T Load<T>(string fileName, T defaultValue) where T : class
        {
            var path = Path.Combine(_dataFolder, fileName);
            if (!File.Exists(path))
                return defaultValue;

            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json) ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private void Save<T>(string fileName, T data)
        {
            var path = Path.Combine(_dataFolder, fileName);
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        public List<Transaction> LoadTransactions() => Load("transactions.json", new List<Transaction>());
        public void SaveTransactions(List<Transaction> data) => Save("transactions.json", data);

        public List<DailyEntry> LoadDailyEntries() => Load("daily_entries.json", new List<DailyEntry>());
        public void SaveDailyEntries(List<DailyEntry> data) => Save("daily_entries.json", data);

        public List<string> LoadHabits() => Load("habits.json", new List<string>());
        public void SaveHabits(List<string> data) => Save("habits.json", data);

        public List<Budget> LoadBudgets() => Load("budgets.json", new List<Budget>());
        public void SaveBudgets(List<Budget> data) => Save("budgets.json", data);

        public List<string> LoadCategories()
        {
            var categories = Load("categories.json", new List<string>());
            if (categories.Count == 0)
            {
                categories = new List<string> { "Еда", "Транспорт", "Развлечения", "Здоровье", "Одежда", "Жилье", "Образование", "Прочее" };
            }
            return categories;
        }
        public void SaveCategories(List<string> data) => Save("categories.json", data);

        public LevelSystem LoadLevelSystem() => Load("level_system.json", new LevelSystem());
        public void SaveLevelSystem(LevelSystem data) => Save("level_system.json", data);

        public List<Achievement> LoadAchievements()
        {
            var achievements = Load("achievements.json", new List<Achievement>());
            if (achievements.Count == 0)
            {
                achievements = new List<Achievement>
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
            return achievements;
        }
        public void SaveAchievements(List<Achievement> data) => Save("achievements.json", data);

        public List<RecurringTransaction> LoadRecurringTransactions() => Load("recurring_transactions.json", new List<RecurringTransaction>());
        public void SaveRecurringTransactions(List<RecurringTransaction> data) => Save("recurring_transactions.json", data);

        public List<Project> LoadProjects() => Load("projects.json", new List<Project>());
        public void SaveProjects(List<Project> data) => Save("projects.json", data);
    }
}
