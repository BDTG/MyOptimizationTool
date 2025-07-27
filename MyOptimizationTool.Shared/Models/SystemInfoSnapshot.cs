// In project: MyOptimizationTool
// File: Models/SystemInfoSnapshot.cs
using System.Collections.Generic;

namespace MyOptimizationTool.Shared.Models
{
    // Lớp này đóng gói tất cả thông tin mà service sẽ gửi đi
    public class SystemInfoSnapshot
    {
        public SystemMetrics? Metrics { get; set; }
        public ComputerSpecs? Specs { get; set; }
        public List<DiskInfo>? Disks { get; set; }
    }
}