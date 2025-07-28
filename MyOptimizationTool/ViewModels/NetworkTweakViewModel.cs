// In folder: ViewModels/NetworkTweakViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyOptimizationTool.Services; // <-- SỬ DỤNG SERVICE CLIENT
using System.Threading.Tasks;

namespace MyOptimizationTool.ViewModels
{
    public partial class NetworkTweakViewModel : ObservableObject
    {
        // THAY THẾ: Sử dụng Client thay vì Executor
        private readonly NetworkTweakServiceClient _client = new();

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string statusText = "Sẵn sàng để tối ưu hóa mạng.";

        [RelayCommand]
        private async Task OptimizeAsync()
        {
            IsBusy = true;
            StatusText = "Đang gửi yêu cầu tối ưu mạng...";
            await _client.RequestNetworkTweak(isReset: false);
            StatusText = "Đã gửi yêu cầu! Dịch vụ nền sẽ thực hiện tối ưu.";
            IsBusy = false;
        }

        [RelayCommand]
        private async Task ResetAsync()
        {
            IsBusy = true;
            StatusText = "Đang gửi yêu cầu khôi phục mạng...";
            await _client.RequestNetworkTweak(isReset: true);
            StatusText = "Đã gửi yêu cầu! Dịch vụ nền sẽ thực hiện khôi phục.";
            IsBusy = false;
        }
    }
}