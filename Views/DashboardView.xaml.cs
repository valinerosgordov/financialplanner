using System.Windows.Controls;
using NexusFinance.ViewModels;

namespace NexusFinance.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        DataContext = new DashboardViewModel();
    }
}
