using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NexusFinance.ViewModels;

namespace NexusFinance.Views;

public partial class AnalyticsView : UserControl
{
    private AnalyticsViewModel? _viewModel;

    public AnalyticsView()
    {
        InitializeComponent();
        DataContext = new AnalyticsViewModel(new Services.DataService());
        _viewModel = DataContext as AnalyticsViewModel;
        
        Loaded += AnalyticsView_Loaded;
    }

    private void AnalyticsView_Loaded(object sender, RoutedEventArgs e)
    {
        if (_viewModel != null)
        {
            RenderCorrelationMatrix();
        }
    }

    private void RenderCorrelationMatrix()
    {
        if (_viewModel == null || _viewModel.AssetNames.Count == 0)
            return;

        CorrelationGrid.Children.Clear();
        CorrelationGrid.RowDefinitions.Clear();
        CorrelationGrid.ColumnDefinitions.Clear();

        var assetCount = _viewModel.AssetNames.Count;
        var cellSize = 70;

        // Add header row and column
        CorrelationGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
        CorrelationGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });

        // Add columns for each asset
        for (int i = 0; i < assetCount; i++)
        {
            CorrelationGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(cellSize) });
            CorrelationGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(cellSize) });
        }

        // Column headers
        for (int i = 0; i < assetCount; i++)
        {
            var headerText = new TextBlock
            {
                Text = TruncateAssetName(_viewModel.AssetNames[i]),
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0C0C0")),
                FontSize = 10,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };
            Grid.SetRow(headerText, 0);
            Grid.SetColumn(headerText, i + 1);
            CorrelationGrid.Children.Add(headerText);
        }

        // Row headers
        for (int i = 0; i < assetCount; i++)
        {
            var headerText = new TextBlock
            {
                Text = _viewModel.AssetNames[i],
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0C0C0")),
                FontSize = 11,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetRow(headerText, i + 1);
            Grid.SetColumn(headerText, 0);
            CorrelationGrid.Children.Add(headerText);
        }

        // Correlation cells
        foreach (var cell in _viewModel.CorrelationMatrix)
        {
            var row = _viewModel.AssetNames.IndexOf(cell.Asset1) + 1;
            var col = _viewModel.AssetNames.IndexOf(cell.Asset2) + 1;

            if (row < 1 || col < 1)
                continue;

            var border = new Border
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(cell.BackgroundColor)),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#404040")),
                BorderThickness = new Thickness(1),
                Margin = new Thickness(1),
                CornerRadius = new CornerRadius(2),
                ToolTip = cell.TooltipText
            };

            var valueText = new TextBlock
            {
                Text = cell.DisplayValue,
                Foreground = GetTextColor(cell.Correlation),
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            border.Child = valueText;
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            CorrelationGrid.Children.Add(border);
        }
    }

    private string TruncateAssetName(string name)
    {
        return name.Length > 8 ? name.Substring(0, 8) + "..." : name;
    }

    private SolidColorBrush GetTextColor(double correlation)
    {
        // Use lighter text for darker cells, darker text for lighter cells
        var absCorr = Math.Abs(correlation);
        var color = absCorr > 0.6 ? "#E0E0E0" : "#909090";
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    }
}
