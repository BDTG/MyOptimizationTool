// In folder: ViewModels/SystemInfoViewModel.cs
using Microsoft.UI.Dispatching;
using MyOptimizationTool.Models;
using MyOptimizationTool.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace MyOptimizationTool.ViewModels
{
    public class SystemInfoViewModel : INotifyPropertyChanged
    {
        private readonly SystemInfoService _systemInfoService = new();
        private DispatcherTimer? _timer;
        private readonly DispatcherQueue _dispatcherQueue;

        // SỬA LỖI: Chỉ cần một đối tượng Specs duy nhất
        private ComputerSpecs? _specs;
        private SystemMetrics? _metrics;
        private bool _isLoading = true;

        public ComputerSpecs? Specs
        {
            get => _specs;
            set => SetProperty(ref _specs, value);
        }
        public SystemMetrics? Metrics
        {
            get => _metrics;
            set => SetProperty(ref _metrics, value);
        }
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

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

            _dispatcherQueue.TryEnqueue(() =>
            {
                Specs = staticSpecsData; // Gán toàn bộ đối tượng
                _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                _timer.Tick += Timer_Tick_UpdateMetrics;
                _timer.Start();
                Timer_Tick_UpdateMetrics(this, EventArgs.Empty);
                IsLoading = false;
            });
        }

        private void Timer_Tick_UpdateMetrics(object? sender, object? e)
        {
            Metrics = _systemInfoService.GetCurrentMetrics();
        }

        public void Cleanup()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= Timer_Tick_UpdateMetrics;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}