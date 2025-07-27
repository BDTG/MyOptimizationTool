// In project: MyOptimizationTool
// File: Services/SystemInfoServiceClient.cs
using MyOptimizationTool.Models;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyOptimizationTool.Services
{
    public class SystemInfoServiceClient
    {
        private const string PipeName = "MyOptimizationToolPipe";

        // THAY ĐỔI: Trả về gói snapshot đầy đủ
        public async Task<SystemInfoSnapshot?> GetSystemInfoSnapshotAsync()
        {
            try
            {
                await using var clientStream = new NamedPipeClientStream(".", PipeName, PipeDirection.In);
                await clientStream.ConnectAsync(2000);

                using var reader = new StreamReader(clientStream, Encoding.UTF8);
                var json = await reader.ReadToEndAsync();

                return JsonSerializer.Deserialize<SystemInfoSnapshot>(json);
            }
            catch
            {
                return null;
            }
        }
    }
}