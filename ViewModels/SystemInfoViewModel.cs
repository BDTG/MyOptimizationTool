// In folder: ViewModels/SystemInfoViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.ApplicationInsights;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using MyOptimizationTool.Models;
using MyOptimizationTool.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyOptimizationTool.ViewModels
{
    // Lớp đã kế thừa từ ObservableObject
    public partial class SystemInfoViewModel : ObservableObject
    {
        private readonly SystemInfoService _systemInfoService = new();
        private DispatcherTimer? _timer;
        private readonly DispatcherQueue _dispatcherQueue;

        // SỬA LỖI 1: Xóa các trường private thủ công, chỉ giữ lại các thuộc tính được tạo tự động.
        [ObservableProperty]
        private ComputerSpecs? specs;

        [ObservableProperty]
        private SystemMetrics? metrics;

        [ObservableProperty]
        private bool isLoading = true;

        // Thuộc tính này đã đúng
        public ObservableCollection<DiskInfo> Disks { get; } = new();

        public SystemInfoViewModel()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _ = InitializeViewModelAsync();
        }

        private async Task InitializeViewModelAsync()
        {
            IsLoading = true;
            await _systemInfoService.InitializeAsync();

            var staticSpecsData = await _systemInfoService.GetStaticComputerSpecsAsync();
            var diskData = await _systemInfoService.GetDiskInfoAsync();
            var initialMetrics = _systemInfoService.GetCurrentMetrics();

            _dispatcherQueue.TryEnqueue(() =>
            {
                Specs = staticSpecsData;
                Metrics = initialMetrics;

                Disks.Clear();
                foreach (var disk in diskData)
                {
                    Disks.Add(disk);
                }

                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                _timer.Tick += Timer_Tick_UpdateMetrics;
                _timer.Start();

                // SỬA LỖI 2: Xóa dòng gọi không cần thiết này.
                // Dữ liệu đã được gán ở dòng "Metrics = initialMetrics", không cần cập nhật ngay lập tức.
                // Timer_Tick_UpdateMetrics(this, EventArgs.Empty); 

                IsLoading = false;
            });
        }

        private void Timer_Tick_UpdateMetrics(object? sender, object? e)
        {
            if (Metrics?.GpuInfo == null) return;

            var newMetrics = _systemInfoService.GetCurrentMetrics();

            // Cập nhật các thuộc tính, UI sẽ tự động phản hồi
            Metrics.CpuUsagePercentage = newMetrics.CpuUsagePercentage;
            Metrics.RamUsedGB = newMetrics.RamUsedGB;
            Metrics.ProcessCount = newMetrics.ProcessCount;

            // Cập nhật thông tin GPU một cách thông minh
            foreach (var newGpu in newMetrics.GpuInfo)
            {
                // Tìm GpuMetrics tương ứng trong danh sách hiện tại của ViewModel bằng tên
                var existingGpu = Metrics.GpuInfo.FirstOrDefault(g => g.Name == newGpu.Name);
                if (existingGpu != null)
                {
                    // Cập nhật từng thuộc tính của GpuMetrics đã có
                    // Chúng ta cần biến GpuMetrics thành ObservableObject để các thay đổi này cũng mượt mà
                    existingGpu.CoreLoad = newGpu.CoreLoad;
                    existingGpu.Temperature = newGpu.Temperature;
                    existingGpu.VramUsedMB = newGpu.VramUsedMB;

                    // Nếu VramTotalMB chưa có, gán một lần
                    if (existingGpu.VramTotalMB == 0 && newGpu.VramTotalMB > 0)
                    {
                        existingGpu.VramTotalMB = newGpu.VramTotalMB;
                    }
                }
            }
        }

        public void Cleanup()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= Timer_Tick_UpdateMetrics;
            }
            _systemInfoService.Cleanup();
        }

        // SỬA LỖI 3: Xóa toàn bộ phần triển khai INotifyPropertyChanged thủ công (sự kiện và SetProperty)
        // vì ObservableObject đã làm việc đó cho chúng ta.
    }
}