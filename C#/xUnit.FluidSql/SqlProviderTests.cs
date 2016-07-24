using System.Data;
using Xunit;

using TTRider.FluidSql;
using TTRider.FluidSql.Providers.SqlServer;

namespace xUnit.FluidSql
{
    public class SqlProviderTests
    {
        public static SqlServerProvider SqlProvider = new SqlServerProvider();
        public static string ConnectionString = @"Data Source=.;Integrated Security=True";
        
        public static IDbCommand GetCommand(IStatement statement)
        {
            return SqlProvider.GetCommand(statement, ConnectionString);
        }

        public static string GetStatement(IStatement statement)
        {
            return SqlProvider.GenerateStatement(statement);
        }

        public static void AssertSql(IStatement statement, string expectedOutcome)
        {
            var actual = GetStatement(statement);
            Assert.NotNull(actual);
            Assert.Equal(expectedOutcome, actual);
        }        
    }
}