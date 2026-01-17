using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinancialPlanner.Models;
using FinancialPlanner.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FinancialPlanner.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private readonly CurrencyService _currencyService;

        [ObservableProperty]
        private ObservableCollection<Transaction> transactions = new();

        [ObservableProperty]
        private ObservableCollection<DailyEntry> dailyEntries = new();

        [ObservableProperty]
        private ObservableCollection<string> availableHabits = new();

        [ObservableProperty]
        private ObservableCollection<Budget> budgets = new();

        [ObservableProperty]
        private ObservableCollection<string> categories = new();

        [ObservableProperty]
        private ObservableCollection<Currency> availableCurrencies = new();

        [ObservableProperty]
        private DailyEntry? selectedDailyEntry;

        [ObservableProperty]
        private Transaction? selectedTransaction;

        [ObservableProperty]
        private Budget? selectedBudget;

        [ObservableProperty]
        private decimal totalIncome;

        [ObservableProperty]
        private decimal totalExpenses;

        [ObservableProperty]
        private decimal balance;

        [ObservableProperty]
        private string newTransactionDescription = string.Empty;

        [ObservableProperty]
        private decimal newTransactionAmount;

        [ObservableProperty]
        private TransactionType newTransactionType = TransactionType.Expense;

        [ObservableProperty]
        private string newTransactionCategory = string.Empty;

        [ObservableProperty]
        private string newTransactionCurrency = "RUB";

        [ObservableProperty]
        private string newHabitName = string.Empty;

        [ObservableProperty]
        private string newTaskDescription = string.Empty;

        [ObservableProperty]
        private int newTaskPriority = 3;

        [ObservableProperty]
        private string dailyNotes = string.Empty;

        [ObservableProperty]
        private int dailyMood = 5;

        [ObservableProperty]
        private string newBudgetCategory = string.Empty;

        [ObservableProperty]
        private decimal newBudgetAmount;

        [ObservableProperty]
        private string newBudgetCurrency = "RUB";

        [ObservableProperty]
        private string newCategoryName = string.Empty;

        [ObservableProperty]
        private string currencyFrom = "RUB";

        [ObservableProperty]
        private string currencyTo = "USD";

        [ObservableProperty]
        private decimal currencyAmount = 0;

        [ObservableProperty]
        private decimal convertedAmount = 0;

        [ObservableProperty]
        private bool isLoadingRates = false;

        [ObservableProperty]
        private string baseCurrency = "RUB";

        [ObservableProperty]
        private LevelSystem levelSystem = new();

        [ObservableProperty]
        private bool showLevelUpAnimation = false;

        [ObservableProperty]
        private string levelUpMessage = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Achievement> achievements = new();

        [ObservableProperty]
        private ObservableCollection<RecurringTransaction> recurringTransactions = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private DateTime? filterStartDate;

        [ObservableProperty]
        private DateTime? filterEndDate;

        [ObservableProperty]
        private string filterCategory = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Transaction> filteredTransactions = new();

        public MainViewModel()
        {
            _dataService = new DataService();
            _currencyService = new CurrencyService();
            LoadData();
            InitializeTodayEntry();
            LoadCurrencies();
            LoadLevelSystem();
            LoadAchievements();
            LoadRecurringTransactions();
            CheckRecurringTransactions();
            UpdateAchievements();
            FilterTransactions(); // Инициализация фильтрованных транзакций
            _ = LoadExchangeRatesAsync();
            
            // Подписка на изменения для фильтрации
            Transactions.CollectionChanged += (s, e) => FilterTransactions();
        }

        private void LoadData()
        {
            var transactions = _dataService.LoadTransactions();
            Transactions = new ObservableCollection<Transaction>(transactions.OrderByDescending(t => t.Date));

            var entries = _dataService.LoadDailyEntries();
            DailyEntries = new ObservableCollection<DailyEntry>(entries.OrderByDescending(e => e.Date));

            var habits = _dataService.LoadHabits();
            AvailableHabits = new ObservableCollection<string>(habits);

            var budgets = _dataService.LoadBudgets();
            Budgets = new ObservableCollection<Budget>(budgets);

            var categories = _dataService.LoadCategories();
            Categories = new ObservableCollection<string>(categories);

            CalculateTotals();
        }

        private void LoadLevelSystem()
        {
            LevelSystem = _dataService.LoadLevelSystem();
            UpdateLevelProgress();
        }

        private void LoadAchievements()
        {
            var achievements = _dataService.LoadAchievements();
            Achievements = new ObservableCollection<Achievement>(achievements);
        }

        private void LoadRecurringTransactions()
        {
            var recurring = _dataService.LoadRecurringTransactions();
            RecurringTransactions = new ObservableCollection<RecurringTransaction>(recurring);
        }

        private void CheckRecurringTransactions()
        {
            var today = DateTime.Today;
            foreach (var recurring in RecurringTransactions.Where(r => r.IsActive))
            {
                bool shouldCreate = false;
                DateTime? lastTransactionDate = Transactions
                    .Where(t => t.Description == recurring.Description && 
                               t.Amount == recurring.Amount &&
                               t.Category == recurring.Category)
                    .OrderByDescending(t => t.Date)
                    .FirstOrDefault()?.Date.Date;

                switch (recurring.Recurrence)
                {
                    case RecurrenceType.Daily:
                        shouldCreate = !lastTransactionDate.HasValue || lastTransactionDate.Value < today;
                        break;
                    case RecurrenceType.Weekly:
                        var daysSince = (today - (lastTransactionDate ?? recurring.StartDate)).Days;
                        shouldCreate = daysSince >= 7;
                        break;
                    case RecurrenceType.Monthly:
                        shouldCreate = today.Day == recurring.DayOfMonth && 
                                     (!lastTransactionDate.HasValue || lastTransactionDate.Value.Month < today.Month || lastTransactionDate.Value.Year < today.Year);
                        break;
                    case RecurrenceType.Yearly:
                        shouldCreate = today.Day == recurring.StartDate.Day && 
                                     today.Month == recurring.StartDate.Month &&
                                     (!lastTransactionDate.HasValue || lastTransactionDate.Value.Year < today.Year);
                        break;
                }

                if (shouldCreate && (recurring.EndDate == null || today <= recurring.EndDate.Value))
                {
                    var transaction = new Transaction
                    {
                        Description = recurring.Description,
                        Amount = recurring.Amount,
                        Type = recurring.Type,
                        Category = recurring.Category,
                        Currency = recurring.Currency,
                        Date = DateTime.Now
                    };
                    Transactions.Insert(0, transaction);
                }
            }

            if (RecurringTransactions.Any(r => r.IsActive))
            {
                _dataService.SaveTransactions(Transactions.ToList());
                CalculateTotals();
            }
        }

        private void UpdateAchievements()
        {
            bool changed = false;

            foreach (var achievement in Achievements.Where(a => !a.IsUnlocked))
            {
                int currentValue = 0;
                switch (achievement.Type)
                {
                    case AchievementType.Level:
                        currentValue = LevelSystem.Level;
                        break;
                    case AchievementType.TasksCompleted:
                        currentValue = DailyEntries.SelectMany(e => e.Tasks).Count(t => t.IsCompleted);
                        break;
                    case AchievementType.HabitsStreak:
                        // Упрощенная логика - считаем выполненные привычки за последние дни
                        currentValue = DailyEntries
                            .Where(e => e.Date >= DateTime.Today.AddDays(-7))
                            .SelectMany(e => e.Habits)
                            .Count(h => h.IsCompleted);
                        break;
                    case AchievementType.TransactionsCount:
                        currentValue = Transactions.Count;
                        break;
                    case AchievementType.BudgetSaved:
                        currentValue = (int)Budgets.Sum(b => Math.Max(0, b.Amount - GetCategoryTotal(b.Category)));
                        break;
                    case AchievementType.DaysActive:
                        currentValue = DailyEntries.Select(e => e.Date.Date).Distinct().Count();
                        break;
                }

                achievement.CurrentValue = currentValue;
                if (currentValue >= achievement.TargetValue)
                {
                    achievement.IsUnlocked = true;
                    achievement.UnlockedDate = DateTime.Now;
                    changed = true;
                }
            }

            if (changed)
            {
                _dataService.SaveAchievements(Achievements.ToList());
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterTransactions();
        }

        partial void OnFilterStartDateChanged(DateTime? value)
        {
            FilterTransactions();
        }

        partial void OnFilterEndDateChanged(DateTime? value)
        {
            FilterTransactions();
        }

        partial void OnFilterCategoryChanged(string value)
        {
            FilterTransactions();
        }

        private void FilterTransactions()
        {
            var filtered = Transactions.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(t => 
                    t.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    t.Category.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    t.Notes.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (FilterStartDate.HasValue)
            {
                filtered = filtered.Where(t => t.Date.Date >= FilterStartDate.Value.Date);
            }

            if (FilterEndDate.HasValue)
            {
                filtered = filtered.Where(t => t.Date.Date <= FilterEndDate.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(FilterCategory))
            {
                filtered = filtered.Where(t => t.Category == FilterCategory);
            }

            FilteredTransactions = new ObservableCollection<Transaction>(filtered.OrderByDescending(t => t.Date));
        }

        [RelayCommand]
        private void AddRecurringTransaction()
        {
            // Будет вызвано из UI
        }

        [RelayCommand]
        private void DeleteRecurringTransaction(RecurringTransaction? transaction)
        {
            if (transaction == null) return;
            RecurringTransactions.Remove(transaction);
            _dataService.SaveRecurringTransactions(RecurringTransactions.ToList());
        }

        [RelayCommand]
        private void ExportToCsv()
        {
            try
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fileName = $"FinancialPlanner_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var filePath = Path.Combine(folder, fileName);

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("Дата,Описание,Сумма,Тип,Категория,Валюта,Заметки");

                foreach (var transaction in Transactions)
                {
                    csv.AppendLine($"{transaction.Date:yyyy-MM-dd HH:mm}," +
                                 $"\"{transaction.Description}\"," +
                                 $"{transaction.Amount}," +
                                 $"{transaction.Type}," +
                                 $"\"{transaction.Category}\"," +
                                 $"{transaction.Currency}," +
                                 $"\"{transaction.Notes}\"");
                }

                File.WriteAllText(filePath, csv.ToString(), System.Text.Encoding.UTF8);
                
                // Показать сообщение об успехе
                System.Windows.MessageBox.Show($"Данные экспортированы в:\n{filePath}", 
                    "Экспорт завершен", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при экспорте: {ex.Message}", 
                    "Ошибка", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Error);
            }
        }

        private void UpdateLevelProgress()
        {
            var (currentXP, xpToNext) = LevelCalculator.GetLevelProgress(LevelSystem.TotalXP, LevelSystem.Level);
            LevelSystem.CurrentLevelXP = currentXP;
            LevelSystem.XPToNextLevel = xpToNext;
        }

        private void SaveLevelSystem()
        {
            _dataService.SaveLevelSystem(LevelSystem);
        }

        private void LoadCurrencies()
        {
            var currencies = _currencyService.GetAvailableCurrencies();
            AvailableCurrencies = new ObservableCollection<Currency>(currencies);
        }

        private async Task LoadExchangeRatesAsync()
        {
            await Task.CompletedTask;
        }

        private void InitializeTodayEntry()
        {
            var today = DateTime.Today;
            SelectedDailyEntry = DailyEntries.FirstOrDefault(e => e.Date.Date == today);

            if (SelectedDailyEntry == null)
            {
                SelectedDailyEntry = new DailyEntry
                {
                    Date = today,
                    Habits = AvailableHabits.Select(h => new HabitCheck { HabitName = h }).ToList()
                };
                DailyEntries.Insert(0, SelectedDailyEntry);
            }

            DailyNotes = SelectedDailyEntry.Notes;
            DailyMood = SelectedDailyEntry.Mood;
        }

        [RelayCommand]
        private void AddTransaction()
        {
            if (string.IsNullOrWhiteSpace(NewTransactionDescription) || NewTransactionAmount <= 0)
                return;

            var transaction = new Transaction
            {
                Description = NewTransactionDescription,
                Amount = NewTransactionAmount,
                Type = NewTransactionType,
                Category = NewTransactionCategory,
                Currency = NewTransactionCurrency,
                Date = DateTime.Now
            };

            Transactions.Insert(0, transaction);
            _dataService.SaveTransactions(Transactions.ToList());

            NewTransactionDescription = string.Empty;
            NewTransactionAmount = 0;
            NewTransactionCategory = string.Empty;

            CalculateTotals();
            UpdateAchievements();
            FilterTransactions();
        }

        [RelayCommand]
        private void DeleteTransaction(Transaction? transaction)
        {
            if (transaction == null) return;

            Transactions.Remove(transaction);
            _dataService.SaveTransactions(Transactions.ToList());
            CalculateTotals();
        }

        [RelayCommand]
        private void AddHabit()
        {
            if (string.IsNullOrWhiteSpace(NewHabitName)) return;
            if (AvailableHabits.Contains(NewHabitName)) return;

            AvailableHabits.Add(NewHabitName);
            _dataService.SaveHabits(AvailableHabits.ToList());

            if (SelectedDailyEntry != null)
            {
                SelectedDailyEntry.Habits.Add(new HabitCheck { HabitName = NewHabitName });
            }

            NewHabitName = string.Empty;
        }

        [RelayCommand]
        private void DeleteHabit(string? habit)
        {
            if (habit == null) return;

            AvailableHabits.Remove(habit);
            _dataService.SaveHabits(AvailableHabits.ToList());
        }

        [RelayCommand]
        private void AddTask()
        {
            if (string.IsNullOrWhiteSpace(NewTaskDescription) || SelectedDailyEntry == null) return;

            var task = new TaskItem
            {
                Description = NewTaskDescription,
                Priority = NewTaskPriority
            };
            task.XP = LevelCalculator.GetXPForTask(task.Priority);

            SelectedDailyEntry.Tasks.Add(task);

            NewTaskDescription = string.Empty;
            NewTaskPriority = 3;
            SaveDailyEntries();
        }

        [RelayCommand]
        private void SetTaskPriority(TaskItem? task)
        {
            if (task == null) return;
            // Можно добавить диалог для выбора приоритета
            task.XP = LevelCalculator.GetXPForTask(task.Priority);
            SaveDailyEntries();
        }

        [RelayCommand]
        private void DeleteTask(TaskItem? task)
        {
            if (task == null || SelectedDailyEntry == null) return;

            SelectedDailyEntry.Tasks.Remove(task);
            SaveDailyEntries();
        }

        [RelayCommand]
        private void SaveDailyEntry()
        {
            if (SelectedDailyEntry == null) return;

            SelectedDailyEntry.Notes = DailyNotes;
            SelectedDailyEntry.Mood = DailyMood;
            SaveDailyEntries();
        }

        [RelayCommand]
        private void ToggleHabit(HabitCheck? habit)
        {
            if (habit == null) return;
            
            bool wasCompleted = habit.IsCompleted;
            habit.IsCompleted = !habit.IsCompleted;

            // Если привычка выполнена - начисляем XP
            if (habit.IsCompleted && !wasCompleted)
            {
                int xp = LevelCalculator.GetXPForHabit();
                AddXP(xp, $"Привычка: {habit.HabitName}");
            }

            SaveDailyEntries();
        }

        [RelayCommand]
        private void ToggleTask(TaskItem? task)
        {
            if (task == null) return;
            
            bool wasCompleted = task.IsCompleted;
            task.IsCompleted = !task.IsCompleted;

            // Если задача выполнена и XP еще не получена - начисляем XP
            if (task.IsCompleted && !task.XPClaimed && !wasCompleted)
            {
                AddXP(task.XP, $"Задача выполнена: {task.Description}");
                task.XPClaimed = true;
            }
            // Если задача отменена - отнимаем XP (если была получена)
            else if (!task.IsCompleted && task.XPClaimed)
            {
                RemoveXP(task.XP);
                task.XPClaimed = false;
            }

            SaveDailyEntries();
        }

        private void AddXP(int xp, string source)
        {
            int oldLevel = LevelSystem.Level;
            LevelSystem.TotalXP += xp;
            
            // Пересчитываем уровень
            int newLevel = LevelCalculator.CalculateLevel(LevelSystem.TotalXP);
            LevelSystem.Level = newLevel;

            UpdateLevelProgress();

            // Проверяем повышение уровня
            if (newLevel > oldLevel)
            {
                LevelSystem.LastLevelUp = DateTime.Now;
                ShowLevelUpAnimation = true;
                LevelUpMessage = $"🎉 LEVEL UP! 🎉\nУровень {oldLevel} → {newLevel}\n+{xp} XP за {source}";
                
                // Скрываем анимацию через 3 секунды
                Task.Delay(3000).ContinueWith(_ => 
                {
                    Application.Current.Dispatcher.Invoke(() => ShowLevelUpAnimation = false);
                });
            }

            SaveLevelSystem();
            UpdateAchievements();
        }

        private void RemoveXP(int xp)
        {
            LevelSystem.TotalXP = Math.Max(0, LevelSystem.TotalXP - xp);
            int newLevel = LevelCalculator.CalculateLevel(LevelSystem.TotalXP);
            LevelSystem.Level = newLevel;
            UpdateLevelProgress();
            SaveLevelSystem();
        }

        [RelayCommand]
        private void AddBudget()
        {
            if (string.IsNullOrWhiteSpace(NewBudgetCategory) || NewBudgetAmount <= 0) return;

            var budget = new Budget
            {
                Category = NewBudgetCategory,
                Amount = NewBudgetAmount,
                Currency = NewBudgetCurrency,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1)
            };

            Budgets.Add(budget);
            _dataService.SaveBudgets(Budgets.ToList());

            NewBudgetCategory = string.Empty;
            NewBudgetAmount = 0;
        }

        [RelayCommand]
        private void DeleteBudget(Budget? budget)
        {
            if (budget == null) return;

            Budgets.Remove(budget);
            _dataService.SaveBudgets(Budgets.ToList());
        }

        [RelayCommand]
        private void AddCategory()
        {
            if (string.IsNullOrWhiteSpace(NewCategoryName)) return;
            if (Categories.Contains(NewCategoryName)) return;

            Categories.Add(NewCategoryName);
            _dataService.SaveCategories(Categories.ToList());
            NewCategoryName = string.Empty;
        }

        [RelayCommand]
        private void DeleteCategory(string? category)
        {
            if (category == null) return;

            Categories.Remove(category);
            _dataService.SaveCategories(Categories.ToList());
        }

        [RelayCommand]
        private async Task ConvertCurrencyAsync()
        {
            if (CurrencyAmount <= 0) return;

            IsLoadingRates = true;
            try
            {
                var rate = await _currencyService.GetExchangeRateAsync(CurrencyFrom, CurrencyTo);
                ConvertedAmount = CurrencyAmount * rate;
            }
            catch
            {
                ConvertedAmount = _currencyService.ConvertCurrency(CurrencyAmount, CurrencyFrom, CurrencyTo);
            }
            finally
            {
                IsLoadingRates = false;
            }
        }

        partial void OnCurrencyAmountChanged(decimal value)
        {
            _ = ConvertCurrencyAsync();
        }

        partial void OnCurrencyFromChanged(string value)
        {
            _ = ConvertCurrencyAsync();
        }

        partial void OnCurrencyToChanged(string value)
        {
            _ = ConvertCurrencyAsync();
        }

        private void SaveDailyEntries()
        {
            _dataService.SaveDailyEntries(DailyEntries.ToList());
        }

        private void CalculateTotals()
        {
            // Конвертируем все транзакции в базовую валюту
            TotalIncome = Transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Currency == BaseCurrency 
                    ? t.Amount 
                    : _currencyService.ConvertCurrency(t.Amount, t.Currency, BaseCurrency));

            TotalExpenses = Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Currency == BaseCurrency 
                    ? t.Amount 
                    : _currencyService.ConvertCurrency(t.Amount, t.Currency, BaseCurrency));

            Balance = TotalIncome - TotalExpenses;
        }

        public decimal GetCategoryTotal(string category)
        {
            return Transactions
                .Where(t => t.Category == category && t.Type == TransactionType.Expense)
                .Sum(t => t.Currency == BaseCurrency 
                    ? t.Amount 
                    : _currencyService.ConvertCurrency(t.Amount, t.Currency, BaseCurrency));
        }

        public decimal GetBudgetProgress(string category)
        {
            var budget = Budgets.FirstOrDefault(b => b.Category == category);
            if (budget == null) return 0;

            var spent = GetCategoryTotal(category);
            // Конвертируем потраченное в валюту бюджета
            if (budget.Currency != BaseCurrency)
            {
                spent = _currencyService.ConvertCurrency(spent, BaseCurrency, budget.Currency);
            }
            return budget.Amount > 0 ? Math.Min((spent / budget.Amount) * 100, 100) : 0;
        }
    }
}
