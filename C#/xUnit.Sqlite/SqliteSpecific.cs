// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;
using Xunit;

namespace xUnit.Sqlite
{
    public class SqliteSpecific
    {
        [Fact]
        public void Vacuum()
        {
            SqliteAssert.AreEqual(TTRider.FluidSql.Sqlite.Vacuum, "VACUUM;");
        }
    }
}