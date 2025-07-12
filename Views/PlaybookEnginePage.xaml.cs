using Microsoft.Extensions.DependencyInjection;  // Thêm nếu thiếu (cho GetRequiredService)
using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.ViewModels;

namespace MyOptimizationTool.Views
{
    public sealed partial class PlaybookEnginePage : Page
    {
        public PlaybookEnginePage()
        {
            this.InitializeComponent();
            // Set DataContext với DI (sẽ tự inject parserService vào constructor của ViewModel)
            this.DataContext = App.Host.Services.GetRequiredService<PlaybookEngineViewModel>();
        }
    }
}
