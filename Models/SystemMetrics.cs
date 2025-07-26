// In folder: Models/SystemMetrics.cs
using CommunityToolkit.Mvvm.ComponentModel; // THÊM USING NÀY
using System.Collections.Generic;
using System.Collections.ObjectModel; // THÊM USING NÀY

namespace MyOptimizationTool.Models
{
    // THAY ĐỔI: Kế thừa từ ObservableObject
    public partial class SystemMetrics : ObservableObject
    {
        // THAY ĐỔI: Chuyển các thuộc tính động thành [ObservableProperty]
        [ObservableProperty]
        private double ramUsedGB;

        [ObservableProperty]

        private float cpuUsagePercentage;

        [ObservableProperty]
        private int processCount;

        // GpuInfo cần là ObservableCollection để có thể thêm/xóa/cập nhật item
        [ObservableProperty]
        private ObservableCollection<GpuMetrics> gpuInfo = new();

        // Các thuộc tính tĩnh và dẫn xuất không cần thay đổi
        public double RamTotalGB { get; set; } // Sẽ được gán một lần
        public int RamUsagePercentage => RamTotalGB > 0 ? (int)(RamUsedGB / RamTotalGB * 100) : 0;
        public int CpuUsagePercentageInt => (int)CpuUsagePercentage;
        partial void OnCpuUsagePercentageChanged(float value)
        {
            // Khi nó được gọi, chúng ta chủ động thông báo rằng
            // thuộc tính 'CpuUsagePercentageInt' cũng cần được cập nhật trên UI.
            OnPropertyChanged(nameof(CpuUsagePercentageInt));
        }
    }
}