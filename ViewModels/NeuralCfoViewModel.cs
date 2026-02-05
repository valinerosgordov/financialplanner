using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusFinance.Services;
using System.Collections.ObjectModel;

namespace NexusFinance.ViewModels;

/// <summary>
/// Neural CFO ViewModel - AI-powered financial analysis using Google Gemini.
/// Implements Clean Architecture with dependency injection.
/// </summary>
public partial class NeuralCfoViewModel : ObservableObject
{
    private readonly IAiService _aiService;
    private readonly SecureStorageService _secureStorage;
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _userQuestion = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ChatMessage> _chatHistory = new();

    [ObservableProperty]
    private bool _isAnalyzing;

    [ObservableProperty]
    private bool _isConfigured;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public NeuralCfoViewModel() 
        : this(ServiceContainer.Instance.AiService, ServiceContainer.Instance.SecureStorageService, ServiceContainer.Instance.DataService)
    {
    }

    public NeuralCfoViewModel(IAiService aiService, SecureStorageService secureStorage, IDataService dataService)
    {
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        
        try
        {
            CheckConfiguration();
            
            // Add welcome message
            if (IsConfigured)
            {
                ChatHistory.Add(new ChatMessage(
                    "🤖 Neural CFO",
                    "I'm your ruthless financial advisor. Ask me to analyze your finances, identify risks, or suggest optimizations. Be specific.",
                    false
                ));
            }
            else
            {
                ChatHistory.Add(new ChatMessage(
                    "⚠️ System",
                    "Neural CFO is not configured. Please go to Settings and add your Google Gemini API key.",
                    false
                ));
            }
        }
        catch (Exception ex)
        {
            GlobalExceptionHandler.Instance.LogError(ex, "NeuralCfoViewModel.Constructor");
        }
    }

    [RelayCommand]
    private async Task SendQuestion()
    {
        if (string.IsNullOrWhiteSpace(UserQuestion))
        {
            return;
        }

        if (!IsConfigured)
        {
            ChatHistory.Add(new ChatMessage(
                "⚠️ Error",
                "Please configure your API key in Settings first.",
                false
            ));
            return;
        }

        // Add user message
        var question = UserQuestion;
        ChatHistory.Add(new ChatMessage("You", question, true));
        UserQuestion = string.Empty;

        IsAnalyzing = true;
        StatusMessage = "🤔 Analyzing your finances...";

        try
        {
            // Calculate real financial metrics
            var (netWorth, monthlyIncome, monthlyExpense) = CalculateFinancialMetrics();
            
            // Build financial context from real data
            var context = FinancialContextBuilder.BuildSimpleContext(
                netWorth: netWorth,
                monthlyIncome: monthlyIncome,
                monthlyExpense: monthlyExpense
            );

            // Get AI analysis
            var response = await _aiService.AnalyzeFinancialContextAsync(context, question);

            // Add AI response
            ChatHistory.Add(new ChatMessage("🤖 Neural CFO", response, false));
            StatusMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ChatHistory.Add(new ChatMessage(
                "❌ Error",
                $"Analysis failed: {ex.Message}",
                false
            ));
            StatusMessage = string.Empty;
        }
        finally
        {
            IsAnalyzing = false;
        }
    }

    [RelayCommand]
    private void ClearHistory()
    {
        ChatHistory.Clear();
        ChatHistory.Add(new ChatMessage(
            "🤖 Neural CFO",
            "Chat cleared. What would you like to know?",
            false
        ));
    }

    [RelayCommand]
    private void AskSample(string sampleQuestion)
    {
        UserQuestion = sampleQuestion;
    }

    private void CheckConfiguration()
    {
        IsConfigured = _aiService.IsConfigured;
        
        if (!IsConfigured)
        {
            StatusMessage = "⚠️ Not configured - Go to Settings";
        }
    }
    
    private (decimal netWorth, decimal monthlyIncome, decimal monthlyExpense) CalculateFinancialMetrics()
    {
        var accounts = _dataService.GetAccounts();
        var transactions = _dataService.GetTransactions().ToList();
        var investments = _dataService.GetInvestments();
        
        // Calculate net worth (accounts + investments)
        var totalAccounts = accounts.Sum(a => a.Balance);
        var totalInvestments = investments.Sum(i => i.CurrentValue);
        var netWorth = totalAccounts + totalInvestments;
        
        // Calculate monthly income and expense (last 30 days)
        var thirtyDaysAgo = DateTime.Now.AddDays(-30);
        var recentTransactions = transactions.Where(t => t.Date >= thirtyDaysAgo).ToList();
        
        var monthlyIncome = recentTransactions
            .Where(t => t.Amount > 0)
            .Sum(t => t.Amount);
            
        var monthlyExpense = Math.Abs(recentTransactions
            .Where(t => t.Amount < 0)
            .Sum(t => t.Amount));
        
        return (netWorth, monthlyIncome, monthlyExpense);
    }
}

public record ChatMessage(string Sender, string Message, bool IsUser);
