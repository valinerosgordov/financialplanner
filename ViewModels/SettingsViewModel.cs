using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NexusFinance.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace NexusFinance.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly SecureStorageService _secureStorage;
    private readonly LocalizationService _localization;

    [ObservableProperty]
    private string? _apiKey;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<LanguageOption> _availableLanguages;

    [ObservableProperty]
    private LanguageOption? _selectedLanguage;

    public SettingsViewModel()
    {
        _secureStorage = new SecureStorageService();
        _localization = LocalizationService.Instance;

        // Load saved API key (if exists)
        _apiKey = _secureStorage.LoadApiKey();

        // Initialize language options
        _availableLanguages = new ObservableCollection<LanguageOption>
        {
            new LanguageOption { Code = "en-US", DisplayName = "🇺🇸 English" },
            new LanguageOption { Code = "ru-RU", DisplayName = "🇷🇺 Русский" }
        };

        // Set current language selection
        var currentCode = _localization.CurrentCulture.Name;
        _selectedLanguage = _availableLanguages.FirstOrDefault(l => l.Code == currentCode) 
                            ?? _availableLanguages[0];
    }

    [RelayCommand]
    private void SaveApiKey()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            StatusMessage = "⚠️ API Key cannot be empty";
            return;
        }

        try
        {
            _secureStorage.SaveApiKey(ApiKey);
            StatusMessage = "✅ API Key saved securely";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void TestConnection()
    {
        StatusMessage = "🔄 Testing connection... (Feature in development)";
    }

    partial void OnSelectedLanguageChanged(LanguageOption? value)
    {
        if (value == null || value.Code == _localization.CurrentCulture.Name)
            return;

        try
        {
            _localization.SetLanguage(value.Code);
            
            // Show confirmation message
            var message = value.Code == "ru-RU"
                ? "✅ Язык изменён на Русский. Интерфейс обновлён!"
                : "✅ Language changed to English. UI refreshed!";
            
            MessageBox.Show(message, "Language Changed", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Error changing language: {ex.Message}";
        }
    }
}

public class LanguageOption
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
