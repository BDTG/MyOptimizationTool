// In project: MyOptimizationTool.Service
// File: Worker.cs
using MyOptimizationTool.Service.Core; // Cần using này
using MyOptimizationTool.Shared.Models;
using MyOptimizationTool.Shared.Models.Commands;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MyOptimizationTool.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly SystemInfoService _infoService;
        private readonly GameBoostService _boostService;
        private readonly NetworkTweakService _networkTweakService;
        private const string InfoPipeName = "MyOptimizationToolInfoPipe";
        private const string CommandPipeName = "MyOptimizationToolCommandPipe";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _infoService = new SystemInfoService();
            _boostService = new GameBoostService();
            _networkTweakService = new NetworkTweakService();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _infoService.InitializeAsync();
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service running.");
            await Task.WhenAll(
                RunSystemInfoProvider(stoppingToken),
                RunCommandListener(stoppingToken)
            );
        }

        private async Task RunSystemInfoProvider(CancellationToken stoppingToken)
        {
            var staticSpecs = await _infoService.GetStaticComputerSpecsAsync();
            var staticDisks = await _infoService.GetDiskInfoAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var serverStream = new NamedPipeServerStream(InfoPipeName, PipeDirection.Out);
                    await serverStream.WaitForConnectionAsync(stoppingToken);

                    var snapshot = new SystemInfoSnapshot
                    {
                        Metrics = _infoService.GetCurrentMetrics(),
                        Specs = staticSpecs,
                        Disks = staticDisks
                    };
                    var json = JsonSerializer.Serialize(snapshot);
                    var buffer = Encoding.UTF8.GetBytes(json);
                    await serverStream.WriteAsync(buffer, stoppingToken);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in SystemInfoProvider.");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private async Task RunCommandListener(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var serverStream = new NamedPipeServerStream(CommandPipeName, PipeDirection.In);
                    await serverStream.WaitForConnectionAsync(stoppingToken);

                    using var reader = new StreamReader(serverStream);
                    var jsonCommand = await reader.ReadToEndAsync();

                    var commandNode = JsonNode.Parse(jsonCommand);
                    if (commandNode?["GameToLaunch"] != null)
                    {
                        var command = JsonSerializer.Deserialize<GameBoostCommand>(jsonCommand);
                        _logger.LogInformation($"Received boost request for '{command.GameToLaunch.Name}' with mode '{command.Mode}'.");
                        _ = _boostService.OptimizeAndLaunch(command.GameToLaunch, command.Mode);
                    }
                    else if (commandNode?["IsReset"] != null)
                    {
                        var command = JsonSerializer.Deserialize<NetworkTweakCommand>(jsonCommand);
                        _ = _networkTweakService.Execute(command.IsReset); // Giả sử có service này
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in CommandListener.");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }
    }
}