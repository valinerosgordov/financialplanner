using System.Windows;
using NexusFinance.Models;

namespace NexusFinance.Views;

public partial class PayableEditorDialog : Window
{
    public Payable? Result { get; private set; }

    public PayableEditorDialog(Payable? existingPayable = null)
    {
        InitializeComponent();
        
        DueDateBox.SelectedDate = DateTime.Now.AddDays(7);

        if (existingPayable != null)
        {
            TitleBox.Text = existingPayable.Title;
            CreditorBox.Text = existingPayable.CreditorName;
            AmountBox.Text = existingPayable.Amount.ToString();
            DueDateBox.SelectedDate = existingPayable.DueDate;
            
            Title = "Edit Payable";
        }
        else
        {
            Title = "Add New Payable";
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var title = TitleBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            MessageBox.Show("Title is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(CreditorBox.Text))
        {
            MessageBox.Show("Creditor name is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(AmountBox.Text, out var amount) || amount <= 0)
        {
            MessageBox.Show("Invalid amount value!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (DueDateBox.SelectedDate == null)
        {
            MessageBox.Show("Due date is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result = new Payable
        {
            Title = title,
            CreditorName = CreditorBox.Text.Trim(),
            Amount = amount,
            DueDate = DueDateBox.SelectedDate.Value,
            IsPaid = false
        };

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
