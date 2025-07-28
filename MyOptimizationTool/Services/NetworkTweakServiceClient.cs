// In project: MyOptimizationTool
// File: Services/NetworkTweakServiceClient.cs
using MyOptimizationTool.Shared.Models.Commands;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyOptimizationTool.Services
{
    public class NetworkTweakServiceClient
    {
        private const string CommandPipeName = "MyOptimizationToolCommandPipe";

        public async Task RequestNetworkTweak(bool isReset)
        {
            try
            {
                var command = new NetworkTweakCommand { IsReset = isReset };
                var json = JsonSerializer.Serialize(command);

                await using var clientStream = new NamedPipeClientStream(".", CommandPipeName, PipeDirection.Out);
                await clientStream.ConnectAsync(1000);

                await using var writer = new StreamWriter(clientStream);
                await writer.WriteAsync(json);
                await writer.FlushAsync();
            }
            catch
            {
                // Xử lý lỗi sau
            }
        }
    }
}