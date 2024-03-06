using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00088, "v00.02.53-INX_NAVXChangeCreateTime")]
    public class InitialTables_00088 : Migration
    {
        public override void Down()
        {
            Delete.Index("INX_NAVXChangeCreateTime")
            .OnTable("NAVXChange");
        }
        public override void Up()
        {
            Create.Index("INX_NAVXChangeCreateTime")
                        .OnTable("NAVXChange")
                        .OnColumn("CreateTime").Ascending()
                        .WithOptions().NonClustered();
        }
    }
}
