// In folder: Models/DiskInfo.cs
namespace MyOptimizationTool.Models
{
    public class DiskInfo
    {
        public string? Name { get; set; }
        public double TotalSizeGB { get; set; }
        public double FreeSpaceGB { get; set; }
        public double UsedSpaceGB => TotalSizeGB - FreeSpaceGB;
        public int PercentUsed => TotalSizeGB > 0 ? (int)(UsedSpaceGB / TotalSizeGB * 100) : 0;
        public string UsedSpaceGBFormatted => UsedSpaceGB.ToString("N2"); // "N2" = Number with 2 decimal places
        public string TotalSizeGBFormatted => TotalSizeGB.ToString("N2");
    }
}