using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using NexusFinance.Services;

namespace NexusFinance.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty]
    private decimal _netWorth;

    [ObservableProperty]
    private decimal _monthlyIncome;

    [ObservableProperty]
    private decimal _monthlyExpense;

    [ObservableProperty]
    private decimal _savingsRate;

    [ObservableProperty]
    private string _lastUpdated = DateTime.Now.ToString("dd.MM.yyyy HH:mm");

    [ObservableProperty]
    private ObservableCollection<TransactionItem> _recentTransactions = new();

    [ObservableProperty]
    private ObservableCollection<CategoryExpense> _topExpenses = new();

    public DashboardViewModel()
    {
        _dataService = new DataService();
        LoadDashboardData();
    }

    [RelayCommand]
    public void Refresh()
    {
        LoadDashboardData();
        LastUpdated = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
    }

    private void LoadDashboardData()
    {
        var transactions = _dataService.GetTransactions();
        var accounts = _dataService.GetAccounts();
        var investments = _dataService.GetInvestments();

        // Calculate Net Worth
        var totalAccounts = accounts.Sum(a => a.Balance);
        var totalInvestments = investments.Sum(i => i.CurrentValue);
        NetWorth = totalAccounts + totalInvestments;

        // Calculate monthly income and expenses (last 30 days)
        var thirtyDaysAgo = DateTime.Now.AddDays(-30);
        var recentTransactions = transactions.Where(t => t.Date >= thirtyDaysAgo).ToList();
        
        MonthlyIncome = recentTransactions.Where(t => t.IsIncome).Sum(t => t.Amount);
        MonthlyExpense = recentTransactions.Where(t => !t.IsIncome).Sum(t => t.Amount);

        // Calculate savings rate
        SavingsRate = MonthlyIncome > 0 
            ? Math.Round((MonthlyIncome - MonthlyExpense) / MonthlyIncome * 100, 1) 
            : 0;

        // Load recent transactions (last 10)
        RecentTransactions = new ObservableCollection<TransactionItem>(
            transactions
                .OrderByDescending(t => t.Date)
                .Take(10)
                .Select(t => new TransactionItem(
                    t.Date.ToString("dd.MM.yyyy"),
                    t.Description,
                    t.Category,
                    t.Project,
                    t.IsIncome ? t.Amount : -t.Amount,
                    t.IsIncome ? "#00C853" : "#D50000"
                ))
        );

        // Calculate top expenses by category
        var expensesByCategory = recentTransactions
            .Where(t => !t.IsIncome)
            .GroupBy(t => t.Category)
            .Select(g => new CategoryExpense(
                g.Key,
                g.Sum(t => t.Amount),
                GetCategoryColor(g.Key)
            ))
            .OrderByDescending(c => c.Amount)
            .Take(5);

        TopExpenses = new ObservableCollection<CategoryExpense>(expensesByCategory);
    }

    private static string GetCategoryColor(string category)
    {
        var colors = new[] { "#C0C0C0", "#909090", "#808080", "#A0A0A0", "#B0B0B0" };
        var hash = Math.Abs(category.GetHashCode());
        return colors[hash % colors.Length];
    }
}

public record TransactionItem(
    string Date,
    string Description,
    string Category,
    string Project,
    decimal Amount,
    string AmountColor
);

public record CategoryExpense(
    string Name,
    decimal Amount,
    string Color
);
