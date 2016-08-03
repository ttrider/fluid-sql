
using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.Sqlite;
using xUnit.Functional;
using Xunit;

namespace Tests.ProvidersEndToEnd
{
    
    public class SqliteTests
    {
        private const string ConnectionString = @"Data Source =:memory:;";
        static readonly IProvider Provider = new SqliteProvider();


        [Fact]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatements();
            Common.GenerateAndExecute(Provider, statement, ConnectionString);
        }
    }
}