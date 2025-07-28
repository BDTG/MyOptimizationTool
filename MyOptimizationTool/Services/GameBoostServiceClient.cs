// In project: MyOptimizationTool
// File: Services/GameBoostServiceClient.cs
using MyOptimizationTool.Shared.Models;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyOptimizationTool.Services
{
    public class GameBoostServiceClient
    {
        private const string CommandPipeName = "MyOptimizationToolCommandPipe";

        public async Task RequestBoostAndLaunch(Game game, BoostMode mode)
        {
            try
            {
                var command = new GameBoostCommand { GameToLaunch = game, Mode = mode };
                var json = JsonSerializer.Serialize(command);

                await using var clientStream = new NamedPipeClientStream(".", CommandPipeName, PipeDirection.Out);
                await clientStream.ConnectAsync(1000); // Thử kết nối trong 1 giây

                await using var writer = new StreamWriter(clientStream);
                await writer.WriteAsync(json);
                await writer.FlushAsync();
            }
            catch
            {
                // Xử lý lỗi nếu không kết nối được với service sau
            }
        }
    }
}