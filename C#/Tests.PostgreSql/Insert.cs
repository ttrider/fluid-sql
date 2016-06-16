using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace Tests.PostgreSql
{
    [TestClass]
    public partial class Insert: PostgreSqlProviderTests
    {
        [TestMethod]
        public void InsertValues()
        {
            var statement = Sql.Insert.Into(Sql.Name("table1"))               
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"table1\" VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, 'val2' );", text);
        }

        [TestMethod]
        public void InsertAlias()
        {
            var statement = Sql.Insert.Into(Sql.Name("table1").As("tbl1"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"table1\" AS \"tbl1\" VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, 'val2' );", text);
        }

        [TestMethod]
        public void InsertReturningStar()
        {
            var statement = Sql.Insert.Into(Sql.Name("table1"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2")).Output(Sql.Star());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"table1\" VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, 'val2' ) RETURNING *;", text);
        }

        [TestMethod]
        public void InsertReturning()
        {
            var statement = Sql.Insert.Into(Sql.Name("table1"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2")).Output(Sql.Name("id"), Sql.Name("value"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"table1\" VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, 'val2' ) RETURNING \"id\", \"value\";", text);
        }

        [TestMethod]
        public void InsertColumnValues()
        {
            var statement = Sql.Insert.Into(Sql.Name("table1"))
               .Columns(Sql.Name("id"), Sql.Name("value"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"table1\" ( \"id\", \"value\" ) VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, 'val2' );", text);
        }

        [TestMethod]
        public void InsertDefaultColumnValues()
        {
            var statement = Sql.Insert.Into(Sql.Name("table1"))
               .Columns(Sql.Name("id"), Sql.Name("value"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Default());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"table1\" ( \"id\", \"value\" ) VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, DEFAULT );", text);
        }

        [TestMethod]
        public void InsertDefaultOrIgnore()
        {
            var statement = Sql.Insert.Into(Sql.Name("table1")).DefaultValues().OrIgnore();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"table1\" DEFAULT VALUES ON CONFLICT DO NOTHING;", text);
        }

        #region Unit tests from FluidSql
        [TestMethod]
        public void InsertDefault()
        {
            var statement = Sql.Insert.Into(Sql.Name("table1")).DefaultValues();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO \"table1\" DEFAULT VALUES;", text);
        }

        [TestMethod]
        public void InsertFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("tbl2")).From(Sql.Select.From(Sql.Name("tbl1")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO \"tbl2\" SELECT * FROM \"tbl1\";", command.CommandText);
        }

        [TestMethod]
        public void InsertFrom2()
        {
            var statement = Sql.Insert.Into("foo.bar").From(Sql.Select.From("bar.foo"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" SELECT * FROM \"bar\".\"foo\";", command.CommandText);
        }

        [TestMethod]
        public void InsertTopFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("tbl1")).Top(1).From(Sql.Select.From(Sql.Name("tbl2")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO \"tbl1\" SELECT * FROM \"tbl2\" LIMIT 1;", command.CommandText);
        }

        [TestMethod]
        public void InsertColumnsFrom00()
        {
            var statement = Sql.Insert.Into(Sql.Name("tbl1")).From(Sql.Select.From(Sql.Name("tbl1"))).Columns("C1", "C2");

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO \"tbl1\" ( \"C1\", \"C2\" ) SELECT * FROM \"tbl1\";", command.CommandText);
        }

        [TestMethod]
        public void InsertColumnsFrom01()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).From(Sql.Select.From(Sql.Name("bar.foo"))).Columns(Sql.Name("id"), Sql.Name("value"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\";", command.CommandText);
        }

        [TestMethod]
        public void InsertColumnsFromOutput()
        {
            AssertSql(
                Sql.Insert.Into(Sql.Name("tbl1"))
                .From(Sql.Select.From(Sql.Name("tbl1")))
                .Columns(Sql.Name("C1"), Sql.Name("C2"))
                .Output(Sql.Name("inserted", "C1")),
                "INSERT INTO \"tbl1\" ( \"C1\", \"C2\" ) SELECT * FROM \"tbl1\" RETURNING \"C1\";");

            AssertSql(
                Sql.Insert.Into(Sql.Name("tbl1"))
                .From(Sql.Select.From(Sql.Name("tbl1")))
                .Columns(new List<Name> { Sql.Name("C1"), Sql.Name("C2") })
                .Output(Sql.Name("inserted", "C1")),
                "INSERT INTO \"tbl1\" ( \"C1\", \"C2\" ) SELECT * FROM \"tbl1\" RETURNING \"C1\";");

            AssertSql(
                Sql.Insert.Into(Sql.Name("tbl1"))
                .From(Sql.Select.From(Sql.Name("tbl1")))
                .Columns(new List<string> { "C1", "C2" })
                .Output(Sql.Name("inserted", "C1")),
                "INSERT INTO \"tbl1\" ( \"C1\", \"C2\" ) SELECT * FROM \"tbl1\" RETURNING \"C1\";");

            AssertSql(
                Sql.Insert.Into(Sql.Name("foo.bar"))
                .From(Sql.Select.From(Sql.Name("bar.foo")))
                .Columns("id", "value")
                .Output(Sql.Name("inserted", "id")),
                "INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\" RETURNING \"id\";");
        }

        [TestMethod]
        public void InsertColumnsFromOutputInto()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .From(Sql.Select.From(Sql.Name("bar.foo")))
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .OutputInto(Sql.Name("@t"), Sql.Name("inserted", "id"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) SELECT * FROM \"bar\".\"foo\" RETURNING \"id\" INTO \"t\";", command.CommandText);
        }

        [TestMethod]
        public void InsertColumnsValues00()
        {
            var statement = Sql.Insert.Into(Sql.Name("tbl1"))
                .Columns(Sql.Name("C1"), Sql.Name("C2"))
                .Values(Sql.Scalar(123), Sql.Scalar("val0"))
                .Values(234, "val1")
                .Values(new List<Scalar> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { 345, "val2" });

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO \"tbl1\" ( \"C1\", \"C2\" ) VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, 'val2' ), ( 345, 'val2' ), ( 345, 'val2' );", command.CommandText);
        }

        [TestMethod]
        public void InsertIdentityColumnsValues()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .IdentityInsert()
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .Values(Sql.Scalar(123), Sql.Scalar("val0"))
                .Values(234, "val1")
                .Values(new List<Scalar> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { 345, "val2" });

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO \"foo\".\"bar\" ( \"id\", \"value\" ) VALUES ( 123, 'val0' ), ( 234, 'val1' ), ( 345, 'val2' ), ( 345, 'val2' ), ( 345, 'val2' );", command.CommandText);
        }
        #endregion
    }
}

