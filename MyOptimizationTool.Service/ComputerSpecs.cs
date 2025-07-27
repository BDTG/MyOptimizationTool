// In folder: Models/ComputerSpecs.cs
using System.Collections.Generic;

namespace MyOptimizationTool.Models
{
    public class ComputerSpecs
    {
        // SỬA LẠI TÊN CHO ĐÚNG CHUẨN
        public string? OsVersion { get; set; }
        public string? Cpu { get; set; }
        public List<string>? Gpus { get; set; }
        public string? Motherboard { get; set; }
    }
}