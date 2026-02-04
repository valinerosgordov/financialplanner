namespace NexusFinance.Models;

/// <summary>
/// Represents a node in the Sankey diagram (source, intermediate, or sink)
/// </summary>
public class SankeyNode
{
    public string Name { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int ColumnIndex { get; set; } // 0=Input, 1=Allocation, 2=Output
    public double Y { get; set; } // Calculated Y position
    public double Height { get; set; } // Visual height proportional to amount
}

/// <summary>
/// Represents a flow/link between two nodes
/// </summary>
public class SankeyLink
{
    public SankeyNode SourceNode { get; set; } = null!;
    public SankeyNode TargetNode { get; set; } = null!;
    public decimal Amount { get; set; }
    public double SourceY { get; set; } // Y offset in source node
    public double TargetY { get; set; } // Y offset in target node
    public double Width { get; set; } // Visual width proportional to amount
}

/// <summary>
/// Complete Sankey diagram data
/// </summary>
public class SankeyData
{
    public List<SankeyNode> Nodes { get; set; } = new();
    public List<SankeyLink> Links { get; set; } = new();
}
