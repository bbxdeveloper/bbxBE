using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    //https://stackoverflow.com/questions/51568253/one-to-many-relationship-getting-error-in-entity-framework-core-when-trying-to-i
    //https://learn.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key

    public class StockCardEntityTypeConfiguration : IEntityTypeConfiguration<StockCard>
    {
        public void Configure(EntityTypeBuilder<StockCard> builder)
        {


            builder.Property(u => u.UnitPrice).HasPrecision(19, 4);
            builder.Property(u => u.ORealQty).HasPrecision(19, 4);
            builder.Property(u => u.OAvgCost).HasPrecision(19, 4);
            builder.Property(u => u.NRealQty).HasPrecision(19, 4);
            builder.Property(u => u.NAvgCost).HasPrecision(19, 4);


        }
    }
}
