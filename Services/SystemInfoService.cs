using MyOptimizationTool.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace MyOptimizationTool.Services
{
    public class SystemInfoService
    {
        private PerformanceCounter? _cpuCounter;
        private ManagementObject? _ramObject;
        private double _totalRamGB;

        // PHƯƠNG THỨC NÀY ĐÃ BỊ THIẾU
        public Task InitializeAsync()
        {
            return InitializePerformanceCountersAsync();
        }

        // PHƯƠNG THỨC NÀY CŨNG BỊ THIẾU
        public Task<ComputerSpecs> GetStaticComputerSpecsAsync()
        {
            return Task.Run(() =>
            {
                var gpus = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController")
                    .Get().OfType<ManagementObject>()
                    .Select(obj => obj["Name"]?.ToString()?.Trim() ?? "N/A").ToList();

                // SỬA LẠI TÊN CHO ĐÚNG VỚI MODEL MỚI
                return new ComputerSpecs
                {
                    OsVersion = GetWmiProperty("Win32_OperatingSystem", "Caption"),
                    Cpu = GetWmiProperty("Win32_Processor", "Name"),
                    Gpus = gpus,
                    Motherboard = $"{GetWmiProperty("Win32_BaseBoard", "Manufacturer")} {GetWmiProperty("Win32_BaseBoard", "Product")}"
                };
            });
        }

        private string GetWmiProperty(string wmiClass, string wmiProperty, string wmiProperty2 = "")
        {
            try
            {
                var searcher = new ManagementObjectSearcher($"SELECT {wmiProperty}{(string.IsNullOrEmpty(wmiProperty2) ? "" : $", {wmiProperty2}")} FROM {wmiClass}");
                var obj = searcher.Get().OfType<ManagementObject>().FirstOrDefault();
                if (obj == null) return "N/A";

                var value1 = obj[wmiProperty]?.ToString()?.Trim() ?? "N/A";
                var value2 = string.IsNullOrEmpty(wmiProperty2) ? "" : obj[wmiProperty2]?.ToString()?.Trim() ?? "";
                return $"{value1} {value2}".Trim();
            }
            catch { return "Lỗi"; }
        }

        private Task InitializePerformanceCountersAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    _cpuCounter.NextValue();

                    var ramSearcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                    var ramObj = ramSearcher.Get().OfType<ManagementObject>().FirstOrDefault();
                    if (ramObj != null)
                        _totalRamGB = Math.Round(Convert.ToUInt64(ramObj["TotalPhysicalMemory"]) / (1024.0 * 1024.0 * 1024.0), 2);

                    var osSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                    _ramObject = osSearcher.Get().OfType<ManagementObject>().FirstOrDefault();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR Initializing PerfCounters] {ex.Message}");
                }
            });
        }

        public SystemMetrics GetCurrentMetrics()
        {
            try
            {
                float cpuUsage = _cpuCounter?.NextValue() ?? 0;
                double freeRamMB = _ramObject != null ? Convert.ToDouble(_ramObject["FreePhysicalMemory"]) : 0;
                double usedRamGB = _totalRamGB - (freeRamMB / 1024.0 / 1024.0);

                var diskInfos = DriveInfo.GetDrives().Where(d => d.IsReady).Select(drive => new DiskInfo
                {
                    Name = drive.Name,
                    TotalSizeGB = drive.TotalSize / (1024.0 * 1024.0 * 1024.0),
                    FreeSpaceGB = drive.TotalFreeSpace / (1024.0 * 1024.0 * 1024.0),
                }).ToList();

                return new SystemMetrics
                {
                    CpuUsagePercentage = cpuUsage,
                    ProcessCount = Process.GetProcesses().Length,
                    RamTotalGB = _totalRamGB,
                    RamUsedGB = Math.Round(usedRamGB, 2),
                    Disks = diskInfos,
                    GpuInfo = new List<GpuMetrics>()
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR GetCurrentMetrics] {ex.Message}");
                return new SystemMetrics();
            }
        }
    }
}