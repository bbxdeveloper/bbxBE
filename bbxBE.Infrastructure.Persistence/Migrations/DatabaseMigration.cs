using bbxBE.Infrastructure.Persistence.Contexts;
using Dapper;
using System.Linq;

namespace bbxBE.Infrastructure.Persistence.Migrations
{
    public class DatabaseMigration
    {
        public DapperContext Context { get; set; }

        public DatabaseMigration(DapperContext context)
        {
            Context = context;
        }

        public void CreateDatabase(string dbName)
        {
            var query = "SELECT * FROM sys.databases WHERE name = @name";
            var parameters = new DynamicParameters();
            parameters.Add("name", dbName);

            using (var connection = Context.CreateConnection())
            {
                var records = connection.Query(query, parameters);
                if (!records.Any())
                    connection.Execute($"CREATE DATABASE {dbName}");
            }
        }
    }
}
