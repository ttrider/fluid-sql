using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.Sqlite
{
    [TestClass]
    public class Insert
    {
        public static SqliteProvider Provider = new SqliteProvider();

        [TestMethod]
        public void InsertDefault()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [TestMethod]
        public void InsertFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).From(Sql.Select.From(Sql.Name("bar.foo")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" SELECT * FROM \"bar\".\"foo\";", text);
        }

        [TestMethod]
        public void InsertTopFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).Top(1).From(Sql.Select.From(Sql.Name("bar.foo")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" SELECT * FROM \"bar\".\"foo\";", text);
        }

        [TestMethod]
        public void InsertColumnsFrom00()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).From(Sql.Select.From(Sql.Name("bar.foo"))).Columns("id", "value");

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\";", text);
        }

        [TestMethod]
        public void InsertColumnsFrom01()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).From(Sql.Select.From(Sql.Name("bar.foo"))).Columns(Sql.Name("id"), Sql.Name("value"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\";", text);
        }

        [TestMethod]
        public void InsertColumnsFromOutput()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .From(Sql.Select.From(Sql.Name("bar.foo")))
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .Output(Sql.Name("inserted", "id"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\";", text);
        }

        [TestMethod]
        public void InsertColumnsFromOutputInto()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .From(Sql.Select.From(Sql.Name("bar.foo")))
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .OutputInto(Sql.Name("@t"), Sql.Name("inserted", "id"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\";", text);
        }

        [TestMethod]
        public void InsertColumnsValues00()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .Values(Sql.Scalar(123), Sql.Scalar("val0"))
                .Values(Sql.Scalar(234), Sql.Scalar("val1"))
                .Values(Sql.Scalar(345), Sql.Scalar("val2"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, 'val2' );", text);
        }


        [TestMethod]
        public void InsertDefaultOrReplace()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrReplace();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT OR REPLACE INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [TestMethod]
        public void InsertDefaultOrRollback()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrRollback();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT OR ROLLBACK INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [TestMethod]
        public void InsertDefaultOrAbort()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrAbort();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT OR ABORT INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [TestMethod]
        public void InsertDefaultOrFail()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrFail();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT OR FAIL INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }

        [TestMethod]
        public void InsertDefaultOrIgnore()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues().OrIgnore();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT OR IGNORE INTO \"foo\".\"bar\" DEFAULT VALUES;", text);
        }
    }
}
