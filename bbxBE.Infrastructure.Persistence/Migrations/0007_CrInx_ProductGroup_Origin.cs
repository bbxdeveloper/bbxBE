using bbxBE.Domain.Entities;
using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00007, "v00.00.01")]
    public class CreateIndexes_00007 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_ProductGroupCode")
                        .OnTable("ProductGroup");

            Delete.Index("INX_OriginCode")
                         .OnTable("Origin");
        }

        public override void Up()
        {
            Create.Index("INX_ProductGroupCode")
                         .OnTable("ProductGroup")
                         .OnColumn("ProductGroupCode").Ascending()
                         .WithOptions().NonClustered();

            Create.Index("INX_OriginCode")
                         .OnTable("Origin")
                         .OnColumn("OriginCode").Ascending()
                         .WithOptions().NonClustered();

        }

    }

}
