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
        // ������־
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("Serilog.json") // ���� Serilog �����ļ�
            .AddJsonFile($"Serilog.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        // ���� Serilog Logger
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // ���� Serilog ��Ϊȫ����־��¼��
        Log.Logger = logger;
        Log.Information($"environment: {environment}");
        // ��¼������־
        Log.Information("Serilog ��ʼ���ɹ�����־������");

        // ��������
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Ӧ�ó�������ʧ��");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService() // ʹ�� Windows ����
            .ConfigureServices((hostContext, services) =>
            {
                // ���� Serilog ��Ϊ��־�ṩ����
                services.AddLogging(logging =>
                {
                    logging.ClearProviders(); // ���Ĭ����־�ṩ����
                    logging.AddSerilog(); // ʹ�� Serilog
                });
                //services.AddAutoIoc(typeof(ISingletonDependency), LifeCycle.Singleton);
                // ���ö�ʱ����
                services.AddAutoIoc(typeof(IScopedDependency), LifeCycle.Scoped);

                services.AddRedis(hostContext.Configuration);
                services.AddHostedService<StockDataWorker>();

                //services.AddHostedService<Worker>();
                services.AddHyTripEntityFramework<CarDbContext>(options =>
                {
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("CarRentalDb"));
                });
            })
            .UseSerilog(); // ȷ�� Serilog �� HostBuilder �б�ʹ��
}