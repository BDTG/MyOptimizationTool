// In folder: ViewModels/StartupAppsViewModel.cs
// ViewModel sẽ được xây dựng chi tiết sau
using MyOptimizationTool.Core;
using MyOptimizationTool.Models;
using System.Collections.ObjectModel;

namespace MyOptimizationTool.ViewModels
{
    public class StartupAppsViewModel
    {
        public ObservableCollection<StartupItem> StartupItems { get; } = new();
        private readonly StartupService _startupService = new();
    }
}