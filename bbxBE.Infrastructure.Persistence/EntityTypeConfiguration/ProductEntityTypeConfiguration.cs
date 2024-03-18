using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    //https://stackoverflow.com/questions/51568253/one-to-many-relationship-getting-error-in-entity-framework-core-when-trying-to-i
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {

            builder
            .HasMany<ProductCode>(g => g.ProductCodes)
            .WithOne(s => s.Product)
            .HasForeignKey(s => s.ProductID);

            builder
            .HasMany<Stock>(r => r.Stocks)
            .WithOne(r => r.Product)
            .HasForeignKey(s => s.ProductID)
            .IsRequired(false);

            builder.Property(u => u.UnitWeight).HasPrecision(19, 4);
            builder.Property(u => u.UnitPrice2).HasPrecision(19, 4);
            builder.Property(u => u.UnitPrice1).HasPrecision(19, 4);
            builder.Property(u => u.ProductFee).HasPrecision(19, 4);
            builder.Property(u => u.OrdUnit).HasPrecision(19, 4);
            builder.Property(u => u.MinStock).HasPrecision(19, 4);
            builder.Property(u => u.LatestSupplyPrice).HasPrecision(19, 4);
        }

    }
}
