// In folder: Models/DashboardInfo.cs
using System;
using System.Collections.Generic;

namespace MyOptimizationTool.Shared.Models
{
    // Lớp này chứa thông tin cho từng phiên bản trong changelog
    public class ChangelogEntry
    {
        public string Version { get; set; } = string.Empty;
        public List<string> Changes { get; set; } = new();
    }

    // Lớp chính chứa tất cả thông tin của Dashboard
    public class DashboardInfo
    {
        public string AppVersion { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string CopyrightNotice { get; set; } = string.Empty;
        public string UpdateMessageTitle { get; set; } = string.Empty;
        public string UpdateMessageContent { get; set; } = string.Empty;
        public List<ChangelogEntry> Changelog { get; set; } = new();
    }
}