// In file: Views/MainPage.xaml.cs
using Microsoft.UI.Xaml.Controls;
using MyOptimizationTool.Views;
using System.Linq;

namespace MyOptimizationTool
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            var dashboardItem = NavView.MenuItems.OfType<NavigationViewItem>().FirstOrDefault(x => (string)x.Tag == "DashboardPage");
            if (dashboardItem != null)
            {
                NavView.SelectedItem = dashboardItem;
                ContentFrame.Navigate(typeof(Views.DashboardPage));
            }
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(Views.SettingsPage));
            }
            else if (args.InvokedItemContainer?.Tag is string navItemTag)
            {
                switch (navItemTag)
                {
                    case "DashboardPage":
                        ContentFrame.Navigate(typeof(Views.DashboardPage));
                        break;
                    case "SystemInfoPage":
                        ContentFrame.Navigate(typeof(Views.SystemInfoPage));
                        break;

                    // THÊM CASE MỚI Ở ĐÂY
                    case "PlaybookEnginePage":
                        ContentFrame.Navigate(typeof(Views.PlaybookEnginePage));
                        break;

                    case "SystemTweakerPage":
                        ContentFrame.Navigate(typeof(Views.SystemTweakerPage));
                        break;
                    case "GameLauncherPage":
                        ContentFrame.Navigate(typeof(Views.GameLauncherPage));
                        break;
                    case "CleanupPage":
                        ContentFrame.Navigate(typeof(Views.SystemCleanupPage));
                        break;
                }
            }
        }
    }
}