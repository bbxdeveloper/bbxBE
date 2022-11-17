using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    //https://learn.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key

    public class UsersEntityTypeConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder
            .HasMany<Offer>(p => p.OfferList)
            .WithOne(b => b.User)
            .HasForeignKey( k=>k.UserID)
            .IsRequired(false);
            
            builder
            .HasMany<Invoice>(p => p.InvoiceList)
            .WithOne(b => b.User)
            .HasForeignKey(k => k.UserID)
            .IsRequired(false);
        }
    }
}
