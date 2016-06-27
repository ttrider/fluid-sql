using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.PostgreSql
{
    /// <summary>
    /// Summary description for IfStatement
    /// </summary>
    [TestClass]
    public class IfStatement : PostgreSqlProviderTests
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
    }
}
