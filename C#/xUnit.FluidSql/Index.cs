// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using TTRider.FluidSql;
using Xunit;

namespace xUnit.FluidSql
{
    public class Index : SqlProviderTests
    {
        [Fact]
        public void CreateIndex()
        {
            AssertSql(Sql.CreateIndex("if", "foo.bar")
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                .Include("col3")
                .Include("col4"),
                "CREATE INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] DESC ) INCLUDE ( [col3], [col4] ) ;");

            AssertSql(Sql.CreateIndex("if", "foo.bar")
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                .Include("col3", "col4"),
                "CREATE INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] DESC ) INCLUDE ( [col3], [col4] ) ;");

            AssertSql(Sql.CreateIndex("if", "foo.bar")
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                .Include(Sql.Name("col3"), Sql.Name("col4")),
                "CREATE INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] DESC ) INCLUDE ( [col3], [col4] ) ;");
        }

        [Fact]
        public void CreateUniqueIndex()
        {
            AssertSql(Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                .Include("col3")
                .Include("col4"),
                "CREATE UNIQUE CLUSTERED INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] DESC ) INCLUDE ( [col3], [col4] ) ;");


            AssertSql(Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn(Sql.Order("col1"), Sql.Order("col2", Direction.Desc))
                .Include("col3")
                .Include("col4"),
                "CREATE UNIQUE CLUSTERED INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] DESC ) INCLUDE ( [col3], [col4] ) ;");

            AssertSql(Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn(new List<Order> { Sql.Order("col1"), Sql.Order("col2", Direction.Desc) })
                .Include("col3")
                .Include("col4"),
                "CREATE UNIQUE CLUSTERED INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] DESC ) INCLUDE ( [col3], [col4] ) ;");

            AssertSql(Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn("col1", "col2")
                .Include("col3")
                .Include("col4"),
                "CREATE UNIQUE CLUSTERED INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] ASC ) INCLUDE ( [col3], [col4] ) ;");
        }

        [Fact]
        public void CreateUniqueIndexConditional()
        {
            var statement = Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                .Include("col3")
                .Include("col4")
                .Where(Sql.Name("col1").GreaterOrEqual(Sql.Scalar(123)));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "CREATE UNIQUE CLUSTERED INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] DESC ) INCLUDE ( [col3], [col4] ) WHERE [col1] >= 123 ;",
                command.CommandText);
        }

        [Fact]
        public void DropIndex()
        {
            var statement = Sql.DropIndex("if", "foo.bar");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DROP INDEX [if] ON [foo].[bar];", command.CommandText);
        }

        [Fact]
        public void AlterIndexRebuild()
        {
            var statement = Sql.AlterIndex("if", "foo.bar").Rebuild();

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("ALTER INDEX [if] ON [foo].[bar] REBUILD;", command.CommandText);
        }

        [Fact]
        public void AlterIndexAllRebuild()
        {
            var statement = Sql.AlterIndexAll("foo.bar").Rebuild();

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("ALTER INDEX ALL ON [foo].[bar] REBUILD;", command.CommandText);
        }

        [Fact]
        public void AlterIndexDisable()
        {
            var statement = Sql.AlterIndex("if", "foo.bar").Disable();

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("ALTER INDEX [if] ON [foo].[bar] DISABLE;", command.CommandText);
        }

        [Fact]
        public void AlterIndexReorg()
        {
            var statement = Sql.AlterIndex("if", "foo.bar").Reorganize();

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("ALTER INDEX [if] ON [foo].[bar] REORGANIZE;", command.CommandText);
        }
    }
}