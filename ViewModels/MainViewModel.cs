using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace NexusFinance.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private string _activeView = "Dashboard";

    public MainViewModel()
    {
        // Start with Dashboard
        CurrentView = new DashboardViewModel();
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        CurrentView = new DashboardViewModel();
        ActiveView = "Dashboard";
    }

    [RelayCommand]
    private void NavigateToProjects()
    {
        CurrentView = new ProjectAnalyticsViewModel();
        ActiveView = "Projects";
    }

    [RelayCommand]
    private void NavigateToWallet()
    {
        CurrentView = new WalletViewModel();
        ActiveView = "Wallet";
    }

    [RelayCommand]
    private void ShowTransactionInput()
    {
        CurrentView = new TransactionInputViewModel();
        ActiveView = "Transaction";
    }

    [RelayCommand]
    private void NavigateToAnalytics()
    {
        CurrentView = new AnalyticsViewModel(new Services.DataService());
        ActiveView = "Analytics";
    }

    [RelayCommand]
    private void NavigateToLiquidity()
    {
        CurrentView = new LiquidityViewModel(new Services.DataService());
        ActiveView = "Liquidity";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = new SettingsViewModel();
        ActiveView = "Settings";
    }

    [RelayCommand]
    private void NavigateToNeuralCfo()
    {
        CurrentView = new NeuralCfoViewModel();
        ActiveView = "AI";
    }
}
