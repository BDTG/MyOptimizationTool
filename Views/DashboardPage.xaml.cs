// In file: Views/DashboardPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.ViewModels;

namespace MyOptimizationTool.Views
{
    public sealed partial class DashboardPage : Page
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage()
        {
            this.InitializeComponent();
            ViewModel = new DashboardViewModel();
            this.DataContext = ViewModel; // Gán ViewModel cho DataContext
        }
    }
}