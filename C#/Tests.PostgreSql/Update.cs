using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;

namespace Tests.PostgreSql
{
    [TestClass]
    public partial class Update
    {
        PostgreSQLProvider Provider = new PostgreSQLProvider();

        [TestMethod]
        public void UpdateWhere()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Name("val0")))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);

            Assert.AreEqual("UPDATE \"table1\" SET \"id\" = 100 WHERE \"value\" = \"val0\";", text);
        }
    }
}
