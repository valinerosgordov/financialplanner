using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinancialPlanner.ConsoleApp.Services;
using FinancialPlanner.ConsoleApp.UI;
using FinancialPlanner.Models;

namespace FinancialPlanner.ConsoleApp.Menus
{
    public class MainMenu
    {
        private readonly DataService _dataService;
        private readonly CurrencyService _currencyService;
        private readonly ConsoleRenderer _renderer;

        public MainMenu(DataService dataService, CurrencyService currencyService, ConsoleRenderer renderer)
        {
            _dataService = dataService;
            _currencyService = currencyService;
            _renderer = renderer;
        }

        public void Show()
        {
            var level = _dataService.LoadLevelSystem();
            var transactions = _dataService.LoadTransactions();
            
            _renderer.ShowLevel(level);
            
            var income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var expenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            var balance = income - expenses;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  💰 Доходы: {income:N2} ₽");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  💸 Расходы: {expenses:N2} ₽");
            Console.ForegroundColor = balance >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"  ⚖️  Баланс: {balance:N2} ₽");
            Console.ResetColor();

            _renderer.Menu(new[]
            {
                "💵 Управление транзакциями",
                "📅 Daily Tracker",
                "🚀 Планировщик проектов",
                "💰 Бюджеты",
                "💱 Конвертер валют",
                "📊 Расширенная аналитика",
                "📈 Статистика и достижения",
                "🔄 Повторяющиеся транзакции",
                "📥 Экспорт данных",
                "Выход"
            });
        }

        public string GetChoice() => Console.ReadLine() ?? "0";

        public async Task Handle(string choice)
        {
            switch (choice)
            {
                case "1": await Transactions(); break;
                case "2": await DailyTracker(); break;
                case "3": await Projects(); break;
                case "4": await Budgets(); break;
                case "5": await CurrencyConverter(); break;
                case "6": Analytics(); break;
                case "7": Statistics(); break;
                case "8": await Recurring(); break;
                case "9": ExportCsv(); break;
                default: _renderer.Warning("Неверный выбор!"); break;
            }
        }

        private async Task Transactions()
        {
            _renderer.Clear();
            _renderer.Header("💵 УПРАВЛЕНИЕ ТРАНЗАКЦИЯМИ");

            var transactions = _dataService.LoadTransactions();
            
            if (transactions.Any())
            {
                _renderer.Write("\nПоследние транзакции:\n", ConsoleColor.Cyan);
                foreach (var t in transactions.OrderByDescending(x => x.Date).Take(10))
                    _renderer.ShowTransaction(t);
            }
            else
            {
                _renderer.Write("Транзакций пока нет.", ConsoleColor.Gray);
            }

            _renderer.Menu(new[] { "Добавить транзакцию", "Удалить транзакцию", "Поиск транзакций", "Назад" });
            var choice = _renderer.Read("\nВыберите опцию: ");
            
            switch (choice)
            {
                case "1": await AddTransaction(); break;
                case "2": DeleteTransaction(transactions); break;
                case "3": SearchTransactions(transactions); break;
            }
        }

        private async Task AddTransaction()
        {
            _renderer.Clear();
            _renderer.Header("➕ НОВАЯ ТРАНЗАКЦИЯ");

            var type = _renderer.Read("Тип (1-Доход, 2-Расход): ") == "1" ? TransactionType.Income : TransactionType.Expense;
            var desc = _renderer.Read("Описание: ");
            var amount = _renderer.ReadDecimal("Сумма: ");
            
            var categories = _dataService.LoadCategories();
            _renderer.Write("\nКатегории:", ConsoleColor.Cyan);
            for (int i = 0; i < categories.Count; i++)
                Console.WriteLine($"{i + 1}. {categories[i]}");
            
            var catIndex = _renderer.ReadInt("Категория: ", 1, categories.Count) - 1;

            var currencies = _currencyService.GetAvailableCurrencies();
            _renderer.Write("\nВалюты:", ConsoleColor.Cyan);
            for (int i = 0; i < currencies.Count; i++)
                Console.WriteLine($"{i + 1}. {currencies[i].Code} - {currencies[i].Name}");
            
            var currIndex = _renderer.ReadInt("Валюта: ", 1, currencies.Count) - 1;

            var transactions = _dataService.LoadTransactions();
            transactions.Add(new Transaction
            {
                Description = desc,
                Amount = amount,
                Type = type,
                Category = categories[catIndex],
                Currency = currencies[currIndex].Code,
                Date = DateTime.Now
            });
            _dataService.SaveTransactions(transactions);
            _renderer.Success("Транзакция добавлена!");
        }

        private void DeleteTransaction(List<Transaction> transactions)
        {
            if (!transactions.Any())
            {
                _renderer.Warning("Нет транзакций для удаления.");
                return;
            }

            _renderer.Write("\nВыберите транзакцию для удаления:", ConsoleColor.Cyan);
            for (int i = 0; i < Math.Min(transactions.Count, 20); i++)
            {
                _renderer.ShowTransaction(transactions.OrderByDescending(t => t.Date).ElementAt(i), i + 1);
            }

            var index = _renderer.ReadInt("Номер транзакции: ", 1, Math.Min(transactions.Count, 20)) - 1;
            var transactionToDelete = transactions.OrderByDescending(t => t.Date).ElementAt(index);
            transactions.Remove(transactionToDelete);
            _dataService.SaveTransactions(transactions);
            _renderer.Success("Транзакция удалена!");
        }

        private void SearchTransactions(List<Transaction> transactions)
        {
            var searchText = _renderer.Read("Введите текст для поиска: ");
            var filtered = transactions.Where(t => 
                t.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                t.Category.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

            if (filtered.Any())
            {
                _renderer.Write($"\nНайдено {filtered.Count} транзакций:\n", ConsoleColor.Cyan);
                foreach (var transaction in filtered.OrderByDescending(t => t.Date))
                {
                    _renderer.ShowTransaction(transaction);
                }
            }
            else
            {
                _renderer.Warning("Транзакции не найдены.");
            }
        }

        private async Task DailyTracker()
        {
            _renderer.Clear();
            _renderer.Header("📅 DAILY TRACKER");

            var entries = _dataService.LoadDailyEntries();
            var today = DateTime.Today;
            var todayEntry = entries.FirstOrDefault(e => e.Date.Date == today);

            if (todayEntry != null)
            {
                _renderer.ShowDailyEntry(todayEntry);
            }
            else
            {
                _renderer.Write("Запись на сегодня еще не создана.", ConsoleColor.Gray);
            }

            _renderer.Menu(new[]
            {
                "Создать/Обновить запись на сегодня",
                "Добавить задачу",
                "Отметить задачу выполненной",
                "Добавить привычку",
                "Отметить привычку",
                "Установить настроение",
                "Назад"
            });

            var choice = _renderer.Read("\nВыберите опцию: ");
            
            switch (choice)
            {
                case "1":
                    await CreateOrUpdateTodayEntryAsync(entries, today);
                    break;
                case "2":
                    await AddTaskAsync(entries, today);
                    break;
                case "3":
                    await ToggleTaskAsync(entries, today);
                    break;
                case "4":
                    AddHabit(entries, today);
                    break;
                case "5":
                    await ToggleHabitAsync(entries, today);
                    break;
                case "6":
                    SetMood(entries, today);
                    break;
            }
        }

        private async Task CreateOrUpdateTodayEntryAsync(List<DailyEntry> entries, DateTime today)
        {
            var entry = entries.FirstOrDefault(e => e.Date.Date == today);
            if (entry == null)
            {
                entry = new DailyEntry { Date = today };
                entries.Add(entry);
            }

            var notes = _renderer.Read("Заметки (Enter для пропуска): ");
            if (!string.IsNullOrEmpty(notes))
            {
                entry.Notes = notes;
            }

            _dataService.SaveDailyEntries(entries);
            _renderer.Success("Запись сохранена!");
        }

        private async Task AddTaskAsync(List<DailyEntry> entries, DateTime today)
        {
            var entry = entries.FirstOrDefault(e => e.Date.Date == today);
            if (entry == null)
            {
                entry = new DailyEntry { Date = today };
                entries.Add(entry);
            }

            var description = _renderer.Read("Описание задачи: ");
            var priority = _renderer.ReadInt("Приоритет (1-5): ", 1, 5);

            var task = new TaskItem
            {
                Description = description,
                Priority = priority,
                XP = LevelCalculator.GetXPForTask(priority)
            };

            entry.Tasks.Add(task);
            _dataService.SaveDailyEntries(entries);
            _renderer.Success($"Задача добавлена! (+{task.XP} XP)");
        }

        private async Task ToggleTaskAsync(List<DailyEntry> entries, DateTime today)
        {
            var entry = entries.FirstOrDefault(e => e.Date.Date == today);
            if (entry == null || !entry.Tasks.Any())
            {
                _renderer.Warning("Нет задач для отметки.");
                return;
            }

            _renderer.Write("\nЗадачи:", ConsoleColor.Cyan);
            for (int i = 0; i < entry.Tasks.Count; i++)
            {
                var task = entry.Tasks[i];
                var status = task.IsCompleted ? "✓" : "○";
                Console.WriteLine($"{i + 1}. {status} {task.Description}");
            }

            var index = _renderer.ReadInt("Номер задачи: ", 1, entry.Tasks.Count) - 1;
            var taskToToggle = entry.Tasks[index];
            taskToToggle.IsCompleted = !taskToToggle.IsCompleted;

            if (taskToToggle.IsCompleted && !taskToToggle.XPClaimed)
            {
                await AddXPAsync(taskToToggle.XP, $"Задача: {taskToToggle.Description}");
                taskToToggle.XPClaimed = true;
            }

            _dataService.SaveDailyEntries(entries);
            _renderer.Success("Задача обновлена!");
        }

        private void AddHabit(List<DailyEntry> entries, DateTime today)
        {
            var habits = _dataService.LoadHabits();
            var habitName = _renderer.Read("Название привычки: ");
            
            if (!habits.Contains(habitName))
            {
                habits.Add(habitName);
                _dataService.SaveHabits(habits);
            }

            var entry = entries.FirstOrDefault(e => e.Date.Date == today);
            if (entry == null)
            {
                entry = new DailyEntry { Date = today };
                entries.Add(entry);
            }

            if (!entry.Habits.Any(h => h.HabitName == habitName))
            {
                entry.Habits.Add(new HabitCheck { HabitName = habitName });
            }

            _dataService.SaveDailyEntries(entries);
            _renderer.Success("Привычка добавлена!");
        }

        private async Task ToggleHabitAsync(List<DailyEntry> entries, DateTime today)
        {
            var entry = entries.FirstOrDefault(e => e.Date.Date == today);
            if (entry == null || !entry.Habits.Any())
            {
                _renderer.Warning("Нет привычек для отметки.");
                return;
            }

            _renderer.Write("\nПривычки:", ConsoleColor.Cyan);
            for (int i = 0; i < entry.Habits.Count; i++)
            {
                var habit = entry.Habits[i];
                var status = habit.IsCompleted ? "✓" : "○";
                Console.WriteLine($"{i + 1}. {status} {habit.HabitName}");
            }

            var index = _renderer.ReadInt("Номер привычки: ", 1, entry.Habits.Count) - 1;
            var habitToToggle = entry.Habits[index];
            var wasCompleted = habitToToggle.IsCompleted;
            habitToToggle.IsCompleted = !habitToToggle.IsCompleted;

            if (habitToToggle.IsCompleted && !wasCompleted)
            {
                var xp = LevelCalculator.GetXPForHabit();
                await AddXPAsync(xp, $"Привычка: {habitToToggle.HabitName}");
            }

            _dataService.SaveDailyEntries(entries);
            _renderer.Success("Привычка обновлена!");
        }

        private void SetMood(List<DailyEntry> entries, DateTime today)
        {
            var entry = entries.FirstOrDefault(e => e.Date.Date == today);
            if (entry == null)
            {
                entry = new DailyEntry { Date = today };
                entries.Add(entry);
            }

            var mood = _renderer.ReadInt("Настроение (1-10): ", 1, 10);
            entry.Mood = mood;
            _dataService.SaveDailyEntries(entries);
            _renderer.Success($"Настроение установлено: {mood}/10");
        }

        private async Task AddXPAsync(int xp, string source)
        {
            var levelSystem = _dataService.LoadLevelSystem();
            var oldLevel = levelSystem.Level;
            
            levelSystem.TotalXP += xp;
            var newLevel = LevelCalculator.CalculateLevel(levelSystem.TotalXP);
            levelSystem.Level = newLevel;

            var (currentXP, xpToNext) = LevelCalculator.GetLevelProgress(levelSystem.TotalXP, levelSystem.Level);
            levelSystem.CurrentLevelXP = currentXP;
            levelSystem.XPToNextLevel = xpToNext;

            if (newLevel > oldLevel)
            {
                _renderer.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(@"
╔══════════════════════════════════════════════════════════════╗
║                                                              ║
║                    ⚡ LEVEL UP! ⚡                          ║
║                                                              ║
║              Уровень {0} → {1}                              ║
║                                                              ║
║              +{2} XP за {3}                                 ║
║                                                              ║
╚══════════════════════════════════════════════════════════════╝
", oldLevel, newLevel, xp, source);
                Console.ResetColor();
                await Task.Delay(3000);
            }
            else
            {
                _renderer.Success($"Получено +{xp} XP! ({source})");
            }

            _dataService.SaveLevelSystem(levelSystem);
        }

        private async Task Budgets()
        {
            _renderer.Clear();
            _renderer.Header("💰 БЮДЖЕТЫ");

            var budgets = _dataService.LoadBudgets();
            var transactions = _dataService.LoadTransactions();

            if (budgets.Any())
            {
                foreach (var budget in budgets)
                {
                    var spent = transactions
                        .Where(t => t.Category == budget.Category && t.Type == TransactionType.Expense)
                        .Sum(t => t.Amount);
                    var progress = budget.Amount > 0 ? (spent / budget.Amount) * 100 : 0;

                    Console.WriteLine($"\n{budget.Category}:");
                    Console.WriteLine($"  Бюджет: {budget.Amount:N2} {budget.Currency}");
                    Console.WriteLine($"  Потрачено: {spent:N2} {budget.Currency}");
                    Console.Write($"  Прогресс: [");
                    var filled = (int)(progress / 2);
                    Console.ForegroundColor = progress > 100 ? ConsoleColor.Red : ConsoleColor.Green;
                    Console.Write(new string('█', filled));
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(new string('░', 50 - filled));
                    Console.ResetColor();
                    Console.WriteLine($"] {progress:F1}%");
                }
            }
            else
            {
                _renderer.Write("Бюджетов пока нет.", ConsoleColor.Gray);
            }

            _renderer.Menu(new[]
            {
                "Добавить бюджет",
                "Удалить бюджет",
                "Назад"
            });

            var choice = _renderer.Read("\nВыберите опцию: ");
            
            switch (choice)
            {
                case "1":
                    AddBudget(budgets);
                    break;
                case "2":
                    DeleteBudget(budgets);
                    break;
            }
        }

        private void AddBudget(List<Budget> budgets)
        {
            var categories = _dataService.LoadCategories();
            _renderer.Write("\nКатегории:", ConsoleColor.Cyan);
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }
            var categoryIndex = _renderer.ReadInt("Выберите категорию: ", 1, categories.Count) - 1;
            var category = categories[categoryIndex];

            var amount = _renderer.ReadDecimal("Сумма бюджета: ");
            var currencies = _currencyService.GetAvailableCurrencies();
            var currencyIndex = _renderer.ReadInt("Валюта (1-RUB, 2-USD, 3-EUR): ", 1, 3) - 1;
            var currency = currencies[currencyIndex].Code;

            budgets.Add(new Budget
            {
                Category = category,
                Amount = amount,
                Currency = currency,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            });

            _dataService.SaveBudgets(budgets);
            _renderer.Success("Бюджет добавлен!");
        }

        private void DeleteBudget(List<Budget> budgets)
        {
            if (!budgets.Any())
            {
                _renderer.Warning("Нет бюджетов для удаления.");
                return;
            }

            for (int i = 0; i < budgets.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {budgets[i].Category} - {budgets[i].Amount:N2} {budgets[i].Currency}");
            }

            var index = _renderer.ReadInt("Номер бюджета: ", 1, budgets.Count) - 1;
            budgets.RemoveAt(index);
            _dataService.SaveBudgets(budgets);
            _renderer.Success("Бюджет удален!");
        }

        private async Task CurrencyConverter()
        {
            _renderer.Clear();
            _renderer.Header("💱 КОНВЕРТЕР ВАЛЮТ");

            var currencies = _currencyService.GetAvailableCurrencies();
            _renderer.Write("Доступные валюты:", ConsoleColor.Cyan);
            for (int i = 0; i < currencies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {currencies[i].Code} - {currencies[i].Name} ({currencies[i].Symbol})");
            }

            var fromIndex = _renderer.ReadInt("\nИз валюты (номер): ", 1, currencies.Count) - 1;
            var fromCurrency = currencies[fromIndex].Code;

            var amount = _renderer.ReadDecimal("Сумма: ");

            var toIndex = _renderer.ReadInt("В валюту (номер): ", 1, currencies.Count) - 1;
            var toCurrency = currencies[toIndex].Code;

            _renderer.Write("Загрузка курса...", ConsoleColor.Yellow);
            var rate = await _currencyService.GetRate(fromCurrency, toCurrency);
            var converted = amount * rate;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n{amount:N2} {fromCurrency} = {converted:N2} {toCurrency}");
            Console.WriteLine($"Курс: 1 {fromCurrency} = {rate:N4} {toCurrency}");
            Console.ResetColor();
        }

        private void Statistics()
        {
            _renderer.Clear();
            _renderer.Header("📈 СТАТИСТИКА И ДОСТИЖЕНИЯ");

            var transactions = _dataService.LoadTransactions();
            var categories = _dataService.LoadCategories();
            var entries = _dataService.LoadDailyEntries();
            var levelSystem = _dataService.LoadLevelSystem();
            var achievements = _dataService.LoadAchievements();

            // Update achievements
            foreach (var achievement in achievements.Where(a => !a.IsUnlocked))
            {
                int currentValue = 0;
                switch (achievement.Type)
                {
                    case AchievementType.Level:
                        currentValue = levelSystem.Level;
                        break;
                    case AchievementType.TasksCompleted:
                        currentValue = entries.SelectMany(e => e.Tasks).Count(t => t.IsCompleted);
                        break;
                    case AchievementType.TransactionsCount:
                        currentValue = transactions.Count;
                        break;
                    case AchievementType.DaysActive:
                        currentValue = entries.Select(e => e.Date.Date).Distinct().Count();
                        break;
                }
                achievement.CurrentValue = currentValue;
                if (currentValue >= achievement.TargetValue)
                {
                    achievement.IsUnlocked = true;
                    achievement.UnlockedDate = DateTime.Now;
                }
            }
            _dataService.SaveAchievements(achievements);

            _renderer.Write("\n📊 Статистика по категориям:", ConsoleColor.Cyan);
            foreach (var category in categories)
            {
                var total = transactions
                    .Where(t => t.Category == category && t.Type == TransactionType.Expense)
                    .Sum(t => t.Amount);
                if (total > 0)
                {
                    Console.WriteLine($"  {category}: {total:N2} ₽");
                }
            }

            _renderer.Write("\n🏆 Достижения:", ConsoleColor.Cyan);
            foreach (var achievement in achievements)
            {
                _renderer.ShowAchievement(achievement);
            }
        }

        private async Task Recurring()
        {
            _renderer.Clear();
            _renderer.Header("🔄 ПОВТОРЯЮЩИЕСЯ ТРАНЗАКЦИИ");

            var recurring = _dataService.LoadRecurringTransactions();
            
            if (recurring.Any())
            {
                foreach (var transaction in recurring)
                {
                    Console.WriteLine($"{transaction.Description} - {transaction.Amount:N2} {transaction.Currency} ({transaction.Recurrence})");
                }
            }
            else
            {
                _renderer.Write("Повторяющихся транзакций нет.", ConsoleColor.Gray);
            }

            _renderer.Menu(new[]
            {
                "Добавить повторяющуюся транзакцию",
                "Удалить",
                "Назад"
            });

            var choice = _renderer.Read("\nВыберите опцию: ");
            if (choice == "1")
            {
                AddRecurringTransaction(recurring);
            }
            else if (choice == "2" && recurring.Any())
            {
                var index = _renderer.ReadInt("Номер: ", 1, recurring.Count) - 1;
                recurring.RemoveAt(index);
                _dataService.SaveRecurringTransactions(recurring);
                _renderer.Success("Удалено!");
            }
        }

        private void AddRecurringTransaction(List<RecurringTransaction> recurring)
        {
            var description = _renderer.Read("Описание: ");
            var amount = _renderer.ReadDecimal("Сумма: ");
            var typeChoice = _renderer.Read("Тип (1-Доход, 2-Расход): ");
            var type = typeChoice == "1" ? TransactionType.Income : TransactionType.Expense;

            var categories = _dataService.LoadCategories();
            var categoryIndex = _renderer.ReadInt("Категория (номер): ", 1, categories.Count) - 1;
            var category = categories[categoryIndex];

            _renderer.Menu(new[] { "Ежедневно", "Еженедельно", "Ежемесячно", "Ежегодно" });
            var recurrenceChoice = _renderer.Read("Повторение: ");
            var recurrence = recurrenceChoice switch
            {
                "1" => RecurrenceType.Daily,
                "2" => RecurrenceType.Weekly,
                "3" => RecurrenceType.Monthly,
                "4" => RecurrenceType.Yearly,
                _ => RecurrenceType.Monthly
            };

            recurring.Add(new RecurringTransaction
            {
                Description = description,
                Amount = amount,
                Type = type,
                Category = category,
                Currency = "RUB",
                Recurrence = recurrence,
                StartDate = DateTime.Now
            });

            _dataService.SaveRecurringTransactions(recurring);
            _renderer.Success("Добавлено!");
        }

        private void ExportCsv()
        {
            var transactions = _dataService.LoadTransactions();
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = $"FinancialPlanner_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var filePath = Path.Combine(folder, fileName);

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Дата,Описание,Сумма,Тип,Категория,Валюта,Заметки");

            foreach (var transaction in transactions)
            {
                csv.AppendLine($"{transaction.Date:yyyy-MM-dd HH:mm}," +
                             $"\"{transaction.Description}\"," +
                             $"{transaction.Amount}," +
                             $"{transaction.Type}," +
                             $"\"{transaction.Category}\"," +
                             $"{transaction.Currency}," +
                             $"\"{transaction.Notes}\"");
            }

            System.IO.File.WriteAllText(filePath, csv.ToString(), System.Text.Encoding.UTF8);
            _renderer.Success($"Данные экспортированы в:\n{filePath}");
        }

        private async Task Projects()
        {
            _renderer.Clear();
            _renderer.Header("🚀 ПЛАНИРОВЩИК ПРОЕКТОВ");

            var projects = _dataService.LoadProjects();
            var transactions = _dataService.LoadTransactions();

            if (projects.Any())
            {
                _renderer.Write("\nАктивные проекты:\n", ConsoleColor.Cyan);
                foreach (var project in projects.Where(p => p.Status == ProjectStatus.InProgress || p.Status == ProjectStatus.Planned))
                {
                    ShowProjectSummary(project);
                }

                _renderer.Write("\nЗавершенные проекты:\n", ConsoleColor.Gray);
                foreach (var project in projects.Where(p => p.Status == ProjectStatus.Completed).Take(5))
                {
                    ShowProjectSummary(project);
                }
            }
            else
            {
                _renderer.Write("Проектов пока нет.", ConsoleColor.Gray);
            }

            _renderer.Menu(new[]
            {
                "Добавить проект",
                "Управление проектом",
                "Добавить этап проекта",
                "Завершить этап",
                "Добавить расход по проекту",
                "Получить оплату по проекту",
                "Удалить проект",
                "Назад"
            });

            var choice = _renderer.Read("\nВыберите опцию: ");
            
            switch (choice)
            {
                case "1":
                    AddProject(projects);
                    break;
                case "2":
                    ManageProject(projects);
                    break;
                case "3":
                    AddMilestone(projects);
                    break;
                case "4":
                    await CompleteMilestoneAsync(projects);
                    break;
                case "5":
                    AddProjectExpense(projects, transactions);
                    break;
                case "6":
                    await ReceiveProjectPaymentAsync(projects, transactions);
                    break;
                case "7":
                    DeleteProject(projects);
                    break;
            }
        }

        private void ShowProjectSummary(Project project)
        {
            var statusColor = project.Status switch
            {
                ProjectStatus.InProgress => ConsoleColor.Green,
                ProjectStatus.Completed => ConsoleColor.Cyan,
                ProjectStatus.OnHold => ConsoleColor.Yellow,
                ProjectStatus.Cancelled => ConsoleColor.Red,
                _ => ConsoleColor.Gray
            };

            Console.ForegroundColor = statusColor;
            Console.Write($"┌─ {project.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"({project.Status}) ─┐");
            
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"│  Клиент: {project.Client}");
            Console.WriteLine($"│  Бюджет: {project.Budget:N2} {project.Currency}");
            Console.WriteLine($"│  Получено: {project.ReceivedAmount:N2} {project.Currency}");
            Console.WriteLine($"│  Расходы: {project.TotalExpenses:N2} {project.Currency}");
            Console.ForegroundColor = project.Profit >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"│  Прибыль: {project.Profit:N2} {project.Currency}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"│  Прогресс: {project.CompletionPercentage:F1}%");
            if (project.Deadline.HasValue)
            {
                var daysLeft = (project.Deadline.Value - DateTime.Now).Days;
                var deadlineColor = daysLeft < 0 ? ConsoleColor.Red : daysLeft < 7 ? ConsoleColor.Yellow : ConsoleColor.Green;
                Console.ForegroundColor = deadlineColor;
                Console.WriteLine($"│  Дедлайн: {project.Deadline.Value:dd.MM.yyyy} ({daysLeft} дн.)");
            }
            Console.ForegroundColor = statusColor;
            Console.WriteLine("└" + new string('─', 68) + "┘");
            Console.ResetColor();
        }

        private void AddProject(List<Project> projects)
        {
            _renderer.Clear();
            _renderer.Header("➕ НОВЫЙ ПРОЕКТ");

            var name = _renderer.Read("Название проекта: ");
            var description = _renderer.Read("Описание (Enter для пропуска): ");
            var client = _renderer.Read("Клиент: ");
            var budget = _renderer.ReadDecimal("Бюджет проекта: ");
            var expectedIncome = _renderer.ReadDecimal("Ожидаемый доход: ");
            var priority = _renderer.ReadInt("Приоритет (1-5): ", 1, 5);

            var deadlineStr = _renderer.Read("Дедлайн (ДД.ММ.ГГГГ или Enter): ");
            DateTime? deadline = null;
            if (!string.IsNullOrEmpty(deadlineStr) && DateTime.TryParse(deadlineStr, out var deadlineDate))
            {
                deadline = deadlineDate;
            }

            var project = new Project
            {
                Name = name,
                Description = description,
                Client = client,
                Budget = budget,
                ExpectedIncome = expectedIncome,
                Priority = priority,
                Deadline = deadline,
                Status = ProjectStatus.Planned
            };

            projects.Add(project);
            _dataService.SaveProjects(projects);
            _renderer.Success("Проект добавлен!");
        }

        private void ManageProject(List<Project> projects)
        {
            if (!projects.Any())
            {
                _renderer.Warning("Нет проектов для управления.");
                return;
            }

            _renderer.Write("\nВыберите проект:", ConsoleColor.Cyan);
            for (int i = 0; i < projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {projects[i].Name} ({projects[i].Status})");
            }

            var index = _renderer.ReadInt("Номер проекта: ", 1, projects.Count) - 1;
            var project = projects[index];

            _renderer.Menu(new[]
            {
                "Изменить статус",
                "Изменить приоритет",
                "Добавить заметки",
                "Просмотр деталей"
            });

            var choice = _renderer.Read("\nВыберите опцию: ");
            
            switch (choice)
            {
                case "1":
                    ChangeProjectStatus(project);
                    break;
                case "2":
                    project.Priority = _renderer.ReadInt("Новый приоритет (1-5): ", 1, 5);
                    _dataService.SaveProjects(projects);
                    _renderer.Success("Приоритет обновлен!");
                    break;
                case "3":
                    project.Notes = _renderer.Read("Заметки: ");
                    _dataService.SaveProjects(projects);
                    _renderer.Success("Заметки сохранены!");
                    break;
                case "4":
                    ShowProjectDetails(project);
                    break;
            }
        }

        private void ChangeProjectStatus(Project project)
        {
            _renderer.Write("\nСтатусы:", ConsoleColor.Cyan);
            Console.WriteLine("1. Запланирован");
            Console.WriteLine("2. В работе");
            Console.WriteLine("3. Приостановлен");
            Console.WriteLine("4. Завершен");
            Console.WriteLine("5. Отменен");

            var choice = _renderer.Read("Выберите статус: ");
            project.Status = choice switch
            {
                "1" => ProjectStatus.Planned,
                "2" => ProjectStatus.InProgress,
                "3" => ProjectStatus.OnHold,
                "4" => ProjectStatus.Completed,
                "5" => ProjectStatus.Cancelled,
                _ => project.Status
            };

            if (project.Status == ProjectStatus.Completed)
            {
                project.EndDate = DateTime.Now;
            }

            _dataService.SaveProjects(_dataService.LoadProjects());
            _renderer.Success("Статус обновлен!");
        }

        private void ShowProjectDetails(Project project)
        {
            _renderer.Clear();
            _renderer.Header($"📋 {project.Name.ToUpper()}");

            Console.WriteLine($"Клиент: {project.Client}");
            Console.WriteLine($"Статус: {project.Status}");
            Console.WriteLine($"Приоритет: {project.Priority}/5");
            Console.WriteLine($"Бюджет: {project.Budget:N2} {project.Currency}");
            Console.WriteLine($"Получено: {project.ReceivedAmount:N2} {project.Currency}");
            Console.WriteLine($"Ожидаемый доход: {project.ExpectedIncome:N2} {project.Currency}");
            Console.WriteLine($"Расходы: {project.TotalExpenses:N2} {project.Currency}");
            Console.ForegroundColor = project.Profit >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"Прибыль: {project.Profit:N2} {project.Currency}");
            Console.ResetColor();
            Console.WriteLine($"Прогресс: {project.CompletionPercentage:F1}%");

            if (project.Milestones.Any())
            {
                _renderer.Write("\nЭтапы проекта:", ConsoleColor.Cyan);
                foreach (var milestone in project.Milestones)
                {
                    var icon = milestone.IsCompleted ? "✓" : "○";
                    var color = milestone.IsCompleted ? ConsoleColor.Green : ConsoleColor.Gray;
                    Console.ForegroundColor = color;
                    Console.WriteLine($"  {icon} {milestone.Name} - {milestone.PaymentAmount:N2} {project.Currency}");
                    Console.ResetColor();
                }
            }

            if (project.Expenses.Any())
            {
                _renderer.Write("\nРасходы по проекту:", ConsoleColor.Cyan);
                foreach (var expense in project.Expenses.OrderByDescending(e => e.Date))
                {
                    Console.WriteLine($"  {expense.Date:dd.MM.yyyy} - {expense.Description}: {expense.Amount:N2} {project.Currency}");
                }
            }

            if (!string.IsNullOrEmpty(project.Notes))
            {
                _renderer.Write("\nЗаметки:", ConsoleColor.Cyan);
                Console.WriteLine(project.Notes);
            }
        }

        private void AddMilestone(List<Project> projects)
        {
            if (!projects.Any())
            {
                _renderer.Warning("Нет проектов.");
                return;
            }

            _renderer.Write("\nВыберите проект:", ConsoleColor.Cyan);
            for (int i = 0; i < projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {projects[i].Name}");
            }

            var index = _renderer.ReadInt("Номер проекта: ", 1, projects.Count) - 1;
            var project = projects[index];

            var name = _renderer.Read("Название этапа: ");
            var description = _renderer.Read("Описание: ");
            var amount = _renderer.ReadDecimal("Сумма оплаты: ");
            var dueDateStr = _renderer.Read("Срок (ДД.ММ.ГГГГ или Enter): ");
            DateTime? dueDate = null;
            if (!string.IsNullOrEmpty(dueDateStr) && DateTime.TryParse(dueDateStr, out var dueDateParsed))
            {
                dueDate = dueDateParsed;
            }

            project.Milestones.Add(new ProjectMilestone
            {
                Name = name,
                Description = description,
                PaymentAmount = amount,
                DueDate = dueDate,
                XP = 50 + (project.Priority * 10)
            });

            _dataService.SaveProjects(projects);
            _renderer.Success("Этап добавлен!");
        }

        private async Task CompleteMilestoneAsync(List<Project> projects)
        {
            var activeProjects = projects.Where(p => p.Status == ProjectStatus.InProgress || p.Status == ProjectStatus.Planned).ToList();
            if (!activeProjects.Any())
            {
                _renderer.Warning("Нет активных проектов.");
                return;
            }

            _renderer.Write("\nВыберите проект:", ConsoleColor.Cyan);
            for (int i = 0; i < activeProjects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {activeProjects[i].Name}");
            }

            var projectIndex = _renderer.ReadInt("Номер проекта: ", 1, activeProjects.Count) - 1;
            var project = activeProjects[projectIndex];

            var incompleteMilestones = project.Milestones.Where(m => !m.IsCompleted).ToList();
            if (!incompleteMilestones.Any())
            {
                _renderer.Warning("Нет незавершенных этапов.");
                return;
            }

            _renderer.Write("\nВыберите этап:", ConsoleColor.Cyan);
            for (int i = 0; i < incompleteMilestones.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {incompleteMilestones[i].Name} - {incompleteMilestones[i].PaymentAmount:N2} {project.Currency}");
            }

            var milestoneIndex = _renderer.ReadInt("Номер этапа: ", 1, incompleteMilestones.Count) - 1;
            var milestone = incompleteMilestones[milestoneIndex];

            milestone.IsCompleted = true;
            milestone.CompletedDate = DateTime.Now;
            project.ReceivedAmount += milestone.PaymentAmount;

            await AddXPAsync(milestone.XP, $"Этап проекта: {milestone.Name}");
            _dataService.SaveProjects(projects);
            _renderer.Success($"Этап завершен! Получено +{milestone.XP} XP");
        }

        private void AddProjectExpense(List<Project> projects, List<Transaction> transactions)
        {
            if (!projects.Any())
            {
                _renderer.Warning("Нет проектов.");
                return;
            }

            _renderer.Write("\nВыберите проект:", ConsoleColor.Cyan);
            for (int i = 0; i < projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {projects[i].Name}");
            }

            var index = _renderer.ReadInt("Номер проекта: ", 1, projects.Count) - 1;
            var project = projects[index];

            var description = _renderer.Read("Описание расхода: ");
            var amount = _renderer.ReadDecimal("Сумма: ");
            var category = _renderer.Read("Категория: ");

            var expense = new ProjectExpense
            {
                Description = description,
                Amount = amount,
                Category = category,
                Date = DateTime.Now
            };

            project.Expenses.Add(expense);

            // Также добавляем как транзакцию
            transactions.Add(new Transaction
            {
                Description = $"[{project.Name}] {description}",
                Amount = amount,
                Type = TransactionType.Expense,
                Category = category,
                Currency = project.Currency,
                Date = DateTime.Now
            });

            _dataService.SaveProjects(projects);
            _dataService.SaveTransactions(transactions);
            _renderer.Success("Расход добавлен!");
        }

        private async Task ReceiveProjectPaymentAsync(List<Project> projects, List<Transaction> transactions)
        {
            var activeProjects = projects.Where(p => p.Status == ProjectStatus.InProgress || p.Status == ProjectStatus.Planned).ToList();
            if (!activeProjects.Any())
            {
                _renderer.Warning("Нет активных проектов.");
                return;
            }

            _renderer.Write("\nВыберите проект:", ConsoleColor.Cyan);
            for (int i = 0; i < activeProjects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {activeProjects[i].Name}");
            }

            var index = _renderer.ReadInt("Номер проекта: ", 1, activeProjects.Count) - 1;
            var project = activeProjects[index];

            var amount = _renderer.ReadDecimal("Сумма оплаты: ");
            var description = _renderer.Read("Описание (Enter для пропуска): ");
            if (string.IsNullOrEmpty(description))
            {
                description = $"Оплата по проекту {project.Name}";
            }

            project.ReceivedAmount += amount;

            // Добавляем как транзакцию дохода
            transactions.Add(new Transaction
            {
                Description = $"[{project.Name}] {description}",
                Amount = amount,
                Type = TransactionType.Income,
                Category = "Проекты",
                Currency = project.Currency,
                Date = DateTime.Now
            });

            // Награда за получение оплаты
            var xp = (int)(amount / 100); // 1 XP за каждые 100 единиц валюты
            await AddXPAsync(Math.Min(xp, 500), $"Оплата по проекту: {project.Name}");

            _dataService.SaveProjects(projects);
            _dataService.SaveTransactions(transactions);
            _renderer.Success($"Оплата получена! +{xp} XP");
        }

        private void DeleteProject(List<Project> projects)
        {
            if (!projects.Any())
            {
                _renderer.Warning("Нет проектов для удаления.");
                return;
            }

            for (int i = 0; i < projects.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {projects[i].Name}");
            }

            var index = _renderer.ReadInt("Номер проекта: ", 1, projects.Count) - 1;
            projects.RemoveAt(index);
            _dataService.SaveProjects(projects);
            _renderer.Success("Проект удален!");
        }

        private void Analytics()
        {
            _renderer.Clear();
            _renderer.Header("📊 РАСШИРЕННАЯ АНАЛИТИКА");

            var transactions = _dataService.LoadTransactions();
            var projects = _dataService.LoadProjects();
            var budgets = _dataService.LoadBudgets();

            var now = DateTime.Now;
            var thisMonth = transactions.Where(t => t.Date.Year == now.Year && t.Date.Month == now.Month).ToList();
            var lastMonth = transactions.Where(t => 
            {
                var lastMonthDate = now.AddMonths(-1);
                return t.Date.Year == lastMonthDate.Year && t.Date.Month == lastMonthDate.Month;
            }).ToList();

            var thisMonthIncome = thisMonth.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var thisMonthExpenses = thisMonth.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            var lastMonthIncome = lastMonth.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var lastMonthExpenses = lastMonth.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            _renderer.Write("\n📈 Сравнение месяцев:", ConsoleColor.Cyan);
            Console.WriteLine($"Текущий месяц:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Доходы: {thisMonthIncome:N2} ₽");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  Расходы: {thisMonthExpenses:N2} ₽");
            Console.ForegroundColor = thisMonthIncome - thisMonthExpenses >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"  Баланс: {thisMonthIncome - thisMonthExpenses:N2} ₽");
            Console.ResetColor();

            Console.WriteLine($"\nПрошлый месяц:");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  Доходы: {lastMonthIncome:N2} ₽");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  Расходы: {lastMonthExpenses:N2} ₽");
            Console.ForegroundColor = lastMonthIncome - lastMonthExpenses >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"  Баланс: {lastMonthIncome - lastMonthExpenses:N2} ₽");
            Console.ResetColor();

            var incomeChange = lastMonthIncome > 0 ? ((thisMonthIncome - lastMonthIncome) / lastMonthIncome) * 100 : 0;
            var expenseChange = lastMonthExpenses > 0 ? ((thisMonthExpenses - lastMonthExpenses) / lastMonthExpenses) * 100 : 0;

            Console.WriteLine($"\nИзменения:");
            Console.ForegroundColor = incomeChange >= 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"  Доходы: {incomeChange:+#0.0;-#0.0}%");
            Console.ForegroundColor = expenseChange <= 0 ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"  Расходы: {expenseChange:+#0.0;-#0.0}%");
            Console.ResetColor();

            // Прогноз доходов на основе проектов
            var activeProjects = projects.Where(p => p.Status == ProjectStatus.InProgress || p.Status == ProjectStatus.Planned).ToList();
            if (activeProjects.Any())
            {
                _renderer.Write("\n💰 Прогноз доходов (на основе проектов):", ConsoleColor.Cyan);
                var expectedIncome = activeProjects.Sum(p => p.ExpectedIncome - p.ReceivedAmount);
                var receivedIncome = activeProjects.Sum(p => p.ReceivedAmount);
                Console.WriteLine($"  Получено: {receivedIncome:N2} ₽");
                Console.WriteLine($"  Ожидается: {expectedIncome:N2} ₽");
                Console.WriteLine($"  Всего: {activeProjects.Sum(p => p.ExpectedIncome):N2} ₽");
            }

            // Топ категорий расходов
            var topExpenseCategories = thisMonth
                .Where(t => t.Type == TransactionType.Expense)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();

            if (topExpenseCategories.Any())
            {
                _renderer.Write("\n🔥 Топ категорий расходов (этот месяц):", ConsoleColor.Cyan);
                foreach (var cat in topExpenseCategories)
                {
                    Console.WriteLine($"  {cat.Category}: {cat.Total:N2} ₽");
                }
            }

            // Топ источников дохода
            var topIncomeSources = thisMonth
                .Where(t => t.Type == TransactionType.Income)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();

            if (topIncomeSources.Any())
            {
                _renderer.Write("\n💎 Топ источников дохода (этот месяц):", ConsoleColor.Cyan);
                foreach (var source in topIncomeSources)
                {
                    Console.WriteLine($"  {source.Category}: {source.Total:N2} ₽");
                }
            }

            // Анализ проектов
            if (projects.Any())
            {
                _renderer.Write("\n🚀 Анализ проектов:", ConsoleColor.Cyan);
                var totalProfit = projects.Sum(p => p.Profit);
                var avgProfit = projects.Average(p => p.Profit);
                var completedProjects = projects.Count(p => p.Status == ProjectStatus.Completed);
                Console.WriteLine($"  Всего проектов: {projects.Count}");
                Console.WriteLine($"  Завершено: {completedProjects}");
                Console.WriteLine($"  Общая прибыль: {totalProfit:N2} ₽");
                Console.WriteLine($"  Средняя прибыль: {avgProfit:N2} ₽");
            }
        }
    }
}
