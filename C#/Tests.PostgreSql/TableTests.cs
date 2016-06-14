using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;

namespace Tests.PostgreSql
{
    [TestClass]
    public partial class TableTests
    {
        PostgreSQLProvider provider = new PostgreSQLProvider();

        [TestMethod]
        public void DropTable()
        {
            var statement = Sql.DropTable ("table1", false);

            var postgresql = provider.GenerateStatement(statement);

            Assert.AreEqual("DROP TABLE \"table1\";", postgresql);
        }

        [TestMethod]
        public void DropTableExists()
        {
            var statement = Sql.DropTable("table1", true);

            var postgresql = provider.GenerateStatement(statement);

            Assert.AreEqual("DROP TABLE IF EXISTS \"table1\";", postgresql);
        }

        [TestMethod]
        public void DropTableCascade()
        {
            var statement = Sql.DropTable("table1", true, true);

            var postgresql = provider.GenerateStatement(statement);

            Assert.AreEqual("DROP TABLE IF EXISTS \"table1\" CASCADE;", postgresql);
        }

        [TestMethod]
        public void DropTableRestrict()
        {
            var statement = Sql.DropTable("table1", false, true);

            var postgresql = provider.GenerateStatement(statement);

            Assert.AreEqual("DROP TABLE IF EXISTS \"table1\" RESTRICT;", postgresql);
        }
    }
}
