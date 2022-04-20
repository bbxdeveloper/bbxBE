using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    public class ProductCodeEntityTypeConfiguration : IEntityTypeConfiguration<ProductCode>
    {
        public void Configure(EntityTypeBuilder<ProductCode> builder)
        {
            // This Converter will perform the conversion to and from Json to the desired type
            builder
                .HasOne<Product>(s => s.Product)
                .WithMany(g => g.ProductCodes)
                .HasForeignKey(s => s.ProductID);
        }
    }
}
