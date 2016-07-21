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
        public void CreateTable00()
        {
            var statement = Sql.CreateTable("BigTable00", true)
                .Columns(
                    TableColumn.Int("id").PrimaryKey().NotNull().AutoIncrement(),
                    TableColumn.BigInt("bigint").Default(1L).Null() 
                );

            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("CREATE TABLE IF NOT EXISTS `BigTable00` ( `id` INTEGER NOT NULL AUTO_INCREMENT PRIMARY KEY, `bigint` BIGINT NULL DEFAULT 1 );", mysql);
        }

        [TestMethod]
        public void CreatePurchasesTableForMergeTest()
        {
            var statement = Sql.CreateTable("Purchases", true)
                .Columns(
                    TableColumn.Int("ProductID").NotNull(),
                    TableColumn.Int("CustomerID").NotNull(),
                    TableColumn.DateTime("PurchaseDate").NotNull())
                .PrimaryKey(Sql.Name("PK_PurchProdID"), Sql.Name("ProductID"), Sql.Name("CustomerID"));

            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("CREATE TABLE IF NOT EXISTS `Purchases` ( `ProductID` INTEGER NOT NULL, `CustomerID` INTEGER NOT NULL, `PurchaseDate` DATETIME NOT NULL, CONSTRAINT `PK_PurchProdID` PRIMARY KEY ( `ProductID` ASC, `CustomerID` ASC ) );", mysql);
        }

        [TestMethod]
        public void CreateFactBuyingHabitsTableForMergeTest()
        {
            var statement = Sql.CreateTable("FactBuyingHabits", false)
                .Columns(
                    TableColumn.Int("ProductID").NotNull(),
                    TableColumn.Int("CustomerID").NotNull(),
                    TableColumn.DateTime("LastPurchaseDate").NotNull())
                .PrimaryKey(Sql.Name("PK_FactProdID"), Sql.Name("ProductID"), Sql.Name("CustomerID"));

            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("CREATE TABLE `FactBuyingHabits` ( `ProductID` INTEGER NOT NULL, `CustomerID` INTEGER NOT NULL, `LastPurchaseDate` DATETIME NOT NULL, CONSTRAINT `PK_FactProdID` PRIMARY KEY ( `ProductID` ASC, `CustomerID` ASC ) );", mysql);
        }

        [TestMethod]
        public void CreateTemporaryTable00()
        {
            var statement = Sql.CreateTemporaryTable("BigTempTable00", true)
                .Columns(TableColumn.Int("id"));

            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("CREATE TEMPORARY TABLE IF NOT EXISTS `BigTempTable00` ( `id` INTEGER );", mysql);
        }

        [TestMethod]
        public void DropTable00()
        {
            var statement = Sql.DropTable("BigTable00", true);

            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("DROP TABLE IF EXISTS `BigTable00`;", mysql);
        }

        [TestMethod]
        public void DropTemporaryTable00()
        {
            var statement = Sql.DropTemporaryTable("BigTempTable00", true);
            var mysql = provider.GenerateStatement(statement);

            Assert.AreEqual("DROP TEMPORARY TABLE IF EXISTS `BigTempTable00`;", mysql);
        }

    }
}
