using bbxBE.Domain.Entities;
using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00008, "v00.00.01-INX_WarehouseCode")]
    public class CreateIndexes_00008 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_WarehouseCode")
                        .OnTable("WarehouseCode");
        }

        public override void Up()
        {
            Create.Index("INX_WarehouseCode")
                         .OnTable("Warehouse")
                         .OnColumn("WarehouseCode").Ascending()
                         .WithOptions().NonClustered();

        }

    }

}
