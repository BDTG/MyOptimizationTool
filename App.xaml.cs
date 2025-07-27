// In file: App.xaml.cs
using Microsoft.UI.Xaml;
using MyOptimizationTool.Services;

namespace MyOptimizationTool
{
    public partial class App : Application
    {
        // Thuộc tính này để lưu trữ cửa sổ chính, rất quan trọng
        public static Window? MainWindow { get; private set; }
        

        private Window? m_window;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Tạo một cửa sổ MainWindow mới
            m_window = new MainWindow();

            // Gán nó vào thuộc tính static để các phần khác có thể truy cập
            MainWindow = m_window;

            // Kích hoạt cửa sổ
            m_window.Activate();            
        }
    }
}