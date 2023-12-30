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
        }
    }
}
