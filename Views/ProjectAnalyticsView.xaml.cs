using System.Windows.Controls;
using NexusFinance.ViewModels;

namespace NexusFinance.Views;

public partial class ProjectAnalyticsView : UserControl
{
    public ProjectAnalyticsView()
    {
        InitializeComponent();
        DataContext = new ProjectAnalyticsViewModel();
    }
}
