// In file: App.xaml.cs
using Microsoft.UI.Xaml;

namespace MyOptimizationTool
{
    public partial class App : Application
    {
        // THUỘC TÍNH NÀY BẮT BUỘC PHẢI CÓ
        public static Window? MainWindow { get; private set; }

        private Window? m_window;

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            // VÀ PHẢI ĐƯỢC GÁN GIÁ TRỊ Ở ĐÂY
            MainWindow = m_window;

            m_window.Activate();
        }
    }
}