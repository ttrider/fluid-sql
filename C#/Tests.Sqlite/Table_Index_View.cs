// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.Sqlite
{
    [TestClass]
    public partial class Table_Index_View
    {
        public static SqliteProvider Provider = new SqliteProvider();

        [TestMethod]
        public IStatement CreateTable()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), false)
                .Columns(
                    TableColumn.Int("C1").NotNull().PrimaryKey().AutoIncrement()
                    , TableColumn.NVarChar("C2").Null().Default("foo"))
                ;

            var text = Provider.GenerateStatement(statement);


            Assert.IsNotNull(text);
            Assert.AreEqual(
                "CREATE TABLE \"tbl\" ( \"C1\" INTEGER PRIMARY KEY ASC AUTOINCREMENT NOT NULL, \"C2\" NVARCHAR NULL DEFAULT ( 'foo' ) );",
                text);
            return statement;
        }

        [TestMethod]
        public IStatement CreateTableConflicts()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl2"), false)
                .Columns(
                    TableColumn.Int("C1").NotNull().PrimaryKey(OnConflict.Ignore).AutoIncrement()
                    , TableColumn.NVarChar("C2").Null(OnConflict.Fail))
                ;

            var text = Provider.GenerateStatement(statement);


            Assert.IsNotNull(text);
            Assert.AreEqual(
                "CREATE TABLE \"tbl2\" ( \"C1\" INTEGER PRIMARY KEY ASC ON CONFLICT IGNORE AUTOINCREMENT NOT NULL, \"C2\" NVARCHAR NULL ON CONFLICT FAIL );",
                text);
            return statement;
        }


        [TestMethod]
        public IStatement CreateTableWithIndex()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl3"), false)
                .Columns(
                    TableColumn.Int("C1").NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .IndexOn("IX_tbl", new Order { Column = Sql.Name("C2") })
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual(
                "CREATE TABLE \"tbl3\" ( \"C1\" INTEGER NOT NULL, \"C2\" NVARCHAR NULL );\r\nCREATE INDEX \"IX_tbl\" ON \"tbl3\" ( \"C2\" ASC );",
                text);
            return statement;
        }

        [TestMethod]
        public IStatement CreateTableWithIndexIfNotExists()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), true)
                .Columns(
                    TableColumn.Int("C1").NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .IndexOn("IX_tbl", new Order { Column = Sql.Name("C2") })
                ;

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual(
                "CREATE TABLE IF NOT EXISTS \"tbl\" ( \"C1\" INTEGER NOT NULL, \"C2\" NVARCHAR NULL );\r\nCREATE INDEX IF NOT EXISTS \"IX_tbl\" ON \"tbl\" ( \"C2\" ASC );",
                text);
            return statement;
        }

        [TestMethod]
        public void CreateTableWith2IndexesIfNotExists()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), true)
                .Columns(
                    TableColumn.Int("C1").NotNull()
                    , TableColumn.NVarChar("C2").Null())
                    .IndexOn("IX_tbl", new Order { Column = Sql.Name("C2") })
                    .IndexOn("IX_tbl2", new Order { Column = Sql.Name("C1") })
                ;

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("CREATE TABLE IF NOT EXISTS \"tbl\" ( \"C1\" INTEGER NOT NULL, \"C2\" NVARCHAR NULL );\r\nCREATE INDEX IF NOT EXISTS \"IX_tbl\" ON \"tbl\" ( \"C2\" ASC );\r\nCREATE INDEX IF NOT EXISTS \"IX_tbl2\" ON \"tbl\" ( \"C1\" ASC );", text);
        }

        [TestMethod]
        public void DropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DROP TABLE \"some\".\"table\";", text);
        }

        [TestMethod]
        public void DropTableExists()
        {
            var statement = Sql.DropTable(Sql.Name("some.table"), true);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DROP TABLE IF EXISTS \"some\".\"table\";", text);
        }

        [TestMethod]
        public void CreateView()
        {
            var statement = Sql.CreateView(Sql.Name("foo"), Sql.Select.From("bar"));

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("CREATE VIEW \"foo\" AS SELECT * FROM \"bar\";", text);
        }

        [TestMethod]
        public void CreateTempView()
        {
            var statement = Sql.CreateTemporaryView(Sql.Name("foo"), Sql.Select.From("bar"));

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("CREATE TEMPORARY VIEW \"foo\" AS SELECT * FROM \"bar\";", text);
        }

        [TestMethod]
        public void AlertView()
        {
            var statement = Sql.AlterView(Sql.Name("foo"), Sql.Select.From("bar"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DROP VIEW IF EXISTS \"foo\";\r\nCREATE VIEW \"foo\" AS SELECT * FROM \"bar\";", text);
        }

        [TestMethod]
        public void DropView()
        {
            var statement = Sql.DropView(Sql.Name("foo"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DROP VIEW \"foo\";", text);
        }


        [TestMethod]
        public void CreateViewIfNotExists()
        {
            var statement = Sql.CreateView(Sql.Name("foo"), Sql.Select.From("bar"), true);

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("CREATE VIEW IF NOT EXISTS \"foo\" AS SELECT * FROM \"bar\";", text);
        }

        [TestMethod]
        public void CreateOrAlterView()
        {
            var statement = Sql.CreateOrAlterView(Sql.Name("foo"), Sql.Select.From("bar"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DROP VIEW IF EXISTS \"foo\";\r\nCREATE VIEW \"foo\" AS SELECT * FROM \"bar\";", text);
        }

        [TestMethod]
        public void DropViewIfExists()
        {
            var statement = Sql.DropView(Sql.Name("foo"), true);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DROP VIEW IF EXISTS \"foo\";", text);
        }

        [TestMethod]
        public void CreateIndex()
        {
            var statement = Sql.CreateIndex("if", "foo.bar")
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("CREATE INDEX \"if\" ON \"foo\".\"bar\" ( \"col1\" ASC, \"col2\" DESC );", text);
        }

        [TestMethod]
        public void CreateUniqueIndex()
        {
            var statement = Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("CREATE UNIQUE INDEX \"if\" ON \"foo\".\"bar\" ( \"col1\" ASC, \"col2\" DESC );", text);
        }


        [TestMethod]
        public void CreateUniqueIndexConditional()
        {
            var statement = Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                .Where(Sql.Name("col1").GreaterOrEqual(Sql.Scalar(123)));

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual(
                "CREATE UNIQUE INDEX \"if\" ON \"foo\".\"bar\" ( \"col1\" ASC, \"col2\" DESC ) WHERE \"col1\" >= 123;",
                text);
        }

        [TestMethod]
        public void DropIndex()
        {
            var statement = Sql.DropIndex("if", "foo.bar");

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("DROP INDEX \"foo\".\"if\";", text);
        }

        [TestMethod]
        public void DropIndexIfExists()
        {
            var statement = Sql.DropIndex("if", "foo.bar", true);

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("DROP INDEX IF EXISTS \"foo\".\"if\";", text);
        }

        [TestMethod]
        public void AlterIndexRebuild()
        {
            var statement = Sql.AlterIndex("if", "foo.bar").Rebuild();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("REINDEX \"foo\".\"if\";", text);
        }

        [TestMethod]
        public void AlterIndexAllRebuild()
        {
            var statement = Sql.AlterIndexAll("foo.bar").Rebuild();

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("REINDEX \"foo\".\"bar\";", text);
        }

        [TestMethod]
        public void ReIndex()
        {
            var statement = Sql.Reindex("if", "foo.bar");

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("REINDEX \"foo\".\"if\";", text);
        }

        [TestMethod]
        public void ReIndexAll()
        {
            var statement = Sql.Reindex("foo.bar");

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("REINDEX \"foo\".\"bar\";", text);
        }
    }
}