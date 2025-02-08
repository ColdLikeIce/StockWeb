using CommonCore.EntityFramework.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StockWorker.Db.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWorker.Db
{
    public class CarDbContext : DbContext
    {
        public CarDbContext(DbContextOptions<CarDbContext> options) : base(options)

        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddProvider(new EFLoggerProvider());
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }
            optionsBuilder.ConfigureWarnings(b => b.Ignore(CoreEventId.ContextInitialized));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* modelBuilder.Entity<AxaProductCode>()
                .HasOne(x => x.AxaPlanCode)
                .WithMany(x => x.AxaProductCode)
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.PlanId);*/
        }

        public DbSet<StockData> StockData { get; set; }
        public DbSet<StockGroup> StockGroup { get; set; }
    }
}