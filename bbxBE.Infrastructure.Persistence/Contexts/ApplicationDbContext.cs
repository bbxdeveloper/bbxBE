﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using bbxBE.Application.Interfaces;
using bbxBE.Domain.Common;
using bbxBE.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace bbxBE.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly ILoggerFactory _loggerFactory;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            IDateTimeService dateTime,
            ILoggerFactory loggerFactory
            ) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dateTime = dateTime;
            _loggerFactory = loggerFactory;
        }

        public DbSet<USR_USER> USR_USER { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<ProductGroup> ProductGroup { get; set; }
        public DbSet<Origin> Origin { get; set; }
        public DbSet<ProductCode> ProductCode { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Counter> Counter { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreateTime = _dateTime.NowUtc;
                        entry.Entity.UpdateTime = entry.Entity.CreateTime;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdateTime = _dateTime.NowUtc;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var _mockData = this.Database.GetService<IMockService>();
        //    var seedPositions = _mockData.SeedPositions(1000);
        //    builder.Entity<Position>().HasData(seedPositions);

            //mock adatokkal tölthetünk itt

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
            
        }
    }
}