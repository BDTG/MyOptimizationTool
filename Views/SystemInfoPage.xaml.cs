// In folder: Views/SystemInfoPage.xaml.cs
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.ViewModels; // Đảm bảo using này đúng

namespace MyOptimizationTool.Views
{
    public sealed partial class SystemInfoPage : Page
    {
        public SystemInfoViewModel ViewModel { get; }

        public SystemInfoPage()
        {
            this.InitializeComponent();
            ViewModel = new SystemInfoViewModel();
            this.DataContext = ViewModel;
        }

        // Đây là phương thức xử lý sự kiện Unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Gọi đến phương thức Cleanup trong ViewModel để dừng timer, tránh memory leak
            ViewModel.Cleanup();
        }
    }
}