using CommonCore.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWorker.Service
{
    public interface IStockService : IScopedDependency
    {
        /// <summary>
        /// 根据热门地点接口进行下载
        /// </summary>
        /// <returns></returns>
        Task<bool> RunHotStock();
    }
}