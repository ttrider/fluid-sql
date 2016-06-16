using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;

namespace Tests.PostgreSql
{
    public class PostgreSqlProviderTests
    {
        public static PostgreSQLProvider Provider = new PostgreSQLProvider();
        public static string ConnectionString = @"Data Source=.;Integrated Security=True";

        public static IDbCommand GetCommand(IStatement statement)
        {
            return Provider.GetCommand(statement, ConnectionString);
        }

        public static string GetStatement(IStatement statement)
        {
            return Provider.GenerateStatement(statement);
        }

        public static void AssertSql(IStatement statement, string expectedOutcome)
        {
            var actual = GetStatement(statement);
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedOutcome, actual);
        }        
    }
}
