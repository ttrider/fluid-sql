using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.PostgreSql
{
    /// <summary>
    /// Summary description for IfStatement
    /// </summary>
    [TestClass]
    public class ControlStructures : PostgreSqlProviderTests
    {
        [TestMethod]
        public void IfThen()
        {
            var statement = Sql.If(Sql.Function("EXISTS", Sql.Select.From("tbl1")))
                .Then(
                    Sql.Delete.From(Sql.Name("tbl1")), 
                    Sql.Insert.Into(Sql.Name("tbl1")).Values(Sql.Scalar(123), Sql.Scalar("val0")));  

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN IF EXISTS( ( SELECT * FROM \"tbl1\" ) ) THEN DELETE FROM \"tbl1\";\r\nINSERT INTO \"tbl1\" VALUES ( 123, 'val0' );\r\nEND IF;\r\nEND\r\n$do$;", command.CommandText);
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
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN IF EXISTS( ( SELECT * FROM \"tbl1\" ) ) THEN DELETE FROM \"tbl1\";\r\nINSERT INTO \"tbl1\" VALUES ( 123, 'val0' );\r\nELSE INSERT INTO \"tbl1\" VALUES ( 1, 'val1' );\r\nINSERT INTO \"tbl1\" VALUES ( 2, 'val2' );\r\nEND IF;\r\nEND\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void ContinueStatement()
        {
            AssertSql(Sql.Continue, "CONTINUE;");
            AssertSql(Sql.Continue.Label("test"), "CONTINUE [ test ];");
            AssertSql(Sql.Continue.Label("test").When(Sql.Name("avalue").IsNull()), "CONTINUE [ test ] WHEN \"avalue\" IS NULL;");
        }

        [TestMethod]
        public void ExitStatement()
        {
            AssertSql(Sql.Exit, "EXIT;");
            AssertSql(Sql.Exit.Label("test"), "EXIT [ test ];");
            AssertSql(Sql.Exit.Label("test").When(Sql.Name("avalue").IsNull()), "EXIT [ test ] WHEN \"avalue\" IS NULL;");
        }

        [TestMethod]
        public void CommentOutDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).CommentOut();
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("/* DROP TABLE \"some\".\"table\" */", command.CommandText);
        }

        [TestMethod]
        public void StringifyDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).Stringify();
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("' DROP TABLE \"some\".\"table\"';", command.CommandText);
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

        [TestMethod]
        public void Perform()
        {
            var statement = Sql.Perform("pg_sleep(10)");

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("PERFORM pg_sleep(10);", command.CommandText);
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
            Assert.AreEqual("SELECT  \"foo\", \"foo\"  FROM sys.objects;", command.CommandText);
        }

        [TestMethod]
        public void Throw()
        {
            var statement = Sql.Throw("test message");
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("RAISE 'test message';", command.CommandText);
        }

        [TestMethod]
        public void TryCatch()
        {
            var statement =
                Sql.Try(Sql.Select.Output(Sql.Scalar(1))).Catch(Sql.Throw());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN\r\nSELECT 1;\r\nEXCEPTION WHEN other THEN RAISE;\r\nEND;", command.CommandText);
        }        
    }
}
 