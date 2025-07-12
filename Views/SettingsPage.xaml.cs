// In file: Views/SettingsPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.ViewModels;

namespace MyOptimizationTool.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage()
        {
            this.InitializeComponent();
            ViewModel = new SettingsViewModel();
            this.DataContext = ViewModel; // Gán ViewModel cho DataContext
        }
    }
}