// In project: MyOptimizationTool.Service
// File: Worker.cs
using MyOptimizationTool.Models;
using MyOptimizationTool.Services;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;

namespace MyOptimizationTool.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SystemInfoService _infoService;
        private const string PipeName = "MyOptimizationToolPipe";

        // Dữ liệu tĩnh chỉ cần lấy một lần
        private ComputerSpecs? _staticSpecs;
        private List<DiskInfo>? _staticDisks;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _infoService = new SystemInfoService();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _infoService.InitializeAsync();
            // Lấy dữ liệu tĩnh ngay khi service khởi động
            _staticSpecs = await _infoService.GetStaticComputerSpecsAsync();
            _staticDisks = await _infoService.GetDiskInfoAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MyOptimizationTool Service running.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var serverStream = new NamedPipeServerStream(PipeName, PipeDirection.Out);
                    await serverStream.WaitForConnectionAsync(stoppingToken);

                    // Đóng gói tất cả dữ liệu vào một đối tượng snapshot
                    var snapshot = new
                    {
                        Metrics = _infoService.GetCurrentMetrics(),
                        Specs = _staticSpecs,
                        Disks = _staticDisks
                    };

                    var json = JsonSerializer.Serialize(snapshot);
                    var buffer = Encoding.UTF8.GetBytes(json);
                    await serverStream.WriteAsync(buffer, stoppingToken);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in worker service.");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}