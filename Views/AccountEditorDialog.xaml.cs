using System.Windows;
using System.Windows.Controls;
using NexusFinance.Models;

namespace NexusFinance.Views;

public partial class AccountEditorDialog : Window
{
    public Account? Result { get; private set; }

    public AccountEditorDialog(Account? existingAccount = null)
    {
        InitializeComponent();

        if (existingAccount != null)
        {
            AccountNameBox.Text = existingAccount.Name;
            InstitutionBox.Text = existingAccount.Institution;
            BalanceBox.Text = existingAccount.Balance.ToString();
            
            // Select the type
            foreach (ComboBoxItem item in TypeBox.Items)
            {
                if (item.Content.ToString() == existingAccount.Type)
                {
                    TypeBox.SelectedItem = item;
                    break;
                }
            }
            
            Title = "Edit Account";
        }
        else
        {
            Title = "Add New Account";
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var name = AccountNameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Account name is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(InstitutionBox.Text))
        {
            MessageBox.Show("Institution is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(BalanceBox.Text, out var balance))
        {
            MessageBox.Show("Invalid balance value!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedType = ((ComboBoxItem)TypeBox.SelectedItem).Content.ToString() ?? "Checking";

        Result = new Account
        {
            Name = name,
            Institution = InstitutionBox.Text.Trim(),
            Balance = balance,
            Type = selectedType
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
