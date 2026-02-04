using System.Windows.Controls;
using NexusFinance.ViewModels;

namespace NexusFinance.Views;

public partial class WalletView : UserControl
{
    public WalletView()
    {
        InitializeComponent();
        DataContext = new WalletViewModel();
    }
}
