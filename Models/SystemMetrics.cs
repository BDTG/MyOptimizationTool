// In folder: Models/SystemMetrics.cs
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace MyOptimizationTool.Models
{
    public partial class SystemMetrics : ObservableObject
    {
        [ObservableProperty]
        private double ramUsedGB;

        [ObservableProperty]
        private float cpuUsagePercentage;

        [ObservableProperty]
        private int processCount;

        [ObservableProperty]
        private ObservableCollection<GpuMetrics> gpuInfo = new();

        // Các thuộc tính dẫn xuất
        public double RamTotalGB { get; set; }
        public int RamUsagePercentage => RamTotalGB > 0 ? (int)(RamUsedGB / RamTotalGB * 100) : 0;
        public int CpuUsagePercentageInt => (int)CpuUsagePercentage;

        // Các phương thức partial để thông báo cho UI
        partial void OnRamUsedGBChanged(double value) => OnPropertyChanged(nameof(RamUsagePercentage));
        partial void OnCpuUsagePercentageChanged(float value) => OnPropertyChanged(nameof(CpuUsagePercentageInt));
    }
}