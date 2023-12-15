using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    //https://stackoverflow.com/questions/51568253/one-to-many-relationship-getting-error-in-entity-framework-core-when-trying-to-i
    //https://learn.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key

    public class InvoiceEntityTypeConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {

            builder
            .HasMany<InvoiceLine>(i => i.InvoiceLines)
            .WithOne(c => c.Invoice)
            .HasForeignKey(s => s.InvoiceID)
            .IsRequired(true);

            builder
            .HasMany<NAVXChange>(i => i.NAVXChanges)
            .WithOne(i => i.Invoice)
            .HasForeignKey(s => s.InvoiceID)
            .IsRequired(false);

            builder
            .HasMany<InvPayment>(i => i.InvPayments)
            .WithOne(i => i.Invoice)
            .HasForeignKey(s => s.InvoiceID)
            .IsRequired(false);

        }
    }
}
