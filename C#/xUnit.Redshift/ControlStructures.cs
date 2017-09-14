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
    public class ControlStructures : RedshiftSqlProviderTests
    {
        [Fact]
        public void IfThen()
        {
            var statement = Sql.If(Sql.Function("EXISTS", Sql.Select.From("tbl1")))
                .Then(
                    Sql.Delete.From(Sql.Name("tbl1")), 
                    Sql.Insert.Into(Sql.Name("tbl1")).Values(Sql.Scalar(123), Sql.Scalar("val0")));  

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DO\r\n$do$\r\nBEGIN IF EXISTS( ( SELECT * FROM \"tbl1\" ) ) THEN DELETE FROM \"tbl1\";\r\nINSERT INTO \"tbl1\" VALUES ( 123, 'val0' );\r\nEND IF;\r\nEND\r\n$do$;", command.CommandText);
        }

        [Fact]
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

            Assert.NotNull(command);
            Assert.Equal("DO\r\n$do$\r\nBEGIN IF EXISTS( ( SELECT * FROM \"tbl1\" ) ) THEN DELETE FROM \"tbl1\";\r\nINSERT INTO \"tbl1\" VALUES ( 123, 'val0' );\r\nELSE INSERT INTO \"tbl1\" VALUES ( 1, 'val1' );\r\nINSERT INTO \"tbl1\" VALUES ( 2, 'val2' );\r\nEND IF;\r\nEND\r\n$do$;", command.CommandText);
        }

        [Fact]
        public void ContinueStatement()
        {
            AssertSql(Sql.Continue, "CONTINUE;");
            AssertSql(Sql.Continue.Label("test"), "CONTINUE [ test ];");
            AssertSql(Sql.Continue.Label("test").When(Sql.Name("avalue").IsNull()), "CONTINUE [ test ] WHEN \"avalue\" IS NULL;");
        }

        [Fact]
        public void ExitStatement()
        {
            AssertSql(Sql.Exit, "EXIT;");
            AssertSql(Sql.Exit.Label("test"), "EXIT [ test ];");
            AssertSql(Sql.Exit.Label("test").When(Sql.Name("avalue").IsNull()), "EXIT [ test ] WHEN \"avalue\" IS NULL;");
        }

        [Fact]
        public void CommentOutDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).CommentOut();
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("/* DROP TABLE \"some\".\"table\" */", command.CommandText);
        }

        [Fact]
        public void StringifyDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).Stringify();
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("' DROP TABLE \"some\".\"table\"';", command.CommandText);
        }

        [Fact]
        public void Return()
        {
            var statement = Sql.Return();

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("RETURN;", command.CommandText);
        }

        [Fact]
        public void Return2()
        {
            var statement = Sql.Return(2);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("RETURN 2;", command.CommandText);
        }

        [Fact]
        public void Perform()
        {
            var statement = Sql.Perform("pg_sleep(10)");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("PERFORM pg_sleep(10);", command.CommandText);
        }

        [Fact]
        public void Snippet01()
        {
            var statement = Sql.SnippetStatement("SELECT * FROM sys.objects");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM sys.objects;", command.CommandText);
        }

        [Fact]
        public void Snippet02()
        {
            var statement = Sql.TemplateStatement("SELECT {0},{0} FROM sys.objects", Sql.Name("foo"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT  \"foo\", \"foo\"  FROM sys.objects;", command.CommandText);
        }

        [Fact]
        public void Throw()
        {
            var statement = Sql.Throw("test message");
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("RAISE 'test message';", command.CommandText);
        }

        [Fact]
        public void TryCatch()
        {
            var statement =
                Sql.Try(Sql.Select.Output(Sql.Scalar(1))).Catch(Sql.Throw());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN\r\nSELECT 1;\r\nEXCEPTION WHEN other THEN RAISE;\r\nEND;", command.CommandText);
        }        
    }
}
 