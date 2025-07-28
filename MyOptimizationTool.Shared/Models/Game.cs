// In folder: Models/Game.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyOptimizationTool.Shared.Models
{
    public class Game : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public string ExecutablePath { get; set; } = string.Empty;
        public string? ImagePath { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}