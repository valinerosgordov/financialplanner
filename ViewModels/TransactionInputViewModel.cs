using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using NexusFinance.Models;
using NexusFinance.Services;

namespace NexusFinance.ViewModels;

public partial class TransactionInputViewModel : ObservableObject
{
    private readonly DataService _dataService;

    [ObservableProperty]
    private bool _isIncome = false;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private DateTime _transactionDate = DateTime.Now;

    [ObservableProperty]
    private string? _selectedProject;

    [ObservableProperty]
    private string? _selectedCategory;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _projects = new();

    [ObservableProperty]
    private ObservableCollection<string> _categories = new();

    public TransactionInputViewModel()
    {
        _dataService = new DataService();
        LoadData();
    }

    [RelayCommand]
    private void SetIncome()
    {
        IsIncome = true;
        LoadCategories();
    }

    [RelayCommand]
    private void SetExpense()
    {
        IsIncome = false;
        LoadCategories();
    }

    [RelayCommand]
    private void AddTransaction()
    {
        if (string.IsNullOrWhiteSpace(Description))
        {
            MessageBox.Show("Description is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Amount <= 0)
        {
            MessageBox.Show("Amount must be greater than zero!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedCategory))
        {
            MessageBox.Show("Category is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var transaction = new Transaction
        {
            Description = Description,
            Amount = Amount,
            Date = TransactionDate,
            Category = SelectedCategory,
            Project = SelectedProject ?? "Personal",
            IsIncome = IsIncome
        };

        _dataService.AddTransaction(transaction);

        MessageBox.Show(
            $"✅ Transaction Saved!\n\n" +
            $"Type: {(IsIncome ? "Income" : "Expense")}\n" +
            $"Description: {Description}\n" +
            $"Amount: ₽{Amount:N0}\n" +
            $"Date: {TransactionDate:dd.MM.yyyy}\n" +
            $"Project: {transaction.Project}\n" +
            $"Category: {SelectedCategory}",
            "Success",
            MessageBoxButton.OK,
            MessageBoxImage.Information
        );

        // Reset form
        Description = string.Empty;
        Amount = 0;
        TransactionDate = DateTime.Now;
        Notes = string.Empty;
        LoadCategories();
    }

    private void LoadData()
    {
        var projectsList = _dataService.GetProjects();
        Projects = new ObservableCollection<string>(projectsList.Select(p => p.Name));
        
        if (Projects.Any())
        {
            SelectedProject = Projects.First();
        }

        LoadCategories();
    }

    private void LoadCategories()
    {
        var categoriesList = _dataService.GetCategories();
        var filtered = categoriesList.Where(c => 
            (IsIncome && c.Type == "Income") || (!IsIncome && c.Type == "Expense")
        );
        
        Categories = new ObservableCollection<string>(filtered.Select(c => c.Name));
        
        if (Categories.Any())
        {
            SelectedCategory = Categories.First();
        }
    }
}
