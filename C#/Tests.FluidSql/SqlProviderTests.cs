using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.SqlServer;

namespace FluidSqlTests
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
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedOutcome, actual);
        }
        public static void AssertSqlToken(Token statement, string expectedOutcome)
        {
            var actual = TTRider.FluidSql.Providers.SqlServer.SqlServerVisitor.Compile(statement).Value;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedOutcome, actual);
        }
    }
}
