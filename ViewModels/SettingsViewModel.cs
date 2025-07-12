// In folder: ViewModels/SettingsViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;

namespace MyOptimizationTool.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private int selectedThemeIndex;

        public SettingsViewModel()
        {
            if (App.MainWindow?.Content is FrameworkElement root)
            {
                SelectedThemeIndex = (int)root.ActualTheme - 1;
                if (SelectedThemeIndex < 0) SelectedThemeIndex = 2;
            }
        }

        partial void OnSelectedThemeIndexChanged(int value)
        {
            if (App.MainWindow?.Content is FrameworkElement rootElement)
            {
                switch (value)
                {
                    case 0: rootElement.RequestedTheme = ElementTheme.Light; break;
                    case 1: rootElement.RequestedTheme = ElementTheme.Dark; break;
                    case 2: rootElement.RequestedTheme = ElementTheme.Default; break;
                }
            }
        }
    }
}