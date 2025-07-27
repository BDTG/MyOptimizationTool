using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyOptimizationTool.Core;
using System.Threading.Tasks;

namespace MyOptimizationTool.ViewModels
{
    public partial class NetworkTweakViewModel : ObservableObject
    {
        private readonly TweakScriptExecutor _executor = new();

        [ObservableProperty] private bool isBusy;
        [ObservableProperty] private string statusText = "Sẵn sàng để tối ưu hóa mạng.";

        [RelayCommand]
        private async Task OptimizeAsync()
        {
            IsBusy = true;
            StatusText = "Đang áp dụng các tinh chỉnh mạng, vui lòng chờ...";
            await _executor.ExecuteFromFileAsync("NetworkTweaks.json");
            StatusText = "Hoàn tất! Khởi động lại máy để áp dụng tất cả thay đổi.";
            IsBusy = false;
        }
        [RelayCommand]
        private async Task ResetAsync()
        {
            IsBusy = true;
            StatusText = "Đang khôi phục cài đặt mạng mặc định của Windows...";

            // Gọi phương thức khôi phục mới
            await _executor.ExecuteNetworkResetScriptAsync();

            StatusText = "Hoàn tất khôi phục! Vui lòng khởi động lại máy.";
            IsBusy = false;
        }
    }
}