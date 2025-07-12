// In folder: Models/FeaturePage.cs
using System.Collections.Generic;

namespace MyOptimizationTool.Models
{
    // --- Lớp cơ sở cho tất cả các loại trang ---
    public abstract class FeaturePage
    {
        public string Description { get; set; } = string.Empty;
        public string? TopLineText { get; set; }
        public string? BottomLineText { get; set; }
        public string? BottomLineLink { get; set; }
        public bool IsRequired { get; set; } = true;
    }

    // --- Lớp cơ sở cho các lựa chọn ---
    public class PageOption
    {
        public string Text { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    // --- Các loại trang cụ thể ---
    public class RadioPage : FeaturePage
    {
        public List<PageOption> Options { get; set; } = new();
        public string DefaultOption { get; set; } = string.Empty;
    }

    public class CheckboxPage : FeaturePage
    {
        public List<PageOption> Options { get; set; } = new();
    }

    public class RadioImagePage : FeaturePage
    {
        public List<RadioImageOption> Options { get; set; } = new();
        public string DefaultOption { get; set; } = string.Empty;
        public bool CheckDefaultBrowser { get; set; }
        public string? DependsOn { get; set; }
    }

    // --- Lớp lựa chọn đặc biệt cho RadioImagePage ---
    public class RadioImageOption : PageOption
    {
        public string FileName { get; set; } = string.Empty;
        public string? GradientTopColor { get; set; }
        public string? GradientBottomColor { get; set; }
    }
}