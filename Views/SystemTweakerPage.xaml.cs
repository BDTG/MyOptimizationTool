// In file: Views/SystemTweakerPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.ViewModels;

namespace MyOptimizationTool.Views
{
    public sealed partial class SystemTweakerPage : Page
    {
        public SystemTweakerViewModel ViewModel { get; }

        public SystemTweakerPage()
        {
            this.InitializeComponent();
            ViewModel = new SystemTweakerViewModel();
            this.DataContext = ViewModel;
        }
    }
}