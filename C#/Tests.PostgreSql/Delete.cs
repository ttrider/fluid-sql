using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;

namespace Tests.PostgreSql
{
    [TestClass]
    public partial class Delete
    {
        PostgreSQLProvider Provider = new PostgreSQLProvider();

        [TestMethod]
        public void DeleteFromTable()
        {
            var statement = Sql.Delete.From(Sql.Name("table1"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\";", text);
        }

        [TestMethod]
        public void DeleteWhere()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Where(Sql.Name("id").IsEqual(Sql.Scalar(100)));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" WHERE \"id\" = 100;", text);
        }

        [TestMethod]
        public void DeleteWhereConditions()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Where(Sql.Name("id").IsEqual(Sql.Scalar(100)).Or(Sql.Name("value").IsEqual(Sql.Scalar("val1"))));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" WHERE \"id\" = 100 OR \"value\" = 'val1';", text);
        }
    }
}
