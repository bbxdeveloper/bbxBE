using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    //https://stackoverflow.com/questions/51568253/one-to-many-relationship-getting-error-in-entity-framework-core-when-trying-to-i
    public class OfferLineEntityTypeConfiguration : IEntityTypeConfiguration<OfferLine>
    {
        public void Configure(EntityTypeBuilder<OfferLine> builder)
        {
            //soon

            builder.Property(u => u.VatPercentage).HasPrecision(19, 4);
            builder.Property(u => u.UnitPrice).HasPrecision(19, 4);
            builder.Property(u => u.UnitGross).HasPrecision(19, 4);
            builder.Property(u => u.Quantity).HasPrecision(19, 4);
            builder.Property(u => u.OriginalUnitPriceHUF).HasPrecision(19, 4);
            builder.Property(u => u.OriginalUnitPrice).HasPrecision(19, 4);
            builder.Property(u => u.Discount).HasPrecision(19, 4);
        }
    }
}
