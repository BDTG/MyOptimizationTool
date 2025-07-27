// In file: Views/StartupAppsPage.xaml.cs
using Microsoft.UI.Xaml.Controls;

namespace MyOptimizationTool.Views
{
    public sealed partial class StartupAppsPage : Page
    {
        public StartupAppsPage()
        {
            this.InitializeComponent();
            // Converter BooleanToInvertedBooleanConverter đã được dùng chung nên không cần định nghĩa lại
        }
    }
}