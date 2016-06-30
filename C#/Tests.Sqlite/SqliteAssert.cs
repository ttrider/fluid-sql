// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.Sqlite
{
    public static class SqliteAssert
    {
        public static SqliteProvider Provider = new SqliteProvider();

        public static void AreEqual(IStatement statement, string script)
        {
            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual(script, text);
        }
    }
}