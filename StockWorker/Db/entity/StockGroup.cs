﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWorker.Db.entity
{
    public class StockGroup
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string? Name { get; set; }
        public string? GroupName { get; set; }
        public string? GroupId { get; set; }
        public int? Type { get; set; }
    }
}