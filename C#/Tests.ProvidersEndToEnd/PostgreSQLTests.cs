using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.PostgreSQL;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.ProvidersEndToEnd
{
    [TestClass]
    public class PostgreSQLTests
    {
        // create database fluidtest

        static readonly IProvider Provider = new PostgreSQLProvider();

        private readonly string connectionString = new NpgsqlConnectionStringBuilder
        {
            Database = "fluidtest",
            Username = "fluidtest",
            Password = "fluidtest",
            Host = "localhost",
            Port = 5432
        }.ConnectionString;


        [TestMethod]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatement();
            Common.GenerateAndExecute(Provider, statement, connectionString);
        }
    }
}
