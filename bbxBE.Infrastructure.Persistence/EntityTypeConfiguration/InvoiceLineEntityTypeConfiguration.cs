using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
            .HasOne<Invoice>(i => i.DeliveryNote)
            .WithMany(c => c.InvoiceLines)
            .HasForeignKey(s => s.RelDeliveryNoteInvoiceID)
            .IsRequired(false);
            */


        }
    }
}
