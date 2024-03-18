using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    //https://stackoverflow.com/questions/51568253/one-to-many-relationship-getting-error-in-entity-framework-core-when-trying-to-i
    public class OfferEntityTypeConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder
            .HasMany<OfferLine>(g => g.OfferLines)
            .WithOne(c => c.Offer)
            .HasForeignKey(s => s.OfferID)
            .IsRequired(true);


            builder.Property(u => u.ExchangeRate).HasPrecision(19, 4);

        }
    }
}
