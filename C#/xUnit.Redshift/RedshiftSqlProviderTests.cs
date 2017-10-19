// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Data;
using Xunit;

using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Redshift;


namespace xUnit.Redshift
{
    public class RedshiftSqlProviderTests
    {
        public static RedshiftSQLProvider Provider = new RedshiftSQLProvider();
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
            Assert.NotNull(actual);
            Assert.Equal(expectedOutcome, actual);
        }
    }
}
