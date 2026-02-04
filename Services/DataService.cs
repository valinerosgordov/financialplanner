using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using NexusFinance.Models;

namespace NexusFinance.Services;

public class DataService
{
    private static readonly string DataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "NexusFinance",
        "data.json"
    );

    private AppData _data;

    public DataService()
    {
        _data = LoadData();
    }

    // Projects
    public ObservableCollection<Project> GetProjects() => new(_data.Projects);

    public void AddProject(Project project)
    {
        _data.Projects.Add(project);
        SaveData();
    }

    public void UpdateProject(string oldName, Project updatedProject)
    {
        var index = _data.Projects.FindIndex(p => p.Name == oldName);
        if (index >= 0)
        {
            _data.Projects[index] = updatedProject;
            SaveData();
        }
    }

    public void DeleteProject(string name)
    {
        _data.Projects.RemoveAll(p => p.Name == name);
        SaveData();
    }

    // Transactions
    public ObservableCollection<Transaction> GetTransactions() => new(_data.Transactions);

    public void AddTransaction(Transaction transaction)
    {
        _data.Transactions.Add(transaction);
        SaveData();
    }

    public void DeleteTransaction(Transaction transaction)
    {
        _data.Transactions.Remove(transaction);
        SaveData();
    }

    // Accounts
    public ObservableCollection<Account> GetAccounts() => new(_data.Accounts);

    public void AddAccount(Account account)
    {
        _data.Accounts.Add(account);
        SaveData();
    }

    public void UpdateAccount(string oldName, Account updatedAccount)
    {
        var index = _data.Accounts.FindIndex(a => a.Name == oldName);
        if (index >= 0)
        {
            _data.Accounts[index] = updatedAccount;
            SaveData();
        }
    }

    public void DeleteAccount(string name)
    {
        _data.Accounts.RemoveAll(a => a.Name == name);
        SaveData();
    }

    // Investments
    public ObservableCollection<Investment> GetInvestments() => new(_data.Investments);

    public void AddInvestment(Investment investment)
    {
        _data.Investments.Add(investment);
        SaveData();
    }

    public void UpdateInvestment(string oldName, Investment updatedInvestment)
    {
        var index = _data.Investments.FindIndex(i => i.Name == oldName);
        if (index >= 0)
        {
            _data.Investments[index] = updatedInvestment;
            SaveData();
        }
    }

    public void DeleteInvestment(string name)
    {
        _data.Investments.RemoveAll(i => i.Name == name);
        SaveData();
    }

    // Categories
    public ObservableCollection<Category> GetCategories() => new(_data.Categories);

    public void AddCategory(Category category)
    {
        _data.Categories.Add(category);
        SaveData();
    }

    public void DeleteCategory(string name)
    {
        _data.Categories.RemoveAll(c => c.Name == name);
        SaveData();
    }
    
    // Payables
    public List<Payable> GetPayables() => _data.Payables;
    
    public void AddPayable(Payable payable)
    {
        _data.Payables.Add(payable);
        SaveData();
    }
    
    public void UpdatePayable(string id, Payable updatedPayable)
    {
        var index = _data.Payables.FindIndex(p => p.Id == id);
        if (index != -1)
        {
            updatedPayable.Id = id;
            _data.Payables[index] = updatedPayable;
            SaveData();
        }
    }
    
    public void DeletePayable(string id)
    {
        _data.Payables.RemoveAll(p => p.Id == id);
        SaveData();
    }
    
    public void MarkPayableAsPaid(string id)
    {
        var payable = _data.Payables.FirstOrDefault(p => p.Id == id);
        if (payable != null)
        {
            payable.IsPaid = true;
            SaveData();
        }
    }
    
    // Receivables
    public List<Receivable> GetReceivables() => _data.Receivables;
    
    public void AddReceivable(Receivable receivable)
    {
        _data.Receivables.Add(receivable);
        SaveData();
    }
    
    public void UpdateReceivable(string id, Receivable updatedReceivable)
    {
        var index = _data.Receivables.FindIndex(r => r.Id == id);
        if (index != -1)
        {
            updatedReceivable.Id = id;
            _data.Receivables[index] = updatedReceivable;
            SaveData();
        }
    }
    
    public void DeleteReceivable(string id)
    {
        _data.Receivables.RemoveAll(r => r.Id == id);
        SaveData();
    }
    
    public void MarkReceivableAsReceived(string id)
    {
        var receivable = _data.Receivables.FirstOrDefault(r => r.Id == id);
        if (receivable != null)
        {
            receivable.IsReceived = true;
            SaveData();
        }
    }
    
    // Team Members
    public List<TeamMember> GetTeamMembers() => _data.TeamMembers;
    
    public List<TeamMember> GetTeamMembersByProject(string projectId)
    {
        return _data.TeamMembers
            .Where(tm => tm.ProjectId == projectId)
            .OrderByDescending(tm => tm.IsActive)
            .ThenBy(tm => tm.Name)
            .ToList();
    }
    
    public void AddTeamMember(TeamMember member)
    {
        _data.TeamMembers.Add(member);
        SaveData();
    }
    
    public void UpdateTeamMember(string id, TeamMember updatedMember)
    {
        var index = _data.TeamMembers.FindIndex(tm => tm.Id == id);
        if (index != -1)
        {
            updatedMember.Id = id;
            _data.TeamMembers[index] = updatedMember;
            SaveData();
        }
    }
    
    public void DeleteTeamMember(string id)
    {
        _data.TeamMembers.RemoveAll(tm => tm.Id == id);
        SaveData();
    }
    
    public void ToggleTeamMemberStatus(string id)
    {
        var member = _data.TeamMembers.FirstOrDefault(tm => tm.Id == id);
        if (member != null)
        {
            member.IsActive = !member.IsActive;
            SaveData();
        }
    }
    
    public decimal CalculateProjectPayroll(string projectId)
    {
        return _data.TeamMembers
            .Where(tm => tm.ProjectId == projectId && tm.IsActive)
            .Sum(tm => tm.MonthlyCost);
    }

    private AppData LoadData()
    {
        try
        {
            if (File.Exists(DataPath))
            {
                var json = File.ReadAllText(DataPath);
                return JsonSerializer.Deserialize<AppData>(json) ?? CreateDefaultData();
            }
        }
        catch
        {
            // If loading fails, return default data
        }

        return CreateDefaultData();
    }

    private void SaveData()
    {
        try
        {
            var directory = Path.GetDirectoryName(DataPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(_data, options);
            File.WriteAllText(DataPath, json);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Failed to save data: {ex.Message}", "Error", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private static AppData CreateDefaultData()
    {
        return new AppData
        {
            Projects = new List<Project>
            {
                new() { Name = "NexusAI", Revenue = 250000, Cost = 135000, Description = "AI Platform Project" },
                new() { Name = "FinSync", Revenue = 180000, Cost = 95000, Description = "Financial Sync Service" },
                new() { Name = "Personal", Revenue = 0, Cost = 65000, Description = "Personal Expenses" }
            },
            Transactions = new List<Transaction>
            {
                new() { Description = "Salary - NexusAI", Amount = 125000, Date = DateTime.Now, Category = "Income", Project = "NexusAI", IsIncome = true },
                new() { Description = "AWS Cloud Services", Amount = 12500, Date = DateTime.Now.AddDays(-1), Category = "Infrastructure", Project = "NexusAI", IsIncome = false }
            },
            Accounts = new List<Account>
            {
                new() { Name = "Checking Account", Institution = "Sberbank", Balance = 450000, Type = "Checking" },
                new() { Name = "Savings", Institution = "Tinkoff", Balance = 1200000, Type = "Savings" }
            },
            Investments = new List<Investment>
            {
                new() { Name = "Bitcoin", Type = "Crypto", Amount = 500000, CurrentValue = 800000 },
                new() { Name = "TSLA Stock", Type = "Stock", Amount = 300000, CurrentValue = 350000 }
            },
            Categories = new List<Category>
            {
                new() { Name = "Income", Type = "Income", Icon = "💰" },
                new() { Name = "Salary", Type = "Income", Icon = "💵" },
                new() { Name = "Infrastructure", Type = "Expense", Icon = "☁️" },
                new() { Name = "Food", Type = "Expense", Icon = "🍕" },
                new() { Name = "Rent", Type = "Expense", Icon = "🏠" },
                new() { Name = "Transport", Type = "Expense", Icon = "🚗" }
            }
        };
    }
}

public class AppData
{
    public List<Project> Projects { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
    public List<Account> Accounts { get; set; } = new();
    public List<Investment> Investments { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Payable> Payables { get; set; } = new();
    public List<Receivable> Receivables { get; set; } = new();
    public List<TeamMember> TeamMembers { get; set; } = new();
}
