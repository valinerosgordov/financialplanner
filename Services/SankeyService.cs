using NexusFinance.Models;

namespace NexusFinance.Services;

/// <summary>
/// Service for generating Sankey diagram data from transaction flows
/// </summary>
public class SankeyService
{
    private const double NodeWidth = 40;
    private const double ColumnSpacing = 250;
    private const double NodeSpacing = 20;
    
    /// <summary>
    /// Generate Sankey diagram data from transactions
    /// </summary>
    public SankeyData GenerateSankeyFromTransactions(
        List<Transaction> transactions,
        List<Account> accounts,
        double canvasHeight = 600)
    {
        var data = new SankeyData();
        
        // Group transactions by source (income) and category (expense)
        var incomeBySource = transactions
            .Where(t => t.IsIncome)
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Category) ? "Other Income" : t.Category)
            .Select(g => new { Source = g.Key, Amount = g.Sum(t => t.Amount) })
            .OrderByDescending(x => x.Amount)
            .ToList();
        
        var expenseByCategory = transactions
            .Where(t => !t.IsIncome)
            .GroupBy(t => string.IsNullOrWhiteSpace(t.Category) ? "Other Expense" : t.Category)
            .Select(g => new { Category = g.Key, Amount = g.Sum(t => t.Amount) })
            .OrderByDescending(x => x.Amount)
            .ToList();
        
        // Calculate total flow
        var totalIncome = incomeBySource.Sum(x => x.Amount);
        var totalExpense = expenseByCategory.Sum(x => x.Amount);
        var totalFlow = Math.Max(totalIncome, totalExpense);
        
        if (totalFlow == 0)
        {
            // Return empty diagram if no data
            return data;
        }
        
        // Create INPUT nodes (income sources)
        double currentY = 0;
        foreach (var income in incomeBySource)
        {
            var height = (double)(income.Amount / totalFlow) * (canvasHeight - NodeSpacing * incomeBySource.Count);
            data.Nodes.Add(new SankeyNode
            {
                Name = income.Source,
                TotalAmount = income.Amount,
                ColumnIndex = 0,
                Y = currentY,
                Height = height
            });
            currentY += height + NodeSpacing;
        }
        
        // Create ALLOCATION node (middle)
        var allocationNode = new SankeyNode
        {
            Name = "Cash Flow",
            TotalAmount = totalFlow,
            ColumnIndex = 1,
            Y = (canvasHeight - (double)(totalFlow / totalFlow) * canvasHeight) / 2,
            Height = (double)(totalFlow / totalFlow) * canvasHeight * 0.8
        };
        data.Nodes.Add(allocationNode);
        
        // Create OUTPUT nodes (expense categories)
        currentY = 0;
        foreach (var expense in expenseByCategory)
        {
            var height = (double)(expense.Amount / totalFlow) * (canvasHeight - NodeSpacing * expenseByCategory.Count);
            data.Nodes.Add(new SankeyNode
            {
                Name = expense.Category,
                TotalAmount = expense.Amount,
                ColumnIndex = 2,
                Y = currentY,
                Height = height
            });
            currentY += height + NodeSpacing;
        }
        
        // Create links: Input -> Allocation
        double sourceOffset = 0;
        foreach (var inputNode in data.Nodes.Where(n => n.ColumnIndex == 0))
        {
            data.Links.Add(new SankeyLink
            {
                SourceNode = inputNode,
                TargetNode = allocationNode,
                Amount = inputNode.TotalAmount,
                SourceY = 0,
                TargetY = sourceOffset,
                Width = inputNode.Height
            });
            sourceOffset += inputNode.Height;
        }
        
        // Create links: Allocation -> Output
        double targetOffset = 0;
        foreach (var outputNode in data.Nodes.Where(n => n.ColumnIndex == 2))
        {
            data.Links.Add(new SankeyLink
            {
                SourceNode = allocationNode,
                TargetNode = outputNode,
                Amount = outputNode.TotalAmount,
                SourceY = targetOffset,
                TargetY = 0,
                Width = outputNode.Height
            });
            targetOffset += outputNode.Height;
        }
        
        return data;
    }
    
    /// <summary>
    /// Calculate X position for a node based on column index
    /// </summary>
    public double GetNodeX(int columnIndex)
    {
        return columnIndex * ColumnSpacing + 50;
    }
    
    /// <summary>
    /// Get node width (constant for all nodes)
    /// </summary>
    public double GetNodeWidth()
    {
        return NodeWidth;
    }
}
