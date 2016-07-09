using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for ControlStructures
    /// </summary>
    [TestClass]
    public class ControlStructures : MySqlProviderTests
    {
        [TestMethod]
        public void Case()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.Case.When(Sql.Scalar("a").IsEqual(Sql.Scalar("b")), Sql.Scalar("a"))
                    .When(Sql.Scalar("a").NotEqual(Sql.Scalar("b")), Sql.Scalar("b"))
                    .Else(Sql.Scalar("c"))),
                "SELECT CASE WHEN N'a' = N'b' THEN N'a' WHEN N'a' <> N'b' THEN N'b' ELSE N'c' END;"
                );
        }

        [TestMethod]
        public void IIF()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.IIF(
                        Sql.Scalar("a").IsEqual(Sql.Scalar("b")),
                        Sql.Scalar("a"), Sql.Scalar("b"))),
                "SELECT IF ( N'a' = N'b', N'a', N'b' );"
                );
        }

        [TestMethod]
        public void IfThen()
        {
            var statement = Sql.If(Sql.Function("EXISTS", Sql.Select.From("tbl1")))
                .Then(
                    Sql.Delete.From(Sql.Name("tbl1")),
                    Sql.Insert.Into(Sql.Name("tbl1")).Values(Sql.Scalar(123), Sql.Scalar("val0")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF EXISTS( ( SELECT * FROM `tbl1` ) )\r\nTHEN\r\nDELETE FROM `tbl1`;\r\nINSERT INTO `tbl1` VALUES ( 123, N'val0' );\r\nEND IF;", command.CommandText);
        }

        [TestMethod]
        public void IfThenElse()
        {
            var statement = Sql.If(Sql.Function("EXISTS", Sql.Select.From("tbl1")))
               .Then(
                    Sql.Delete.From(Sql.Name("tbl1")),
                    Sql.Insert.Into(Sql.Name("tbl1")).Values(Sql.Scalar(123), Sql.Scalar("val0")))
                .Else(
                    Sql.Insert.Into(Sql.Name("tbl1")).Values(Sql.Scalar(1), Sql.Scalar("val1")),
                    Sql.Insert.Into(Sql.Name("tbl1")).Values(Sql.Scalar(2), Sql.Scalar("val2")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF EXISTS( ( SELECT * FROM `tbl1` ) )\r\nTHEN\r\nDELETE FROM `tbl1`;\r\nINSERT INTO `tbl1` VALUES ( 123, N'val0' );\r\nELSE\r\nINSERT INTO `tbl1` VALUES ( 1, N'val1' );\r\nINSERT INTO `tbl1` VALUES ( 2, N'val2' );\r\nEND IF;", command.CommandText);
        }

        //not checked on db
        [TestMethod]
        public void ContinueStatement()
        {
            AssertSql(Sql.Continue, "ITERATE;");
            AssertSql(Sql.Continue.Label("test"), "ITERATE test;");
        }

        //not checked on db
        [TestMethod]
        public void BreakStatement()
        {
            AssertSql(Sql.Break, "LEAVE;");
            AssertSql(Sql.Break.Label("test"), "LEAVE test;");
        }

        [TestMethod]
        public void Transactions()
        {
            AssertSql(Sql.BeginTransaction(), "START TRANSACTION;");
            AssertSql(Sql.CommitTransaction(), "COMMIT;");
            AssertSql(Sql.RollbackTransaction(), "ROLLBACK;");
            AssertSql(Sql.SaveTransaction(Sql.Name("testLabel")), "SAVEPOINT `testLabel`;");
            AssertSql(Sql.RollbackTransaction(Sql.Name("testLabel")), "ROLLBACK TO SAVEPOINT `testLabel`;");

        }
        
        /*[TestMethod]
        public void ExitStatement()
        {
            AssertSql(Sql.Exit, "EXIT;");
            AssertSql(Sql.Exit.Label("test"), "EXIT [ test ];");
            AssertSql(Sql.Exit.Label("test").When(Sql.Name("avalue").IsNull()), "EXIT [ test ] WHEN \"avalue\" IS NULL;");
        }*/

        [TestMethod]
        public void CommentOutDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).CommentOut();
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("/* DROP TABLE `some`.`table` */", command.CommandText);
        }

        [TestMethod]
        public void StringifyDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).Stringify();
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("' DROP TABLE `some`.`table`';", command.CommandText);
        }

        [TestMethod]
        public void Snippet01()
        {
            var statement = Sql.SnippetStatement("SELECT * FROM sys.objects");

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM sys.objects;", command.CommandText);
        }

        [TestMethod]
        public void Snippet02()
        {
            var statement = Sql.TemplateStatement("SELECT {0},{0} FROM sys.objects", Sql.Name("foo"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT  `foo`, `foo`  FROM sys.objects;", command.CommandText);
        }

        [TestMethod]
        public void Return()
        {
            var statement = Sql.Return();

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("RETURN;", command.CommandText);
        }

        [TestMethod]
        public void Return2()
        {
            var statement = Sql.Return(2);

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("RETURN 2;", command.CommandText);
        }        
    }
}
