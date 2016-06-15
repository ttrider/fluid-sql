using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;

namespace Tests.PostgreSql
{
    [TestClass]
    public partial class Select
    {
        PostgreSQLProvider Provider = new PostgreSQLProvider();

        [TestMethod]
        public void SelectStarFromTable()
        {
            var statement = Sql.Select.From("table1");

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM \"table1\";", text); 
        }

        [TestMethod]
        public void SelectColumnsFromTable()
        {
            var statement = Sql.Select.Output(Sql.Name("id"), Sql.Name("value")).From("table1");

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\", \"value\" FROM \"table1\";", text);
        }

        [TestMethod]
        public void SelectWhereStarFromTable()
        {
            var statement = Sql.Select.From("table1").Where(Sql.Name("id").IsEqual(Sql.Scalar(100))); ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM \"table1\" WHERE \"id\" = 100;", text);
        }

        [TestMethod]
        public void SelectOrderBy()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").OrderBy(Sql.Name("id"), Direction.Asc);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\" FROM \"table1\" ORDER BY \"id\" ASC;", text); 
        }

        [TestMethod]
        public void SelectGroupBy()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").GroupBy(Sql.Name("id")).OrderBy(Sql.Name("id"), Direction.Asc);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\" FROM \"table1\" GROUP BY \"id\" ORDER BY \"id\" ASC;", text);
        }

        [TestMethod]
        public void SelectGroupByHaving()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").GroupBy(Sql.Name("id"), Sql.Name("value")).Having(Sql.Name("value").IsEqual(Sql.Scalar("val8")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\" FROM \"table1\" GROUP BY \"id\", \"value\" HAVING \"value\" = 'val8';", text);
        }
    }
}
