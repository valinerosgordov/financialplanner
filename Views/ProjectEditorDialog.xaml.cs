using System.Windows;
using NexusFinance.Models;

namespace NexusFinance.Views;

public partial class ProjectEditorDialog : Window
{
    public Project? Result { get; private set; }
    private readonly string? _originalName;

    public ProjectEditorDialog(Project? existingProject = null)
    {
        InitializeComponent();

        if (existingProject != null)
        {
            _originalName = existingProject.Name;
            ProjectNameBox.Text = existingProject.Name;
            DescriptionBox.Text = existingProject.Description;
            RevenueBox.Text = existingProject.Revenue.ToString();
            CostBox.Text = existingProject.Cost.ToString();
            
            Title = "Edit Project";
        }
        else
        {
            Title = "Add New Project";
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var name = ProjectNameBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Project name is required!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(RevenueBox.Text, out var revenue))
        {
            MessageBox.Show("Invalid revenue value!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(CostBox.Text, out var cost))
        {
            MessageBox.Show("Invalid cost value!", "Validation Error", 
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Result = new Project
        {
            Name = name,
            Description = DescriptionBox.Text.Trim(),
            Revenue = revenue,
            Cost = cost
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
