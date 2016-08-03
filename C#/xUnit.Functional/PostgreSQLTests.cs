using Npgsql;
using TTRider.FluidSql.Providers.PostgreSQL;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers;
using Xunit;

namespace xUnit.Functional
{
    
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


        [Fact]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatements();
            Common.GenerateAndExecute(Provider, statement, connectionString);
        }

        [Fact]
        public void ParameterizedStatement()
        {
            var statement = Common.CreateParameterizedStatements();
            Common.GenerateAndExecute(Provider, statement, connectionString);
        }
    }
}
