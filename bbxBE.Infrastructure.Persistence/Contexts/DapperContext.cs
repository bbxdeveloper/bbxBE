using bbxBE.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;

namespace bbxBE.Infrastructure.Persistence.Contexts
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _connectionStringMaster;
        private readonly DataBaseTypeEnum _dbType;

        public DapperContext(IConfiguration configuration) // DataBaseType
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("bbxdbconnection");
            _connectionStringMaster = _configuration.GetConnectionString("masterdbconnection");
            _dbType = (DataBaseTypeEnum)Enum.Parse(typeof(DataBaseTypeEnum), _configuration.GetValue<string>("DataBaseType"));
        }

        public IDbConnection CreateConnection() =>
            _dbType switch
            {
                DataBaseTypeEnum.SQLITE_LOCAL => new SqliteConnection(_connectionString),
                DataBaseTypeEnum.MSSQL_SERVER => new SqlConnection(_connectionString),
                _ => new SqlConnection(_connectionString),
            };

        public IDbConnection CreateMasterDbConnection() =>
        _dbType switch
        {
            DataBaseTypeEnum.SQLITE_LOCAL => new SqliteConnection(_connectionStringMaster),
            DataBaseTypeEnum.MSSQL_SERVER => new SqlConnection(_connectionStringMaster),
            _ => new SqlConnection(_connectionStringMaster),
        };
    }
}
