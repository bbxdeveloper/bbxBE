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
    public class InvoiceEntityTypeConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder
            .HasMany<InvoiceLine>(i => i.InvoiceLines)
            .WithOne(c => c.Invoice)
            .HasForeignKey(s => s.InvoiceID)
            .IsRequired();

            

        }
    }
}
