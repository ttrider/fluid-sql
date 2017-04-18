// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.Redshift
{
    public class TableDrop : RedshiftSqlProviderTests
    {
        [Fact]
        public void DropTable()
        {
            var statement = Sql.DropTable("table1", false);

            var RedshiftSql = Provider.GenerateStatement(statement);

            Assert.Equal("DROP TABLE \"table1\";", RedshiftSql);
        }

        [Fact]
        public void DropTableExists()
        {
            var statement = Sql.DropTable("table1", true);

            var RedshiftSql = Provider.GenerateStatement(statement);

            Assert.Equal("DROP TABLE IF EXISTS \"table1\";", RedshiftSql);
        }

        [Fact]
        public void DropTableCascade()
        {
            var statement = Sql.DropTable("table1", true).Cascade();

            var RedshiftSql = Provider.GenerateStatement(statement);

            Assert.Equal("DROP TABLE IF EXISTS \"table1\" CASCADE;", RedshiftSql);
        }

        [Fact]
        public void DropTableRestrict()
        {
            var statement = Sql.DropTable("table1", true).Restrict();

            var RedshiftSql = Provider.GenerateStatement(statement);

            Assert.Equal("DROP TABLE IF EXISTS \"table1\" RESTRICT;", RedshiftSql);
        }
    }
}
