using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.MySql;

namespace Tests.MySqlTests
{
    [TestClass]
    public partial class TableTests
    {
        MySqlProvider provider = new MySqlProvider();

        [TestMethod]
        public IStatement CreateTable00()
        {
            var statement = Sql.CreateTable("BigTable00", true)
                .Columns(
                    TableColumn.Int("id").PrimaryKey().NotNull().AutoIncrement(),
                    TableColumn.BigInt("bigint").Default(1L).Null() 
                );

            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("CREATE TABLE IF NOT EXISTS `BigTable00` ( `id` INTEGER NOT NULL AUTO_INCREMENT PRIMARY KEY, `bigint` BIGINT NULL DEFAULT 1 );", mysql);


            return statement;
        }

        [TestMethod]
        public IStatement CreateTemporaryTable00()
        {
            var statement = Sql.CreateTemporaryTable("BigTempTable00", true)
                .Columns(TableColumn.Int("id"));

            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("CREATE TEMPORARY TABLE IF NOT EXISTS `BigTempTable00` ( `id` INTEGER );", mysql);


            return statement;
        }

        [TestMethod]
        public IStatement DropTable00()
        {
            var statement = Sql.DropTable("BigTable00", true);

            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("DROP TABLE IF EXISTS `BigTable00`;", mysql);


            return statement;
        }
        [TestMethod]
        public IStatement DropTemporaryTable00()
        {
            var statement = Sql.DropTemporaryTable("BigTempTable00", true);
            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("DROP TEMPORARY TABLE IF EXISTS `BigTempTable00`;", mysql);


            return statement;
        }

    }
}
