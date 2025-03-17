using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWorker.Db.entity
{
    public class DateLog
    {
        [Key]
        public int Id { get; set; }

        public DateTime? date { get; set; }
        public decimal? sum { get; set; }
        public decimal? DiffPrice { get; set; }
    }
}