using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using bbxBE.Application.Interfaces;
using bbxBE.Domain.Common;
using bbxBE.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

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
        public DbSet<VatRate> VatRate { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceLine> InvoiceLine { get; set; }
        public DbSet<SummaryByVatRate> SummaryByVatRate { get; set; }
        public DbSet<AdditionalInvoiceData> AdditionalInvoiceData { get; set; }
        public DbSet<AdditionalInvoiceLineData> AdditionalInvoiceLineData { get; set; }
        public DbSet<Offer> Offer { get; set; }
        public DbSet<OfferLine> OfferLine { get; set; }
        public DbSet<Stock> Stock { get; set; }

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

           
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
  
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
    }
}