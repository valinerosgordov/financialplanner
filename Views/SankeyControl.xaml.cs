using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using NexusFinance.Models;

namespace NexusFinance.Views;

public partial class SankeyControl : UserControl
{
    public static readonly DependencyProperty SankeyDataProperty =
        DependencyProperty.Register(
            nameof(SankeyData),
            typeof(SankeyData),
            typeof(SankeyControl),
            new PropertyMetadata(null, OnSankeyDataChanged));

    public SankeyData? SankeyData
    {
        get => (SankeyData?)GetValue(SankeyDataProperty);
        set => SetValue(SankeyDataProperty, value);
    }

    private const double NodeWidth = 40;
    private const double ColumnSpacing = 250;

    public SankeyControl()
    {
        InitializeComponent();
    }

    private static void OnSankeyDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SankeyControl control && e.NewValue is SankeyData data)
        {
            control.RenderSankey(data);
        }
    }

    private void RenderSankey(SankeyData data)
    {
        SankeyCanvas.Children.Clear();

        if (data.Nodes.Count == 0)
        {
            // Show "No Data" message
            var noDataText = new TextBlock
            {
                Text = "No transaction data available for Sankey visualization",
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080")),
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Canvas.SetLeft(noDataText, 200);
            Canvas.SetTop(noDataText, 280);
            SankeyCanvas.Children.Add(noDataText);
            return;
        }

        // Draw links first (so they appear behind nodes)
        foreach (var link in data.Links)
        {
            DrawLink(link);
        }

        // Draw nodes on top
        foreach (var node in data.Nodes)
        {
            DrawNode(node);
        }
    }

    private void DrawNode(SankeyNode node)
    {
        var x = GetNodeX(node.ColumnIndex);
        var y = node.Y;
        var height = node.Height;

        // Node rectangle
        var rect = new Rectangle
        {
            Width = NodeWidth,
            Height = height,
            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#303030")),
            Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#606060")),
            StrokeThickness = 1,
            RadiusX = 4,
            RadiusY = 4
        };

        Canvas.SetLeft(rect, x);
        Canvas.SetTop(rect, y);
        SankeyCanvas.Children.Add(rect);

        // Node label
        var label = new TextBlock
        {
            Text = node.Name,
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0E0E0")),
            FontSize = 12,
            FontWeight = FontWeights.SemiBold
        };

        var labelX = node.ColumnIndex switch
        {
            0 => x - label.ActualWidth - 10, // Left of node
            2 => x + NodeWidth + 10, // Right of node
            _ => x + NodeWidth / 2 - 20 // Center (allocation node)
        };

        Canvas.SetLeft(label, labelX);
        Canvas.SetTop(label, y + height / 2 - 8);
        SankeyCanvas.Children.Add(label);

        // Amount label
        var amountLabel = new TextBlock
        {
            Text = $"₽{node.TotalAmount:N0}",
            Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C0C0C0")),
            FontSize = 10
        };

        Canvas.SetLeft(amountLabel, labelX);
        Canvas.SetTop(amountLabel, y + height / 2 + 8);
        SankeyCanvas.Children.Add(amountLabel);
    }

    private void DrawLink(SankeyLink link)
    {
        var sourceX = GetNodeX(link.SourceNode.ColumnIndex) + NodeWidth;
        var sourceY = link.SourceNode.Y + link.SourceY + link.Width / 2;

        var targetX = GetNodeX(link.TargetNode.ColumnIndex);
        var targetY = link.TargetNode.Y + link.TargetY + link.Width / 2;

        // Create Bezier curve
        var path = new Path
        {
            Stroke = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)), // Semi-transparent white
            StrokeThickness = link.Width,
            Fill = null,
            Opacity = 0.3
        };

        var geometry = new PathGeometry();
        var figure = new PathFigure
        {
            StartPoint = new Point(sourceX, sourceY - link.Width / 2)
        };

        // Top curve
        var controlPoint1 = new Point(sourceX + ColumnSpacing / 2, sourceY - link.Width / 2);
        var controlPoint2 = new Point(targetX - ColumnSpacing / 2, targetY - link.Width / 2);
        figure.Segments.Add(new BezierSegment(
            controlPoint1,
            controlPoint2,
            new Point(targetX, targetY - link.Width / 2),
            true));

        // Line to bottom
        figure.Segments.Add(new LineSegment(new Point(targetX, targetY + link.Width / 2), true));

        // Bottom curve back
        var controlPoint3 = new Point(targetX - ColumnSpacing / 2, targetY + link.Width / 2);
        var controlPoint4 = new Point(sourceX + ColumnSpacing / 2, sourceY + link.Width / 2);
        figure.Segments.Add(new BezierSegment(
            controlPoint3,
            controlPoint4,
            new Point(sourceX, sourceY + link.Width / 2),
            true));

        figure.IsClosed = true;
        geometry.Figures.Add(figure);

        path.Data = geometry;
        path.Fill = new SolidColorBrush(Color.FromArgb(40, 200, 200, 200)); // Subtle grey fill

        // Hover effect
        path.MouseEnter += (s, e) => path.Opacity = 0.6;
        path.MouseLeave += (s, e) => path.Opacity = 0.3;

        SankeyCanvas.Children.Add(path);
    }

    private double GetNodeX(int columnIndex)
    {
        return columnIndex * ColumnSpacing + 50;
    }
}
