using MyOptimizationTool.Service;
using Microsoft.Extensions.Hosting;

// Nếu bạn đã tick vào "Do not use top-level statements", file của bạn sẽ có cấu trúc này
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService(options =>
            {
                options.ServiceName = "MyOptimizationTool Background Service";
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
            });
}