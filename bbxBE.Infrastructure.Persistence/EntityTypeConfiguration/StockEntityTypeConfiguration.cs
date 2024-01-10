using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    //https://stackoverflow.com/questions/51568253/one-to-many-relationship-getting-error-in-entity-framework-core-when-trying-to-i
    //https://learn.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key

    public class StockEntityTypeConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {

            builder
                .HasOne<Warehouse>(w => w.Warehouse)
                .WithMany(s => s.Stocks)
                .HasForeignKey(s => s.WarehouseID)
                .IsRequired(true);

            builder
                .HasOne<Location>(l => l.Location)
                .WithMany(s => s.Stocks)
                .HasForeignKey(s => s.LocationID)
                .IsRequired(false);

            builder
                .HasOne<Product>(p => p.Product)
                .WithMany(s => s.Stocks)
                .HasForeignKey(s => s.ProductID)
                .IsRequired(true);

        }
    }
}
