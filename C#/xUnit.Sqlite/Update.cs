// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;
using Xunit;

namespace xUnit.Sqlite
{
    public class Update
    {
        public static SqliteProvider Provider = new SqliteProvider();

        [Fact]
        public void UpdateDefault()
        {
            var statement =
                Sql.Update("foo.bar").Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("UPDATE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [Fact]
        public void UpdateOrFail()
        {
            var statement = Sql.Update("foo.bar").OrFail()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("UPDATE OR FAIL \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [Fact]
        public void UpdateOrAbort()
        {
            var statement = Sql.Update("foo.bar").OrAbort()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("UPDATE OR ABORT \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [Fact]
        public void UpdateOrIgnore()
        {
            var statement = Sql.Update("foo.bar").OrIgnore()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("UPDATE OR IGNORE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [Fact]
        public void UpdateOrReplace()
        {
            var statement = Sql.Update("foo.bar").OrReplace()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("UPDATE OR REPLACE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [Fact]
        public void UpdateOrRollback()
        {
            var statement = Sql.Update("foo.bar").OrRollback()
                .Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("UPDATE OR ROLLBACK \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b';", text);
        }

        [Fact]
        public void UpdateWhere()
        {
            var statement = Sql.Update("foo.bar")
                .Set(Sql.Name("a"), Sql.Scalar(1))
                .Set(Sql.Name("c"), Sql.Scalar("1"))
                .Where(Sql.Name("z").IsEqual(Sql.Scalar("b")))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);

            Assert.Equal("UPDATE \"foo\".\"bar\" SET \"a\" = 1, \"c\" = '1' WHERE \"z\" = 'b';", text);
        }
    }
}