using CommonCore;
using CommonCore.Dependency;
using CommonCore.Enum;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StockWorker;
using StockWorker.Db;

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
                //services.AddAutoIoc(typeof(ISingletonDependency), LifeCycle.Singleton);
                // 配置定时服务
                services.AddAutoIoc(typeof(IScopedDependency), LifeCycle.Scoped);

                services.AddRedis(hostContext.Configuration);
                services.AddHostedService<StockDataWorker>();

                //services.AddHostedService<Worker>();
                services.AddHyTripEntityFramework<CarDbContext>(options =>
                {
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("CarRentalDb"));
                });
            })
            .UseSerilog(); // 确保 Serilog 在 HostBuilder 中被使用
}