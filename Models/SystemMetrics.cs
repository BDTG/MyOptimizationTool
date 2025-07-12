// In folder: Models/SystemMetrics.cs
using System.Collections.Generic;

namespace MyOptimizationTool.Models
{
    public class SystemMetrics
    {
        public double RamUsedGB { get; set; }
        public double RamTotalGB { get; set; }
        public int RamUsagePercentage => RamTotalGB > 0 ? (int)(RamUsedGB / RamTotalGB * 100) : 0;

        public float CpuUsagePercentage { get; set; }
        public int CpuUsagePercentageInt => (int)CpuUsagePercentage;

        public int ProcessCount { get; set; }
        public List<DiskInfo>? Disks { get; set; }
        public List<GpuMetrics>? GpuInfo { get; set; }
    }
}