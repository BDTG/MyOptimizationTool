// In file: MainWindow.xaml.cs
using Microsoft.UI.Xaml;
using MyOptimizationTool.Views; // Đảm bảo using này đúng

namespace MyOptimizationTool
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "My Optimization Tool";

            // SỬA LỖI: Ra lệnh cho Frame điều hướng thẳng đến MainPage
            // ngay khi cửa sổ được tạo, bỏ qua tất cả các bước kiểm tra.
            RootFrame.Navigate(typeof(MainPage));
        }
    }
}