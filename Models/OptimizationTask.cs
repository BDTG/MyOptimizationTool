// In folder: Models/OptimizationTask.cs
using System.Collections.Generic;

namespace MyOptimizationTool.Models
{
    public enum TaskType { Registry, Appx, Executable, Unknown }

    public class OptimizationTask
    {
        public string Name { get; set; } = string.Empty;
        public TaskType Type { get; set; } = TaskType.Unknown;

        // Dùng Dictionary để lưu các tham số linh hoạt
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}