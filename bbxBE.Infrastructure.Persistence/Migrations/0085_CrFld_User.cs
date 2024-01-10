using FluentMigrator;

//https://code-maze.com/dapper-migrations-fluentmigrator-aspnetcore/

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    [Migration(00086, "v00.02.46-Users.UserLevel mező")]
    public class InitialTables_00086 : Migration
    {
        public override void Down()
        {
            Delete.Column("UserLevel").FromTable("Users");
        }
        public override void Up()
        {
            Alter.Table("Users")
                    .AddColumn("Userlevel").AsString().Nullable();
            Update.Table("Users").Set(new { UserLevel = "LEVEL2" }).AllRows();
        }
    }
}
