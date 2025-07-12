// In folder: Models/Playbook.cs
using System.Collections.Generic;

namespace MyOptimizationTool.Models
{
    public class Playbook
    {
        public string Name { get; set; } = "Chưa có tên";
        public string Description { get; set; } = "Chưa có mô tả";
        public string Author { get; set; } = "Không rõ tác giả";
        public string RootPath { get; set; } = string.Empty;

        // Danh sách tất cả các tác vụ đã được phân tích
        public List<OptimizationTask> Tasks { get; set; } = new();
        public List<FeaturePage> FeaturePages { get; set; } = new();
    }
}