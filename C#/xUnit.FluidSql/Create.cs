// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.FluidSql
{
    public class Create
    {
        [Fact]
        public void CreateTable()
        {
            var statement = Sql.CreateTable(Sql.Name("tbl"), false)
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.NVarChar("C2").Null())
                .PrimaryKey("PK_tbl", new Order { Column = Sql.Name("C1"), Direction = Direction.Asc })
                ;

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "CREATE TABLE [tbl] ( [C1] INT NOT NULL IDENTITY ( 1, 1 ), [C2] NVARCHAR ( MAX ) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ( [C1] ASC ) );",
                command.CommandText);
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

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "CREATE TABLE [tbl] ( [C1] INT NOT NULL IDENTITY ( 1, 1 ), [C2] NVARCHAR ( MAX ) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ( [C1] ASC ), CONSTRAINT [UC_tbl] UNIQUE NONCLUSTERED ( [C2] ASC ) );",
                command.CommandText);
        }

        [Fact]
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

            Assert.NotNull(command);
            Assert.Equal(
                "CREATE TABLE [tbl] ( [C1] INT NOT NULL IDENTITY ( 1, 1 ), [C2] NVARCHAR ( MAX ) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ( [C1] ASC ), CONSTRAINT [UC_tbl] UNIQUE CLUSTERED ( [C2] ASC ) );",
                command.CommandText);
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

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "CREATE TABLE [tbl] ( [C1] INT NOT NULL IDENTITY ( 1, 1 ), [C2] NVARCHAR ( MAX ) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ( [C1] ASC ) );\r\nCREATE NONCLUSTERED INDEX [IX_tbl] ON [tbl] ( [C2] ASC ) ;",
                command.CommandText);
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

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "IF OBJECT_ID ( N'[tbl]', N'U' ) IS NULL\r\nBEGIN;\r\nCREATE TABLE [tbl] ( [C1] INT NOT NULL IDENTITY ( 1, 1 ), [C2] NVARCHAR ( MAX ) NULL, CONSTRAINT [PK_tbl] PRIMARY KEY ( [C1] ASC ) );\r\nCREATE NONCLUSTERED INDEX [IX_tbl] ON [tbl] ( [C2] ASC ) ;\r\nEND;",
                command.CommandText);
        }

        [Fact]
        public void CreateTableVariable()
        {
            var statement = Sql.CreateTableVariable(Sql.Name("tbl"))
                .Columns(
                    TableColumn.Int("C1").Identity().NotNull()
                    , TableColumn.Int("C2").NotNull())
                .PrimaryKey(new Order { Column = Sql.Name("C1"), Direction = Direction.Asc })
                .UniqueConstrainOn("UC_tbl", new Order { Column = Sql.Name("C2") })
                ;

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "DECLARE @tbl TABLE ( [C1] INT NOT NULL IDENTITY ( 1, 1 ), [C2] INT NOT NULL, PRIMARY KEY ( [C1] ASC ), UNIQUE NONCLUSTERED ( [C2] ASC ) );",
                command.CommandText);
        }

        [Fact]
        public void CreateView()
        {
            var statement = Sql.CreateView(Sql.Name("foo"), Sql.Select.From("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE VIEW [foo] AS SELECT * FROM [bar];", command.CommandText);
        }

        [Fact]
        public void AlertView()
        {
            var statement = Sql.AlterView(Sql.Name("foo"), Sql.Select.From("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("ALTER VIEW [foo] AS SELECT * FROM [bar];", command.CommandText);
        }

        [Fact]
        public void DropView()
        {
            var statement = Sql.DropView(Sql.Name("foo"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DROP VIEW [foo];", command.CommandText);
        }

        [Fact]
        public void CreateViewIfNotExists()
        {
            var statement = Sql.CreateView(Sql.Name("foo"), Sql.Select.From("bar"), true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF OBJECT_ID ( N'[foo]' ) IS NULL EXEC (N' CREATE VIEW [foo] AS SELECT * FROM [bar]' );",
                command.CommandText);
        }

        [Fact]
        public void CreateOrAlterView()
        {
            var statement = Sql.CreateOrAlterView(Sql.Name("foo"), Sql.Select.From("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "IF OBJECT_ID ( N'[foo]' ) IS NULL EXEC (N' CREATE VIEW [foo] AS SELECT * FROM [bar]' );\r\nELSE EXEC (N' ALTER VIEW [foo] AS SELECT * FROM [bar]' );",
                command.CommandText);
        }

        [Fact]
        public void DropViewIfExists()
        {
            var statement = Sql.DropView(Sql.Name("foo"), true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF OBJECT_ID ( N'[foo]' ) IS NOT NULL EXEC (N' DROP VIEW [foo];' );", command.CommandText);
        }

        [Fact]
        public void CreateSchema()
        {
            var statement = Sql.CreateSchema(Sql.Name("CM"), true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = N'CM' ) BEGIN EXEC (N' CREATE SCHEMA [CM]' ) END;",
                command.CommandText);
        }

        [Fact]
        public void CreateSchema_Always()
        {
            var statement = Sql.CreateSchema(Sql.Name("CM"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("EXEC (N' CREATE SCHEMA [CM]' );", command.CommandText);
        }

        [Fact]
        public void CreateSchema_Auth()
        {
            var statement = Sql.CreateSchema(Sql.Name("CM"), true).Authorization("dbo");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "IF NOT EXISTS ( SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = N'CM' ) BEGIN EXEC (N' CREATE SCHEMA [CM] AUTHORIZATION [dbo]' ) END;",
                command.CommandText);
        }

        [Fact]
        public void DropSchema()
        {
            var statement = Sql.DropSchema(Sql.Name("CM"), true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "IF EXISTS ( SELECT * FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = N'CM' ) BEGIN EXEC (N' DROP SCHEMA [CM]' ) END;",
                command.CommandText);
        }

        [Fact]
        public void DropSchema_Always()
        {
            var statement = Sql.DropSchema(Sql.Name("CM"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("EXEC (N' DROP SCHEMA [CM]' );", command.CommandText);
        }
    }
}