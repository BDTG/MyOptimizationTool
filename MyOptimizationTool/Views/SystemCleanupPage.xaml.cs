// In file: Views/SystemCleanupPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.ViewModels;

namespace MyOptimizationTool.Views
{
    public sealed partial class SystemCleanupPage : Page
    {
        public SystemCleanupViewModel ViewModel { get; }

        public SystemCleanupPage()
        {
            this.InitializeComponent();
            ViewModel = new SystemCleanupViewModel();
            this.DataContext = ViewModel;
        }
    }
}