using System.Windows.Controls;
using NexusFinance.ViewModels;

namespace NexusFinance.Views;

public partial class LiquidityView : UserControl
{
    public LiquidityView()
    {
        InitializeComponent();
        DataContext = new LiquidityViewModel(new Services.DataService());
    }
}
