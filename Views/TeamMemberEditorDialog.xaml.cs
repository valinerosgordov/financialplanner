using System.Windows;
using System.Windows.Controls;
using NexusFinance.Models;

namespace NexusFinance.Views;

public partial class TeamMemberEditorDialog : Window
{
    public TeamMember? Result { get; private set; }
    private readonly string _projectId;

    public TeamMemberEditorDialog(string projectId, TeamMember? existingMember = null)
    {
        InitializeComponent();
        
        _projectId = projectId;

        if (existingMember != null)
        {
            NameBox.Text = existingMember.Name;
            RoleBox.Text = existingMember.Role;
            SalaryBox.Text = existingMember.Salary.ToString();
            IsActiveBox.IsChecked = existingMember.IsActive;
            
            // Select frequency
            FrequencyBox.SelectedIndex = existingMember.PaymentFrequency switch
            {
                PaymentFrequency.Monthly => 0,
                PaymentFrequency.Hourly => 1,
                PaymentFrequency.OneTime => 2,
                _ => 0
            };
            
            Title = "Edit Team Member";
        }
        else
        {
            Title = "Add Team Member";
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var name = NameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Name is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(RoleBox.Text))
        {
            MessageBox.Show("Role is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(SalaryBox.Text, out var salary) || salary < 0)
        {
            MessageBox.Show("Invalid salary value!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var frequency = FrequencyBox.SelectedIndex switch
        {
            0 => PaymentFrequency.Monthly,
            1 => PaymentFrequency.Hourly,
            2 => PaymentFrequency.OneTime,
            _ => PaymentFrequency.Monthly
        };

        Result = new TeamMember
        {
            Name = name,
            Role = RoleBox.Text.Trim(),
            Salary = salary,
            PaymentFrequency = frequency,
            IsActive = IsActiveBox.IsChecked ?? true,
            ProjectId = _projectId
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
