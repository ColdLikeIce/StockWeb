using Serilog;
using StockWorker;

public class Program
{
    public static void Main(string[] args)
    {
        // 配置日志
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("Serilog.json") // 加载 Serilog 配置文件
            .AddJsonFile($"Serilog.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        // 创建 Serilog Logger
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // 设置 Serilog 作为全局日志记录器
        Log.Logger = logger;
        Log.Information($"environment: {environment}");
        // 记录启动日志
        Log.Information("Serilog 初始化成功，日志已启动");

        // 启动主机
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "应用程序启动失败");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService() // 使用 Windows 服务
            .ConfigureServices((hostContext, services) =>
            {
                // 配置 Serilog 作为日志提供程序
                services.AddLogging(logging =>
                {
                    logging.ClearProviders(); // 清除默认日志提供程序
                    logging.AddSerilog(); // 使用 Serilog
                });

                // 配置定时服务
                services.AddHostedService<Worker>();
            })
            .UseSerilog(); // 确保 Serilog 在 HostBuilder 中被使用
}