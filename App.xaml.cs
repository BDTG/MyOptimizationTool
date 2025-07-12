using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using MyOptimizationTool.Core;  // Để đăng ký services như PlaybookParserService
using MyOptimizationTool.ViewModels;  // Để đăng ký ViewModels

namespace MyOptimizationTool
{
    public partial class App : Application
    {
        // THUỘC TÍNH NÀY BẮT BUỘC PHẢI CÓ
        public static Window? MainWindow { get; private set; }

        // Thêm: Host cho DI container (để inject services toàn app)
        public static IHost? Host { get; private set; }

        private Window? m_window;

        public App()
        {
            this.InitializeComponent();

            // Setup DI container
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Đăng ký services (singleton để reuse instance)
                    services.AddSingleton<PlaybookParserService>();
                    // Đăng ký thêm services khác nếu có (e.g., services.AddSingleton<RegistryManager>();)

                    // Đăng ký ViewModels (transient để tạo mới mỗi lần)
                    services.AddTransient<PlaybookEngineViewModel>(sp => new PlaybookEngineViewModel(sp.GetRequiredService<PlaybookParserService>()));

                    // Thêm các ViewModels khác tương tự (e.g., services.AddTransient<DashboardViewModel>();)
                })
                .Build();
        }

        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // Start host async (để services ready)
            await Host!.StartAsync();

            m_window = new MainWindow();

            // VÀ PHẢI ĐƯỢC GÁN GIÁ TRỊ Ở ĐÂY
            MainWindow = m_window;

            m_window.Activate();
        }
    }
}
