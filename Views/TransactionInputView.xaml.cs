using System.Windows.Controls;
using NexusFinance.ViewModels;

namespace NexusFinance.Views;

public partial class TransactionInputView : UserControl
{
    public TransactionInputView()
    {
        InitializeComponent();
        DataContext = new TransactionInputViewModel();
    }
}
