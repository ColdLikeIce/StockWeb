using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWorker.Db.entity
{
    public class StockData
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? Type { get; set; }
        public int? hasScanner { get; set; }
        public int? groupCount { get; set; }
        public int? Scanner { get; set; }
        public int? Sort { get; set; }
        public decimal? CostPrice { get; set; }
        public int? Count { get; set; }
        public string? Url { get; set; }
        public int? MaxType { get; set; }
        public int? MinType { get; set; }
        public decimal? InsertPrice { get; set; }
        public decimal? OutPrice { get; set; }
        public decimal? Sum { get; set; }

        public string? OutPutName { get; set; }
        public decimal? lastPrice { get; set; }
        public decimal? nowPrice { get; set; }
        public decimal? nowSum { get; set; }
    }
}