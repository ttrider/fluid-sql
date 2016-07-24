// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;
using Xunit;

namespace xUnit.Sqlite
{
   public static class SqliteAssert
    {
        public static SqliteProvider Provider = new SqliteProvider();

        public static void AreEqual(IStatement statement, string script)
        {
            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal(script, text);
        }
    }
}