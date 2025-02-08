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
    }
}