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
    public class Insert
    {
        public static SqliteProvider Provider = new SqliteProvider();

        [Fact]
        public void InsertDefault()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues();

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [Fact]
        public void InsertFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).From(Sql.Select.From(Sql.Name("bar.foo")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT INTO \"foo\".\"bar\" SELECT * FROM \"bar\".\"foo\";", text);
        }

        [Fact]
        public void InsertTopFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).Top(1).From(Sql.Select.From(Sql.Name("bar.foo")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT INTO \"foo\".\"bar\" SELECT * FROM \"bar\".\"foo\";", text);
        }

        [Fact]
        public void InsertColumnsFrom00()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .From(Sql.Select.From(Sql.Name("bar.foo")))
                .Columns("id", "value");

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\";", text);
        }

        [Fact]
        public void InsertColumnsFrom01()
        {
            var statement =
                Sql.Insert.Into(Sql.Name("foo.bar"))
                    .From(Sql.Select.From(Sql.Name("bar.foo")))
                    .Columns(Sql.Name("id"), Sql.Name("value"));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\";", text);
        }

        [Fact]
        public void InsertColumnsFromOutput()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .From(Sql.Select.From(Sql.Name("bar.foo")))
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .Output(Sql.Name("inserted", "id"));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\";", text);
        }

        [Fact]
        public void InsertColumnsFromOutputInto()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .From(Sql.Select.From(Sql.Name("bar.foo")))
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .OutputInto(Sql.Name("@t"), Sql.Name("inserted", "id"));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\";", text);
        }

        [Fact]
        public void InsertColumnsValues00()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .Values(Sql.Scalar(123), Sql.Scalar("val0"))
                .Values(Sql.Scalar(234), Sql.Scalar("val1"))
                .Values(Sql.Scalar(345), Sql.Scalar("val2"));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal(
                "INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, 'val2' );",
                text);
        }

        [Fact]
        public void InsertDefaultOrReplace()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrReplace();

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT OR REPLACE INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [Fact]
        public void InsertDefaultOrRollback()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrRollback();

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT OR ROLLBACK INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [Fact]
        public void InsertDefaultOrAbort()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrAbort();

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT OR ABORT INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [Fact]
        public void InsertDefaultOrFail()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrFail();

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT OR FAIL INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [Fact]
        public void InsertDefaultOrIgnore()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrIgnore();

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("INSERT OR IGNORE INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }
    }
}