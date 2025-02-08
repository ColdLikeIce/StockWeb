using Serilog;
using StockWorker.Service;

namespace StockWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    // ���ÿ�ʼʱ��ͽ���ʱ��
                    DateTime startTime = new DateTime(now.Year, now.Month, now.Day, 9, 25, 0);
                    DateTime endTime = new DateTime(now.Year, now.Month, now.Day, 11, 35, 0);
                    // ��ȡ��ǰ���ڼ� (0-6��0�������죬6��������)
                    var workday = now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday;

                    DateTime startTime2 = new DateTime(now.Year, now.Month, now.Day, 12, 59, 0);
                    DateTime endTime2 = new DateTime(now.Year, now.Month, now.Day, 15, 30, 0);
                    // �жϵ�ǰʱ���Ƿ��ڷ�Χ��
                    if (workday && (now >= startTime && now <= endTime) || (now >= startTime2 && now <= endTime2))
                    {
                        Log.Information($"��ʼִ������");
                        await StockDemo.Test();
                    }
                    await Task.Delay(10000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Log.Error($"������{ex.Message}");
                }
            }
        }
    }
}