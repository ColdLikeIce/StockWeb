using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StockWorker.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWorker
{
    public class StockDataWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private bool isFirst = true;

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceProvider"></param>
        public StockDataWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var _domain = scope.ServiceProvider.GetRequiredService<IStockService>();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _domain.RunHotStock();
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