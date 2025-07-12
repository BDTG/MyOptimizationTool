// In file: Views/PlaybookEnginePage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.ViewModels; // Đảm bảo using này đúng

namespace MyOptimizationTool.Views
{
    public sealed partial class PlaybookEnginePage : Page
    {
        // Tạo một thuộc tính để giữ ViewModel
        public PlaybookEngineViewModel ViewModel { get; }

        public PlaybookEnginePage()
        {
            this.InitializeComponent();

            // SỬA LỖI Ở ĐÂY:
            // 1. Tạo một đối tượng ViewModel mới
            ViewModel = new PlaybookEngineViewModel();

            // 2. Đặt DataContext của trang này thành đối tượng ViewModel đó.
            // Đây là bước quan trọng nhất để {Binding} hoạt động.
            this.DataContext = ViewModel;
        }
    }
}