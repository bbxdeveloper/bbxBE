using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    public class AdditionalInvoiceDataEntityTypeConfiguration : IEntityTypeConfiguration<AdditionalInvoiceData>
    {
        public void Configure(EntityTypeBuilder<AdditionalInvoiceData> builder)
        {
            builder.HasOne(aid => aid.Invoice)
                    .WithMany(invoice => invoice.AdditionalInvoiceData)
                    .HasForeignKey(aid => aid.InvoiceID)
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
