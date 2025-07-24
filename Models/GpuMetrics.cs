namespace MyOptimizationTool.Models
{
    public class GpuMetrics
    {
        public string Name { get; set; } = "N/A";

        // VRAM
        public double VramUsedMB { get; set; }
        public double VramTotalMB { get; set; }
        public int VramUsagePercentage => VramTotalMB > 0 ? (int)(VramUsedMB / VramTotalMB * 100) : 0;
        public string VramUsedMBFormatted => VramUsedMB.ToString("N0");
        public string VramTotalMBFormatted => VramTotalMB.ToString("N0");

        // THÊM MỚI: Các thuộc tính giám sát
        public float CoreLoad { get; set; } // % tải của nhân GPU
        public int CoreLoadInt => (int)CoreLoad;

        public float? Temperature { get; set; } // Nhiệt độ GPU (có thể null)
        public int TemperatureInt => Temperature.HasValue ? (int)Temperature.Value : 0;
    }
}