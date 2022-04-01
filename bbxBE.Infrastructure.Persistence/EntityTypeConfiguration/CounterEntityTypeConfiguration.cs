using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    public class CounterEntityTypeConfiguration : IEntityTypeConfiguration<Counter>
    {
        public void Configure(EntityTypeBuilder<Counter> builder)
        {
            // This Converter will perform the conversion to and from Json to the desired type
            builder.Property(e => e.CounterPool).HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<IList<CounterPoolItem>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}
