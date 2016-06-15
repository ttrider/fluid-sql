using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;

namespace Tests.PostgreSql
{
    [TestClass]
    public partial class Insert
    {
        PostgreSQLProvider Provider = new PostgreSQLProvider();

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
    }
}

