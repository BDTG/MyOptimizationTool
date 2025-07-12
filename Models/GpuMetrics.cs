// In folder: Models/GpuMetrics.cs
namespace MyOptimizationTool.Models
{
    public class GpuMetrics
    {
        public string Name { get; set; } = "N/A";
        public double VramUsedMB { get; set; }
        public double VramTotalMB { get; set; }

        public int VramUsagePercentage => VramTotalMB > 0 ? (int)(VramUsedMB / VramTotalMB * 100) : 0;
        public string VramUsedMBFormatted => VramUsedMB.ToString("N0"); // Số nguyên
        public string VramTotalMBFormatted => VramTotalMB.ToString("N0"); // Số nguyên
    }
}