using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    //https://stackoverflow.com/questions/51568253/one-to-many-relationship-getting-error-in-entity-framework-core-when-trying-to-i
    //https://learn.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key

    public class InvoiceLineEntityTypeConfiguration : IEntityTypeConfiguration<InvoiceLine>
    {
        public void Configure(EntityTypeBuilder<InvoiceLine> builder)
        {
            /* EZT ÁTNÉZNI
            builder
            .HasOne<Invoice>(i => i.Invoice)
            .WithMany(c => c.InvoiceLines)
            .HasForeignKey(s => s.InvoiceID)
            .IsRequired(true);

            builder
            .HasOne<InvoiceLine>(i => i.DeliveryNote)
            .WithMany(c => c.InvoiceLines)
            .HasForeignKey(s => s.RelDeliveryNoteInvoiceID)
            .IsRequired(false);

            */

            builder
            .HasMany<AdditionalInvoiceLineData>(i => i.AdditionalInvoiceLineData)
            .WithOne(c => c.InvoiceLine)
            .HasForeignKey(s => s.InvoiceLineID)
            .IsRequired(false);

            builder.Property(u => u.VatPercentage).HasPrecision(19, 4);
            builder.Property(u => u.UnitWeight).HasPrecision(19, 4);
            builder.Property(u => u.UnitPriceHUF).HasPrecision(19, 4);
            builder.Property(u => u.UnitPrice).HasPrecision(19, 4);
            builder.Property(u => u.TakeoverAmount).HasPrecision(19, 4);
            builder.Property(u => u.Quantity).HasPrecision(19, 4);
            builder.Property(u => u.ProductFeeRate).HasPrecision(19, 4);
            builder.Property(u => u.ProductFeeQuantity).HasPrecision(19, 4);
            builder.Property(u => u.ProductFeeAmount).HasPrecision(19, 4);
            builder.Property(u => u.PendingDNQuantity).HasPrecision(19, 4);
            builder.Property(u => u.LineVatDiscountedAmountHUF).HasPrecision(19, 4);
            builder.Property(u => u.LineVatDiscountedAmount).HasPrecision(19, 4);
            builder.Property(u => u.LineVatAmountHUF).HasPrecision(19, 4);
            builder.Property(u => u.LineVatAmount).HasPrecision(19, 4);
            builder.Property(u => u.LineNetDiscountedAmountHUF).HasPrecision(19, 4);
            builder.Property(u => u.LineNetDiscountedAmount).HasPrecision(19, 4);
            builder.Property(u => u.LineNetAmountHUF).HasPrecision(19, 4);
            builder.Property(u => u.LineNetAmount).HasPrecision(19, 4);
            builder.Property(u => u.LineGrossDiscountedAmountNormalHUF).HasPrecision(19, 4);
            builder.Property(u => u.LineGrossDiscountedAmountNormal).HasPrecision(19, 4);
            builder.Property(u => u.LineGrossAmountNormalHUF).HasPrecision(19, 4);
            builder.Property(u => u.LineGrossAmountNormal).HasPrecision(19, 4);
            builder.Property(u => u.LineExchangeRate).HasPrecision(19, 4);
            builder.Property(u => u.LineDiscountPercent).HasPrecision(19, 4);

        }
    }
}
