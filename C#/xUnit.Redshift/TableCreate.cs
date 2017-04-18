// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.Redshift
{
    public class TableCreate : RedshiftSqlProviderTests
    {
        [Fact]
        public void CreateEmptyTable()
        {
            var statement = Sql.CreateTable(Sql.Name("table1"), false);
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE TABLE \"table1\" ( );", command.CommandText);
        }

        [Fact]
        public void CreateTableWithColumns()
        {
            var statement = Sql.CreateTable(Sql.Name("table1"), false)
                .Columns(TableColumn.Int("id"), TableColumn.Text("text1"));
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE TABLE \"table1\" ( \"id\" INTEGER, \"text1\" TEXT );", command.CommandText);
        }

        [Fact]
        public void CreateTemporaryTableWithColumns()
        {
            var statement = Sql.CreateTemporaryTable(Sql.Name("table1"), false)
                .Columns(TableColumn.Int("id"), TableColumn.Text("text1"));
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE TEMPORARY TABLE \"table1\" ( \"id\" INTEGER, \"text1\" TEXT );", command.CommandText);
        }

        [Fact]
        public void CreateTableWithColumnsOptions()
        {
            var statement = Sql.CreateTable(Sql.Name("table1"), false)
                .Columns(
                    TableColumn.Int("id").NotNull().Default(100)
                    ,TableColumn.Text("text1").Null());
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE TABLE \"table1\" ( \"id\" INTEGER NOT NULL DEFAULT 100, \"text1\" TEXT NULL );", command.CommandText); 
        }

        [Fact]
        public void CreateTableWithPrimaryKey()
        {
            var statement = Sql.CreateTable(Sql.Name("table1"), false)
                .Columns(
                    TableColumn.Int("C1").NotNull()
                    , TableColumn.Text("C2").Null())
                .PrimaryKey("PK_tbl", new Order { Column = Sql.Name("C1"), Direction = Direction.Asc });
                
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE TABLE \"table1\" ( \"C1\" INTEGER NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ) );", command.CommandText);
        }

        #region Unit tests from FluidSql
        [Fact]
        public void CreateTable()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), false)
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .PrimaryKey("PK_tbl", new Order() { Column = Sql.Name("C1"), Direction = Direction.Asc })
                ;

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE TABLE \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ) );", command.CommandText);
        }

        [Fact]
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

            Assert.NotNull(command);
            Assert.Equal("CREATE TABLE \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ), CONSTRAINT \"UC_tbl\" UNIQUE ( \"C2\" ) );", command.CommandText);
        }

        [Fact]
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

            //Assert.Equal("", command.CommandText);

            Assert.NotNull(command);
            Assert.Equal("CREATE TABLE \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ) );", command.CommandText);
        }

        [Fact]
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

            Assert.NotNull(command);
            Assert.Equal("CREATE TABLE IF NOT EXISTS \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" TEXT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ) );", command.CommandText);
        }

        [Fact]
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

            Assert.NotNull(command);
            Assert.Equal("CREATE TEMPORARY TABLE \"tbl\" ( \"C1\" SERIAL NOT NULL, \"C2\" INTEGER NOT NULL, CONSTRAINT \"PK_tbl\" PRIMARY KEY ( \"C1\" ), CONSTRAINT \"UC_tbl\" UNIQUE ( \"C2\" ) );", command.CommandText);
        }
        #endregion
    }
}
