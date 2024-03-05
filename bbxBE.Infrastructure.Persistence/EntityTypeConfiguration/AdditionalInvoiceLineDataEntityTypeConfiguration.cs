using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    public class AdditionalInvoiceLineDataEntityTypeConfiguration : IEntityTypeConfiguration<AdditionalInvoiceLineData>
    {
        public void Configure(EntityTypeBuilder<AdditionalInvoiceLineData> builder)
        {
            builder.HasOne(aild => aild.InvoiceLine)
                    .WithMany(invoiceline => invoiceline.AdditionalInvoiceLineData)
                    .HasForeignKey(aild => aild.InvoiceLineID)
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
