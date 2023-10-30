using bbxBE.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bbxBE.Infrastructure.Persistence.EntityTypeConfigurations
{
    //https://stackoverflow.com/questions/51568253/one-to-many-relationship-getting-error-in-entity-framework-core-when-trying-to-i
    //https://learn.microsoft.com/en-us/ef/core/modeling/relationships?tabs=fluent-api%2Cfluent-api-simple-key%2Csimple-key

    public class NAVXChangeEntityTypeConfiguration : IEntityTypeConfiguration<NAVXChange>
    {
        public void Configure(EntityTypeBuilder<NAVXChange> builder)
        {

            builder.HasMany<NAVXResult>(r => r.NAVXResults).WithOne(r => r.XChange).IsRequired(false);

        }
    }
}
