// In file: Views/GameLauncherPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.ViewModels;

namespace MyOptimizationTool.Views
{
    public sealed partial class GameLauncherPage : Page
    {
        public GameLauncherViewModel ViewModel { get; }

        public GameLauncherPage()
        {
            // DÒNG NÀY BẮT BUỘC PHẢI CÓ VÀ PHẢI NẰM ĐẦU TIÊN
            this.InitializeComponent();
           
            ViewModel = new GameLauncherViewModel();
            this.Name = "GameLauncherPageRoot";
            this.DataContext = ViewModel;
        }
    }
}