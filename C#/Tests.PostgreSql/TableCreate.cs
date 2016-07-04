using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;


namespace Tests.PostgreSql
{
    [TestClass]
    public class TableCreate : PostgreSqlProviderTests
    {
        [TestMethod]
        public void CreateEmptyTable()
        {
            var statement = Sql.CreateTable(Sql.Name("table1"), false);
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE \"table1\" ( );", command.CommandText);
        }

        [TestMethod]
        public void CreateTableWithColumns()
        {
            var statement = Sql.CreateTable(Sql.Name("table1"), false)
                .Columns(TableColumn.Int("id"), TableColumn.Text("text1"));
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE \"table1\" ( \"id\" INTEGER, \"text1\" TEXT );", command.CommandText);
        }

        [TestMethod]
        public void CreateTemporaryTableWithColumns()
        {
            var statement = Sql.CreateTemporaryTable(Sql.Name("table1"), false)
                .Columns(TableColumn.Int("id"), TableColumn.Text("text1"));
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TEMPORARY TABLE \"table1\" ( \"id\" INTEGER, \"text1\" TEXT );", command.CommandText);
        }

        [TestMethod]
        public void CreateTableWithColumnsOptions()
        {
            var statement = Sql.CreateTable(Sql.Name("table1"), false)
                .Columns(
                    TableColumn.Int("id").NotNull().Default(100)
                    ,TableColumn.Text("text1").Null());
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE \"table1\" ( \"id\" INTEGER NOT NULL DEFAULT 100, \"text1\" TEXT NULL );", command.CommandText); 
        }

        [TestMethod]
        public void CreateTableWithPrimaryKey()
        {
            var statement = Sql.CreateTable(Sql.Name("table1"), false)
                .Columns(
                    TableColumn.Int("C1").NotNull()
                    , TableColumn.Text("C2").Null())
                .PrimaryKey("PK_tbl", new Order { Column = Sql.Name("C1"), Direction = Direction.Asc });
                
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE \"table1\" ( \"C1\" INTEGER NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ) );", command.CommandText);
        }

        #region Unit tests from FluidSql
        [TestMethod]
        public void CreateTable()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), false)
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .PrimaryKey("PK_tbl", new Order() { Column = Sql.Name("C1"), Direction = Direction.Asc })
                ;

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ) );", command.CommandText);
        }

        [TestMethod]
        public void CreateTableWithConstrains()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), false)
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .PrimaryKey("PK_tbl", new Order { Column = Sql.Name("C1"), Direction = Direction.Asc })
                .UniqueConstrainOn("UC_tbl", new Order { Column = Sql.Name("C2") })
                ;

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ), CONSTRAINT \"UC_tbl\" UNIQUE ( \"C2\" ) );", command.CommandText);
        }

        [TestMethod]
        public void CreateTableWithIndex()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), false)
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .PrimaryKey("PK_tbl", new Order { Column = Sql.Name("C1"), Direction = Direction.Asc })
                .IndexOn("IX_tbl", new Order { Column = Sql.Name("C2") })
                ;

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ) );\r\nCREATE INDEX \"IX_tbl\" ON \"tbl\" ( \"C2\" ASC );", command.CommandText);
        }

        [TestMethod]
        public void CreateTableWithIndexIfNotExists()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), true)
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .PrimaryKey("PK_tbl", new Order { Column = Sql.Name("C1"), Direction = Direction.Asc })
                .IndexOn("IX_tbl", new Order { Column = Sql.Name("C2") })
                ;

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE IF NOT EXISTS \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ) );\r\nCREATE INDEX \"IX_tbl\" ON \"tbl\" ( \"C2\" ASC );", command.CommandText);
        }

        [TestMethod]
        public void CreateTableVariable()
        {
            var statement = Sql.CreateTableVariable(Sql.Name("tbl"))
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.Int("C2").NotNull())
                .PrimaryKey(new Order() { Column = Sql.Name("C1"), Direction = Direction.Asc })
                .UniqueConstrainOn("UC_tbl", new Order { Column = Sql.Name("C2") })
                ;

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TEMPORARY TABLE \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" INTEGER NOT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ), CONSTRAINT \"UC_tbl\" UNIQUE ( \"C2\" ) );", command.CommandText);
        }
        #endregion
    }
}
