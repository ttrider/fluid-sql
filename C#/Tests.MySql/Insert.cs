using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TTRider.FluidSql;

namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for Insert
    /// </summary>
    [TestClass]
    public class Insert : MySqlProviderTests
    {
        [TestMethod]
        public void InsertValues()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, N'val2' );", text);
        }

        [TestMethod]
        public void InsertAlias()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl").As("tbl1"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, N'val2' );", text);
        }

        [TestMethod]
        public void InsertReturningStar()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2")).Output(Sql.Star());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, N'val2' );", text);
        }

        [TestMethod]
        public void InsertReturning()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2")).Output(Sql.Name("id"), Sql.Name("value"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, N'val2' );", text);
        }

        [TestMethod]
        public void InsertColumnValues()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl"))
               .Columns(Sql.Name("number_value"), Sql.Name("text_value"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Scalar("val2"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` ( `number_value`, `text_value` ) VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, N'val2' );", text);
        }

        [TestMethod]
        public void InsertDefaultColumnValues()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl"))
               .Columns(Sql.Name("number_value"), Sql.Name("text_value"))
               .Values(Sql.Scalar(123), Sql.Scalar("val0"))
               .Values(Sql.Scalar(234), Sql.Scalar("val1"))
               .Values(Sql.Scalar(345), Sql.Default());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` ( `number_value`, `text_value` ) VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, DEFAULT );", text);
        }

        [TestMethod]
        public void InsertDefault()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl")).DefaultValues();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` VALUES ( );", text);
        }

        #region Unit tests from FluidSql

        [TestMethod]
        public void InsertFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert2_tbl")).From(Sql.Select.From(Sql.Name("test_insert_tbl")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO `test_insert2_tbl` SELECT * FROM `test_insert_tbl`;", command.CommandText);
        }

        [TestMethod]
        public void InsertFrom2()
        {
            var statement = Sql.Insert.Into("test_db.test_insert_tbl").From(Sql.Select.From("test_db.test_insert2_tbl"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO `test_db`.`test_insert_tbl` SELECT * FROM `test_db`.`test_insert2_tbl`;", command.CommandText);
        }

        [TestMethod]
        public void InsertTopFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl")).Top(1).From(Sql.Select.From(Sql.Name("test_insert2_tbl")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` SELECT * FROM `test_insert2_tbl` LIMIT 1;", command.CommandText);
        }

        [TestMethod]
        public void InsertColumnsFrom00()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl")).From(Sql.Select.From(Sql.Name("test_insert_tbl"))).Columns("number_value", "text_value");

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` ( `number_value`, `text_value` ) SELECT * FROM `test_insert_tbl`;", command.CommandText);
        }

        [TestMethod]
        public void InsertColumnsFrom01()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_db.test_insert_tbl")).From(Sql.Select.From(Sql.Name("test_db.test_insert_tbl"))).Columns(Sql.Name("number_value"), Sql.Name("text_value"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO `test_db`.`test_insert_tbl` ( `number_value`, `text_value` ) SELECT * FROM `test_db`.`test_insert_tbl`;", command.CommandText);
        }

        /*[TestMethod]
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
        }*/

        [TestMethod]
        public void InsertColumnsValues00()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_insert_tbl"))
                .Columns(Sql.Name("number_value"), Sql.Name("text_value"))
                .Values(Sql.Scalar(123), Sql.Scalar("val0"))
                .Values(234, "val1")
                .Values(new List<Scalar> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { 345, "val2" });

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO `test_insert_tbl` ( `number_value`, `text_value` ) VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, N'val2' ), ( 345, N'val2' ), ( 345, N'val2' );", command.CommandText);
        }

        [TestMethod]
        public void InsertIdentityColumnsValues()
        {
            var statement = Sql.Insert.Into(Sql.Name("test_db.test_insert_tbl"))
                .IdentityInsert()
                .Columns(Sql.Name("number_value"), Sql.Name("text_value"))
                .Values(Sql.Scalar(123), Sql.Scalar("val0"))
                .Values(234, "val1")
                .Values(new List<Scalar> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { 345, "val2" });

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("INSERT INTO `test_db`.`test_insert_tbl` ( `number_value`, `text_value` ) VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, N'val2' ), ( 345, N'val2' ), ( 345, N'val2' );", command.CommandText);
        }
        #endregion
    }
}
