using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWorker.Db.entity
{
    public class InsertStock
    {
        [Key]
        public int Id { get; set; }

        public string? Code { get; set; }
        public string? OutPutName { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}