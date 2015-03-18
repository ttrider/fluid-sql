using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace FluidSqlTests
{
    [TestClass]
    public class Create
    {
        [TestMethod]
        public void CreateTable()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), false)
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .PrimaryKey("PK_tbl", new Order() { Column = Sql.Name("C1"), Direction = Direction.Asc })
                ;

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE [tbl] ([C1] INT NOT NULL IDENTITY (1, 1), [C2] NVARCHAR(MAX) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ([C1] ASC ));", command.CommandText);
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

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE [tbl] ([C1] INT NOT NULL IDENTITY (1, 1), [C2] NVARCHAR(MAX) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ([C1] ASC ), CONSTRAINT [UC_tbl] UNIQUE NONCLUSTERED ([C2] ASC ));", command.CommandText);
        }

        [TestMethod]
        public void CreateTableWithConstrainsClustered()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), false)
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .PrimaryKey("PK_tbl", new Order { Column = Sql.Name("C1"), Direction = Direction.Asc })
                .UniqueConstrainOn("UC_tbl", true, new Order { Column = Sql.Name("C2") })
                ;

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE [tbl] ([C1] INT NOT NULL IDENTITY (1, 1), [C2] NVARCHAR(MAX) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ([C1] ASC ), CONSTRAINT [UC_tbl] UNIQUE CLUSTERED ([C2] ASC ));", command.CommandText);
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

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE TABLE [tbl] ([C1] INT NOT NULL IDENTITY (1, 1), [C2] NVARCHAR(MAX) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ([C1] ASC ));CREATE NONCLUSTERED INDEX [IX_tbl] ON [tbl] ([C2] ASC);", command.CommandText);
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

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF OBJECT_ID(N'[tbl]',N'U') IS NULL  BEGIN; CREATE TABLE [tbl] ([C1] INT NOT NULL IDENTITY (1, 1), [C2] NVARCHAR(MAX) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ([C1] ASC ));CREATE NONCLUSTERED INDEX [IX_tbl] ON [tbl] ([C2] ASC); END;", command.CommandText);
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

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DECLARE @tbl TABLE ([C1] INT NOT NULL IDENTITY (1, 1), [C2] INT NOT NULL, PRIMARY KEY ([C1] ASC ), UNIQUE NONCLUSTERED ([C2] ASC ));", command.CommandText);
        }

        [TestMethod]
        public void CreateView()
        {
            var statement = Sql.CreateView(Sql.Name("foo"), Sql.Select.From("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE VIEW [foo] AS SELECT * FROM [bar];", command.CommandText);
        }

        [TestMethod]
        public void AlertView()
        {
            var statement = Sql.AlterView(Sql.Name("foo"), Sql.Select.From("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER VIEW [foo] AS SELECT * FROM [bar];", command.CommandText);
        }

        [TestMethod]
        public void DropView()
        {
            var statement = Sql.DropView(Sql.Name("foo"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP VIEW [foo];", command.CommandText);
        }
    }
}
