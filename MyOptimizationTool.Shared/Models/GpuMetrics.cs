// In folder: Models/GpuMetrics.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyOptimizationTool.Shared.Models
{
    public partial class GpuMetrics : ObservableObject
    {
        public string Name { get; set; } = "N/A";

        [ObservableProperty]
        private double vramUsedMB;

        [ObservableProperty]
        private double vramTotalMB;

        [ObservableProperty]
        private float coreLoad;

        [ObservableProperty]
        private float? temperature;

        // Các thuộc tính dẫn xuất
        public int VramUsagePercentage => VramTotalMB > 0 ? (int)(VramUsedMB / VramTotalMB * 100) : 0;
        public string VramUsedMBFormatted => VramUsedMB.ToString("N0");
        public string VramTotalMBFormatted => VramTotalMB.ToString("N0");
        public int CoreLoadInt => (int)CoreLoad;
        public int TemperatureInt => Temperature.HasValue ? (int)Temperature.Value : 0;

        // Các phương thức partial để thông báo cho UI
        partial void OnVramUsedMBChanged(double value) => OnPropertyChanged(nameof(VramUsagePercentage));
        partial void OnVramTotalMBChanged(double value) => OnPropertyChanged(nameof(VramUsagePercentage));
        partial void OnCoreLoadChanged(float value) => OnPropertyChanged(nameof(CoreLoadInt));
        partial void OnTemperatureChanged(float? value) => OnPropertyChanged(nameof(TemperatureInt));
    }
}