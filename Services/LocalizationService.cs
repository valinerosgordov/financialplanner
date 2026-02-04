using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace NexusFinance.Services;

/// <summary>
/// Singleton service for managing application localization and culture settings.
/// Supports hot-swapping languages at runtime without requiring app restart.
/// </summary>
public sealed class LocalizationService : INotifyPropertyChanged
{
    private static LocalizationService? _instance;
    private static readonly object _lock = new();
    
    private CultureInfo _currentCulture;
    private readonly ResourceManager _resourceManager;
    
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? LanguageChanged;

    private LocalizationService()
    {
        _resourceManager = new ResourceManager(
            "NexusFinance.Properties.Languages.Strings", 
            typeof(LocalizationService).Assembly
        );
        
        // Load saved language preference or default to English
        var savedLanguage = Properties.Settings.Default.Language;
        _currentCulture = string.IsNullOrEmpty(savedLanguage) 
            ? new CultureInfo("en-US") 
            : new CultureInfo(savedLanguage);
        
        ApplyCulture();
    }

    public static LocalizationService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new LocalizationService();
                }
            }
            return _instance;
        }
    }

    public CultureInfo CurrentCulture => _currentCulture;

    /// <summary>
    /// Gets a localized string by key using the current culture.
    /// </summary>
    public string this[string key]
    {
        get
        {
            try
            {
                var value = _resourceManager.GetString(key, _currentCulture);
                return value ?? $"[{key}]";
            }
            catch
            {
                return $"[{key}]";
            }
        }
    }

    /// <summary>
    /// Changes the application language and updates all culture-dependent formatting.
    /// </summary>
    /// <param name="cultureCode">Culture code (e.g., "en-US", "ru-RU")</param>
    public void SetLanguage(string cultureCode)
    {
        try
        {
            var newCulture = new CultureInfo(cultureCode);
            
            if (_currentCulture.Name == newCulture.Name)
                return;
            
            _currentCulture = newCulture;
            
            // Persist user choice
            Properties.Settings.Default.Language = cultureCode;
            Properties.Settings.Default.Save();
            
            ApplyCulture();
            
            // Notify all bindings to refresh
            OnPropertyChanged(nameof(CurrentCulture));
            OnPropertyChanged("Item[]"); // Notify indexer changed
            
            LanguageChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (CultureNotFoundException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Invalid culture code: {cultureCode}. {ex.Message}");
        }
    }

    /// <summary>
    /// Applies the current culture to the thread for date/number formatting.
    /// </summary>
    private void ApplyCulture()
    {
        Thread.CurrentThread.CurrentCulture = _currentCulture;
        Thread.CurrentThread.CurrentUICulture = _currentCulture;
        
        // Apply to default thread culture for new threads
        CultureInfo.DefaultThreadCurrentCulture = _currentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = _currentCulture;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Markup extension for dynamic localization binding in XAML.
/// Usage: {loc:Translate Key=Dashboard_Title}
/// </summary>
public class TranslateExtension : System.Windows.Markup.MarkupExtension
{
    public string? Key { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Key))
            return "[NO_KEY]";

        // Create a binding to the LocalizationService indexer
        var binding = new System.Windows.Data.Binding($"[{Key}]")
        {
            Source = LocalizationService.Instance,
            Mode = System.Windows.Data.BindingMode.OneWay
        };

        // If we're in design mode or can't get target, return the key
        if (serviceProvider.GetService(typeof(System.Windows.Markup.IProvideValueTarget)) 
            is not System.Windows.Markup.IProvideValueTarget target)
        {
            return Key;
        }

        // Return the binding's value
        return binding.ProvideValue(serviceProvider);
    }
}
