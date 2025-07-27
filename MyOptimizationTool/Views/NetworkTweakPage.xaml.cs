using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.ViewModels;

namespace MyOptimizationTool.Views
{
    public sealed partial class NetworkTweakPage : Page
    {
        public NetworkTweakViewModel ViewModel { get; }

        public NetworkTweakPage()
        {
            this.InitializeComponent();
            ViewModel = new NetworkTweakViewModel();
            this.DataContext = ViewModel;
        }
    }
}