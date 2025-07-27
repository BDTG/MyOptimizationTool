// In folder: ViewModels/SystemInfoViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using MyOptimizationTool.Services;
using MyOptimizationTool.Shared.Models; // <-- SỬ DỤNG NAMESPACE MỚI
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MyOptimizationTool.ViewModels
{
    public partial class SystemInfoViewModel : ObservableObject
    {
        private readonly SystemInfoServiceClient _client = new();
        private DispatcherTimer? _timer;
        private readonly DispatcherQueue _dispatcherQueue;

        [ObservableProperty] private ComputerSpecs? specs;
        [ObservableProperty] private SystemMetrics? metrics;
        [ObservableProperty] private bool isLoading = true;
        public ObservableCollection<DiskInfo> Disks { get; } = new();

        public SystemInfoViewModel()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            _timer.Tick += async (s, e) => await UpdateDataAsync();
            _ = LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            IsLoading = true;
            await UpdateDataAsync(); // Lấy dữ liệu lần đầu
            _timer?.Start();
            IsLoading = false;
        }

        private async Task UpdateDataAsync()
        {
            var snapshot = await _client.GetSystemInfoSnapshotAsync();
            if (snapshot != null)
            {
                _dispatcherQueue.TryEnqueue(() =>
                {
                    // Gán dữ liệu tĩnh (chỉ cần gán lần đầu)
                    if (Specs == null && snapshot.Specs != null) Specs = snapshot.Specs;
                    if (Disks.Count == 0 && snapshot.Disks != null)
                    {
                        foreach (var disk in snapshot.Disks) Disks.Add(disk);
                    }

                    // Cập nhật dữ liệu động
                    if (snapshot.Metrics != null)
                    {
                        if (Metrics == null) Metrics = snapshot.Metrics;
                        else
                        {
                            Metrics.CpuUsagePercentage = snapshot.Metrics.CpuUsagePercentage;
                            Metrics.RamUsedGB = snapshot.Metrics.RamUsedGB;
                            Metrics.ProcessCount = snapshot.Metrics.ProcessCount;
                            Metrics.RamTotalGB = snapshot.Metrics.RamTotalGB;
                            // Cập nhật GPU, v.v...
                        }
                    }
                });
            }
        }
        public void Cleanup() { _timer?.Stop(); }
    }
}