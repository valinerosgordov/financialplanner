using System.Windows;
using System.Windows.Controls;
using NexusFinance.Models;

namespace NexusFinance.Views;

public partial class ReceivableEditorDialog : Window
{
    public Receivable? Result { get; private set; }

    public ReceivableEditorDialog(List<Project> projects, Receivable? existingReceivable = null)
    {
        InitializeComponent();
        
        // Populate projects dropdown
        foreach (var project in projects)
        {
            ProjectBox.Items.Add(new ComboBoxItem { Content = project.Name, Tag = project.Name });
        }
        
        ExpectedDateBox.SelectedDate = DateTime.Now.AddDays(14);

        if (existingReceivable != null)
        {
            TitleBox.Text = existingReceivable.Title;
            DebtorBox.Text = existingReceivable.DebtorName;
            AmountBox.Text = existingReceivable.Amount.ToString();
            ExpectedDateBox.SelectedDate = existingReceivable.ExpectedDate;
            
            // Select project if exists
            if (!string.IsNullOrEmpty(existingReceivable.ProjectId))
            {
                foreach (ComboBoxItem item in ProjectBox.Items)
                {
                    if (item.Tag?.ToString() == existingReceivable.ProjectId)
                    {
                        ProjectBox.SelectedItem = item;
                        break;
                    }
                }
            }
            
            // Select probability
            ProbabilityBox.SelectedIndex = existingReceivable.Probability switch
            {
                ProbabilityLevel.Confirmed => 0,
                ProbabilityLevel.Likely => 1,
                ProbabilityLevel.Uncertain => 2,
                _ => 1
            };
            
            Title = "Edit Receivable";
        }
        else
        {
            Title = "Add New Receivable";
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

        if (string.IsNullOrWhiteSpace(DebtorBox.Text))
        {
            MessageBox.Show("Debtor name is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(AmountBox.Text, out var amount) || amount <= 0)
        {
            MessageBox.Show("Invalid amount value!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (ExpectedDateBox.SelectedDate == null)
        {
            MessageBox.Show("Expected date is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selectedProject = ProjectBox.SelectedItem as ComboBoxItem;
        var projectId = selectedProject?.Tag?.ToString();
        if (selectedProject?.Content?.ToString() == "(None)")
        {
            projectId = null;
        }

        var probability = ProbabilityBox.SelectedIndex switch
        {
            0 => ProbabilityLevel.Confirmed,
            1 => ProbabilityLevel.Likely,
            2 => ProbabilityLevel.Uncertain,
            _ => ProbabilityLevel.Likely
        };

        Result = new Receivable
        {
            Title = title,
            DebtorName = DebtorBox.Text.Trim(),
            Amount = amount,
            ProjectId = projectId,
            ExpectedDate = ExpectedDateBox.SelectedDate.Value,
            Probability = probability,
            IsReceived = false
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
