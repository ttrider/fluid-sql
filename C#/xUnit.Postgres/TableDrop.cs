// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.Postgres
{
    public class TableDrop : PostgreSqlProviderTests
    {
        [Fact]
        public void DropTable()
        {
            var statement = Sql.DropTable("table1", false);

            var postgresql = Provider.GenerateStatement(statement);

            Assert.Equal("DROP TABLE \"table1\";", postgresql);
        }

        [Fact]
        public void DropTableExists()
        {
            var statement = Sql.DropTable("table1", true);

            var postgresql = Provider.GenerateStatement(statement);

            Assert.Equal("DROP TABLE IF EXISTS \"table1\";", postgresql);
        }

        [Fact]
        public void DropTableCascade()
        {
            var statement = Sql.DropTable("table1", true).Cascade();

            var postgresql = Provider.GenerateStatement(statement);

            Assert.Equal("DROP TABLE IF EXISTS \"table1\" CASCADE;", postgresql);
        }

        [Fact]
        public void DropTableRestrict()
        {
            var statement = Sql.DropTable("table1", true).Restrict();

            var postgresql = Provider.GenerateStatement(statement);

            Assert.Equal("DROP TABLE IF EXISTS \"table1\" RESTRICT;", postgresql);
        }
    }
}
