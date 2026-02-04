# NexusFinance

> A native Windows quantitative finance terminal built with WPF and .NET 8

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![WPF](https://img.shields.io/badge/Framework-WPF-0078D4)](https://github.com/dotnet/wpf)

**NexusFinance** is a professional-grade financial planning application designed for developers, entrepreneurs, and quantitative analysts who need advanced analytics and a modern, distraction-free interface.

![NexusFinance Interface](docs/screenshot.png)

---

## Features

### Financial Management
- **Multi-Project Tracking** - Separate personal and business finances with project-level P&L analysis
- **Double-Entry Ledger** - Professional-grade transaction tracking with full audit trail
- **Team & Payroll** - Manage project teams and calculate monthly labor costs
- **Liquidity Forecasting** - Track payables and receivables with urgency-based prioritization

### Advanced Analytics
- **Sankey Diagrams** - Visualize cash flow from income sources to expense categories
- **Correlation Matrix** - Analyze asset diversification and portfolio risk exposure
- **Real-Time Calculations** - Net worth, burn rate, and savings rate updated instantly

### Neural CFO (Experimental)
- **AI-Powered Analysis** - Integrate with Google Gemini for financial insights
- **Contextual Recommendations** - Get actionable advice based on your current financial state
- **Secure API Storage** - API keys encrypted using Windows DPAPI

### Internationalization
- **Multi-Language Support** - English and Russian with hot-swapping (no restart required)
- **Culture-Aware Formatting** - Automatic currency symbols, date formats, and number separators
- **Extensible** - Easy to add additional languages via `.resx` files

---

## Tech Stack

| Component | Technology |
|-----------|-----------|
| **Framework** | .NET 8 (WPF) |
| **Architecture** | MVVM with `CommunityToolkit.Mvvm` |
| **UI Library** | MaterialDesignThemes |
| **Charting** | LiveCharts2 (SkiaSharp) |
| **AI Integration** | Microsoft Semantic Kernel + Google Gemini |
| **Data Persistence** | JSON (System.Text.Json) |
| **Security** | Windows DPAPI for secrets |

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- Windows 10/11 (WPF is Windows-only)
- Visual Studio 2022 or Rider (optional, for development)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/nexusfinance.git
   cd nexusfinance
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the application**
   ```bash
   dotnet build
   ```

4. **Run**
   ```bash
   dotnet run --project NexusFinance.csproj
   ```

The application will launch with sample seed data to help you get started.

---

## Usage

### Quick Start

1. **Dashboard** - View your net worth, monthly income/expense, and savings rate
2. **Add Transaction** - Click the `+` button to record income or expenses
3. **Projects** - Create projects to separate business ventures from personal finances
4. **Wallet** - Manage accounts (checking, savings, cash) and investments (stocks, crypto)
5. **Analytics** - Explore cash flow visualizations and asset correlations
6. **Liquidity** - Track money you owe and money owed to you
7. **Settings** - Configure language and Neural CFO API key (optional)

### Data Storage

Financial data is stored locally in JSON format:
```
%APPDATA%\NexusFinance\data.json
```

### Neural CFO Setup (Optional)

To enable AI-powered financial analysis:

1. Get a free API key from [Google AI Studio](https://ai.google.dev/)
2. Open **Settings** in NexusFinance
3. Paste your API key and click **Save**
4. Navigate to **Neural CFO** to ask financial questions

---

## Architecture

### Project Structure

```
NexusFinance/
├── Models/              # Data entities (Project, Transaction, Account, etc.)
├── ViewModels/          # MVVM ViewModels with CommunityToolkit.Mvvm
├── Views/               # XAML views and user controls
├── Services/            # Business logic and data access
│   ├── DataService.cs           # JSON persistence and CRUD operations
│   ├── LocalizationService.cs   # I18N/L10N management
│   ├── GeminiAnalysisService.cs # AI integration
│   └── CorrelationService.cs    # Statistical analysis
├── Properties/
│   └── Languages/       # .resx files for localization
└── NexusFinance.csproj
```

### Design System: "Monochrome Stealth Glass"

The UI follows a minimalist, high-density design philosophy:
- **Colors:** Monochrome palette (black, grey, silver) with functional green/red for financial indicators
- **Glass Surfaces:** Semi-transparent panels with subtle borders and drop shadows
- **Typography:** Clean sans-serif fonts with a clear hierarchy
- **Density:** Maximum information in minimum space (inspired by Bloomberg Terminal)

---

## Localization

NexusFinance supports multiple languages with runtime hot-swapping:

### Supported Languages
- 🇺🇸 **English** (en-US) - Default
- 🇷🇺 **Russian** (ru-RU) - Complete

### Adding a New Language

1. Create `Properties/Languages/Strings.[culture-code].resx`
2. Translate all keys from `Strings.resx`
3. Add the language to `SettingsViewModel.cs`:
   ```csharp
   new LanguageOption { Code = "fr-FR", DisplayName = "🇫🇷 Français" }
   ```
4. Rebuild and run

See [LOCALIZATION_GUIDE.md](LOCALIZATION_GUIDE.md) for detailed instructions.

---

## Security

### API Key Storage
API keys (e.g., for Google Gemini) are encrypted using **Windows Data Protection API (DPAPI)** and stored in:
```
%APPDATA%\NexusFinance\secure\apikey.enc
```

### Best Practices
- Never commit `data.json` to version control
- Keep API keys secure and do not share `.enc` files
- The `.gitignore` is pre-configured to exclude sensitive files

---

## Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork** the repository
2. **Create a feature branch** (`git checkout -b feature/amazing-feature`)
3. **Commit your changes** (`git commit -m 'Add amazing feature'`)
4. **Push to the branch** (`git push origin feature/amazing-feature`)
5. **Open a Pull Request**

### Code Standards
- Follow C# naming conventions (PascalCase for classes/methods, camelCase for fields)
- Use `var` for local variables where type is obvious
- Add XML documentation for public APIs
- Maintain the "Monochrome Stealth Glass" design system

---

## Roadmap

- [ ] Export to Excel (ClosedXML integration)
- [ ] Recurring transactions automation
- [ ] Budget tracking and alerts
- [ ] Multi-currency support (live exchange rates)
- [ ] Mobile companion app (MAUI)
- [ ] Cloud sync (Azure/Firebase)
- [ ] Monte Carlo risk simulation
- [ ] Tax optimization recommendations

---

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- **MaterialDesignInXamlToolkit** - UI components
- **LiveCharts2** - High-performance charting
- **Microsoft Semantic Kernel** - AI integration framework
- **CommunityToolkit.Mvvm** - Modern MVVM helpers

---

## Support

- **Issues:** [GitHub Issues](https://github.com/yourusername/nexusfinance/issues)
- **Discussions:** [GitHub Discussions](https://github.com/yourusername/nexusfinance/discussions)

---

**Built with .NET 8 and WPF** | **Designed for Developers, Entrepreneurs, and Quants**
