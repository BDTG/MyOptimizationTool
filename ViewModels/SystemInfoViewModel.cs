// In folder: ViewModels/SystemInfoViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class SystemInfoViewModel : ObservableObject
    {
        private readonly SystemInfoService _systemInfoService = App.SystemInfoServiceInstance;
        private DispatcherTimer? _timer;
        private readonly DispatcherQueue _dispatcherQueue;

        [ObservableProperty]
        private ComputerSpecs? specs;

        [ObservableProperty]
        private SystemMetrics? metrics;

        [ObservableProperty]
        private bool isLoading = true;

        public ObservableCollection<DiskInfo> Disks { get; } = new();

        public SystemInfoViewModel()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            IsLoading = true;

            // Lấy dữ liệu tĩnh một lần
            var staticSpecsData = await _systemInfoService.GetStaticComputerSpecsAsync();
            var diskData = await _systemInfoService.GetDiskInfoAsync();

            // Lấy dữ liệu động lần đầu
            var initialMetrics = _systemInfoService.GetCurrentMetrics();

            _dispatcherQueue.TryEnqueue(() =>
            {
                Specs = staticSpecsData;

                // Gán dữ liệu lần đầu, KHÔNG GHI ĐÈ LẠI
                Metrics = initialMetrics;

                Disks.Clear();
                foreach (var disk in diskData)
                {
                    Disks.Add(disk);
                }

                // Bắt đầu timer để cập nhật các giá trị sau đó
                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                _timer.Tick += Timer_Tick_UpdateMetrics;
                _timer.Start();

                IsLoading = false;
            });
        }

        private void Timer_Tick_UpdateMetrics(object? sender, object? e)
        {
            if (Metrics == null) return;

            var newMetrics = _systemInfoService.GetCurrentMetrics();

            // Cập nhật các thuộc tính chính
            Metrics.CpuUsagePercentage = newMetrics.CpuUsagePercentage;
            Metrics.RamUsedGB = newMetrics.RamUsedGB;
            Metrics.ProcessCount = newMetrics.ProcessCount;

            // Cập nhật thông tin GPU một cách thông minh
            foreach (var newGpu in newMetrics.GpuInfo)
            {
                var existingGpu = Metrics.GpuInfo.FirstOrDefault(g => g.Name == newGpu.Name);
                if (existingGpu != null)
                {
                    // Giờ đây các thay đổi này sẽ tự động cập nhật UI một cách mượt mà
                    existingGpu.CoreLoad = newGpu.CoreLoad;
                    existingGpu.Temperature = newGpu.Temperature;
                    existingGpu.VramUsedMB = newGpu.VramUsedMB;

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
            // Không cần gọi _systemInfoService.Cleanup() ở đây nữa
            // vì service sẽ sống cùng ứng dụng.
        }
    }
}