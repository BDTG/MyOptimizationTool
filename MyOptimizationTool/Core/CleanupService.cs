// In folder: Core/CleanupService.cs
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyOptimizationTool.Core
{
    public class CleanupService
    {
        // Tính toán dung lượng của một thư mục (bất đồng bộ)
        public Task<long> GetDirectorySizeAsync(string path)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (!Directory.Exists(path)) return 0;
                    return new DirectoryInfo(path).GetFiles("*", SearchOption.AllDirectories).Sum(fi => fi.Length);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Could not calculate size for {path}: {ex.Message}");
                    return 0;
                }
            });
        }

        // Dọn dẹp một thư mục (bất đồng bộ)
        public Task<long> CleanDirectoryAsync(string path)
        {
            if (path.Equals("$Recycle.Bin", StringComparison.OrdinalIgnoreCase))
            {
                return ClearRecycleBinAsync();
            }

            return Task.Run(async () =>
            {
                long totalFreed = 0;
                if (!Directory.Exists(path)) return totalFreed;

                var directory = new DirectoryInfo(path);

                // Xóa file
                foreach (var file in directory.GetFiles())
                {
                    try { file.Delete(); totalFreed += file.Length; }
                    catch (Exception ex) { Debug.WriteLine($"Could not delete file {file.FullName}: {ex.Message}"); }
                }

                // Xóa thư mục con
                foreach (var subDirectory in directory.GetDirectories())
                {
                    try
                    {
                        var dirSize = await GetDirectorySizeAsync(subDirectory.FullName); // Lấy dung lượng trước khi xóa
                        subDirectory.Delete(true);
                        totalFreed += dirSize;
                    }
                    catch (Exception ex) { Debug.WriteLine($"Could not delete directory {subDirectory.FullName}: {ex.Message}"); }
                }

                return totalFreed;
            });
        }
        private Task<long> ClearRecycleBinAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    // Lấy tổng dung lượng của thùng rác trước khi dọn
                    // Đây là một cách ước tính, không hoàn toàn chính xác nhưng đủ dùng
                    long size = 0;
                    var drive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));
                    var recycleBinPath = Path.Combine(drive.Name, "$Recycle.Bin");
                    if(Directory.Exists(recycleBinPath))
                    {
                        size = new DirectoryInfo(recycleBinPath).GetFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
                    }

                    // Dùng PowerShell để dọn dẹp một cách an toàn
                    var startInfo = new ProcessStartInfo("powershell.exe")
                    {
                        Arguments = "-NoProfile -Command \"Clear-RecycleBin -Force\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        Verb = "runas"
                    };
                    Process.Start(startInfo)?.WaitForExit();
                    Debug.WriteLine("Recycle Bin cleared.");
                    return size;
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"Failed to clear Recycle Bin: {ex.Message}");
                    return 0;
                }
            });
        }
    }
}