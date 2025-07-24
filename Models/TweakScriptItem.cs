using System.Collections.Generic;

namespace MyOptimizationTool.Models
{
    public class TweakScriptItem
    {
        public string Type { get; set; } = string.Empty;
        // Dành cho registry
        public string? Path { get; set; }
        public string? ValueName { get; set; }
        public object? Data { get; set; }
        public string? DataType { get; set; }
        // Dành cho script
        public string? ScriptContent { get; set; }
    }

    public class TweakScriptFile
    {
        public List<TweakScriptItem> Tweaks { get; set; } = new();
    }
}