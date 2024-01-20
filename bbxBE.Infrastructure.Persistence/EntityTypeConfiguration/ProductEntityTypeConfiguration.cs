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


        }

    }
}
