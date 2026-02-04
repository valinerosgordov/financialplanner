namespace NexusFinance.Services;

/// <summary>
/// Service for calculating statistical correlations between asset price series
/// </summary>
public class CorrelationService
{
    /// <summary>
    /// Calculate Pearson Correlation Coefficient between two price series
    /// </summary>
    /// <returns>Correlation coefficient between -1.0 (inverse) and +1.0 (direct correlation)</returns>
    public double CalculatePearsonCorrelation(List<double> seriesX, List<double> seriesY)
    {
        if (seriesX.Count != seriesY.Count || seriesX.Count == 0)
            return 0;

        int n = seriesX.Count;
        
        // Calculate means
        double meanX = seriesX.Average();
        double meanY = seriesY.Average();
        
        // Calculate covariance and standard deviations
        double covariance = 0;
        double varianceX = 0;
        double varianceY = 0;
        
        for (int i = 0; i < n; i++)
        {
            double diffX = seriesX[i] - meanX;
            double diffY = seriesY[i] - meanY;
            
            covariance += diffX * diffY;
            varianceX += diffX * diffX;
            varianceY += diffY * diffY;
        }
        
        // Avoid division by zero
        if (varianceX == 0 || varianceY == 0)
            return 0;
        
        // Pearson correlation formula
        return covariance / Math.Sqrt(varianceX * varianceY);
    }
    
    /// <summary>
    /// Calculate correlation matrix for multiple assets
    /// </summary>
    public Dictionary<(string, string), double> CalculateCorrelationMatrix(
        Dictionary<string, List<double>> assetPrices)
    {
        var matrix = new Dictionary<(string, string), double>();
        var assetNames = assetPrices.Keys.ToList();
        
        for (int i = 0; i < assetNames.Count; i++)
        {
            for (int j = 0; j < assetNames.Count; j++)
            {
                var asset1 = assetNames[i];
                var asset2 = assetNames[j];
                
                if (i == j)
                {
                    // Perfect correlation with itself
                    matrix[(asset1, asset2)] = 1.0;
                }
                else if (!matrix.ContainsKey((asset2, asset1)))
                {
                    // Calculate correlation (symmetric, so only calculate once)
                    var correlation = CalculatePearsonCorrelation(
                        assetPrices[asset1], 
                        assetPrices[asset2]);
                    
                    matrix[(asset1, asset2)] = correlation;
                    matrix[(asset2, asset1)] = correlation;
                }
            }
        }
        
        return matrix;
    }
    
    /// <summary>
    /// Generate synthetic price history for demonstration
    /// </summary>
    public Dictionary<string, List<double>> GenerateSyntheticPriceData(
        List<string> assetNames, 
        int dataPoints = 30)
    {
        var random = new Random(42); // Fixed seed for consistent results
        var data = new Dictionary<string, List<double>>();
        
        // Generate base market trend
        var marketTrend = GenerateRandomWalk(dataPoints, 100, 2, random);
        
        foreach (var asset in assetNames)
        {
            var prices = new List<double>();
            var correlation = GetAssetMarketCorrelation(asset);
            
            for (int i = 0; i < dataPoints; i++)
            {
                // Mix market trend with asset-specific noise
                var marketInfluence = marketTrend[i] * correlation;
                var assetNoise = (random.NextDouble() - 0.5) * 10 * (1 - Math.Abs(correlation));
                
                prices.Add(Math.Max(10, marketInfluence + assetNoise));
            }
            
            data[asset] = prices;
        }
        
        return data;
    }
    
    private List<double> GenerateRandomWalk(int points, double start, double volatility, Random random)
    {
        var values = new List<double> { start };
        
        for (int i = 1; i < points; i++)
        {
            var change = (random.NextDouble() - 0.5) * volatility;
            values.Add(Math.Max(10, values[i - 1] + change));
        }
        
        return values;
    }
    
    private double GetAssetMarketCorrelation(string assetName)
    {
        // Simulate different correlation patterns
        return assetName.ToLower() switch
        {
            var s when s.Contains("btc") || s.Contains("bitcoin") => 0.7,
            var s when s.Contains("eth") || s.Contains("ethereum") => 0.75,
            var s when s.Contains("stock") || s.Contains("s&p") => 0.85,
            var s when s.Contains("gold") || s.Contains("commodity") => -0.3,
            var s when s.Contains("project") || s.Contains("revenue") => 0.2,
            _ => 0.5
        };
    }
}
