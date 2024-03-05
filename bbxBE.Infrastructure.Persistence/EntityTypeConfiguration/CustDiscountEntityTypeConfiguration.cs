using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    public class CustDiscountEntityTypeConfiguration : IEntityTypeConfiguration<CustDiscount>
    {
        public void Configure(EntityTypeBuilder<CustDiscount> builder)
        {

            builder.HasOne(cd => cd.Customer)
                     .WithMany(cust => cust.CustDiscounts)
                     .HasForeignKey(cd => cd.CustomerID)
                     .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(cd => cd.ProductGroup)
                     .WithMany(pg => pg.CustDiscounts)
                     .HasForeignKey(cd => cd.ProductGroupID)
                     .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
