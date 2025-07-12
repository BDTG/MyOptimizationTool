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
            return Task.Run(async () =>
            {
                long totalFreed = 0;
                if (!Directory.Exists(path)) return totalFreed;

                var directory = new DirectoryInfo(path);

                // Xóa file
                foreach (var file in directory.GetFiles())
                {
                    try
                    {
                        var fileSize = file.Length;
                        file.Delete();
                        totalFreed += fileSize;
                    }
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
    }
}