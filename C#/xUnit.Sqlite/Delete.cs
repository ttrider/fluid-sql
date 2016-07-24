// <license>
//     The MIT License (MIT)
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;
using Xunit;

namespace xUnit.Sqlite
{
    public class Delete
    {
        public static SqliteProvider Provider = new SqliteProvider();

        [Fact]
        public void Delete1()
        {
            var statement = Sql.Delete.Top(1).From(Sql.Name("foo.bar"));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("DELETE FROM \"foo\".\"bar\" LIMIT 1;", text);
        }

        [Fact]
        public void Delete1P()
        {
            var statement = Sql.Delete.From(Sql.Name("foo.bar")).Limit(1).Offset(10).OrderBy(Sql.Name("id"));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("DELETE FROM \"foo\".\"bar\" ORDER BY \"id\" ASC LIMIT 1 OFFSET 10;", text);
        }

        [Fact]
        public void DeleteWhere()
        {
            var statement = Sql.Delete.From(Sql.Name("foo.bar")).Where(Sql.Name("f").IsEqual(Sql.Name("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("DELETE FROM \"foo\".\"bar\" WHERE \"f\" = \"b\";", text);
        }
    }
}