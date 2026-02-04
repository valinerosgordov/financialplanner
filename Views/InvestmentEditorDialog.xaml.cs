using System.Windows;
using System.Windows.Controls;
using NexusFinance.Models;

namespace NexusFinance.Views;

public partial class InvestmentEditorDialog : Window
{
    public Investment? Result { get; private set; }

    public InvestmentEditorDialog(Investment? existingInvestment = null)
    {
        InitializeComponent();
        
        // Set default date
        PurchaseDateBox.SelectedDate = DateTime.Now;

        if (existingInvestment != null)
        {
            InvestmentNameBox.Text = existingInvestment.Name;
            AmountBox.Text = existingInvestment.Amount.ToString();
            CurrentValueBox.Text = existingInvestment.CurrentValue.ToString();
            PurchaseDateBox.SelectedDate = existingInvestment.PurchaseDate;
            
            // Select the type
            foreach (ComboBoxItem item in TypeBox.Items)
            {
                if (item.Content.ToString() == existingInvestment.Type)
                {
                    TypeBox.SelectedItem = item;
                    break;
                }
            }
            
            Title = "Edit Investment";
        }
        else
        {
            Title = "Add New Investment";
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var name = InvestmentNameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Investment name is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(AmountBox.Text, out var amount) || amount <= 0)
        {
            MessageBox.Show("Invalid invested amount!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(CurrentValueBox.Text, out var currentValue) || currentValue < 0)
        {
            MessageBox.Show("Invalid current value!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedType = ((ComboBoxItem)TypeBox.SelectedItem).Content.ToString() ?? "Stock";

        Result = new Investment
        {
            Name = name,
            Type = selectedType,
            Amount = amount,
            CurrentValue = currentValue,
            PurchaseDate = PurchaseDateBox.SelectedDate ?? DateTime.Now
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
