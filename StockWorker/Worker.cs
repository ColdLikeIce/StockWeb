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
                    // 设置开始时间和结束时间
                    DateTime startTime = new DateTime(now.Year, now.Month, now.Day, 9, 25, 0);
                    DateTime endTime = new DateTime(now.Year, now.Month, now.Day, 11, 35, 0);
                    // 获取当前星期几 (0-6，0是星期天，6是星期六)
                    var workday = now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday;

                    DateTime startTime2 = new DateTime(now.Year, now.Month, now.Day, 12, 59, 0);
                    DateTime endTime2 = new DateTime(now.Year, now.Month, now.Day, 15, 30, 0);
                    // 判断当前时间是否在范围内
                    if (workday && (now >= startTime && now <= endTime) || (now >= startTime2 && now <= endTime2))
                    {
                        Log.Information($"开始执行任务");
                        await StockDemo.Test();
                    }
                    await Task.Delay(10000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Log.Error($"报错了{ex.Message}");
                }
            }
        }
    }
}