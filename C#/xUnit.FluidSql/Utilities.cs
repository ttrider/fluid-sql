// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Data;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.SqlServer;

namespace xUnit.FluidSql
{
    public static class Utilities
    {
        public static string ConnectionString = @"Data Source=.;Integrated Security=True";

        public static SqlServerProvider SqlProvider = new SqlServerProvider();

        public static IDbCommand GetCommand(IStatement statement)
        {
            //return SqlProvider.GetCommand(statement, ConnectionString);
            return SqlProvider.GetCommand(statement, string.Empty);
        }

        public static IDbCommand GetCommandAsync(IStatement statement)
        {
            var t = SqlProvider.GetCommandAsync(statement, ConnectionString);
            t.Wait();
            return t.Result;
        }

        public static void AssertSqlToken(Token statement, string expectedOutcome)
        {
            /*var actual = SqlServerVisitor.Compile(statement).Value;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedOutcome, actual);*/
        }
    }
}