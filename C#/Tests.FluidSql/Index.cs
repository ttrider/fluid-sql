using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace FluidSqlTests
{
    [TestClass]
    public class Index : SqlProviderTests
    {
        [TestMethod]
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

        [TestMethod]
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
                .OnColumn(new List<Order> {Sql.Order("col1"), Sql.Order("col2", Direction.Desc)})
                .Include("col3")
                .Include("col4"),
                "CREATE UNIQUE CLUSTERED INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] DESC ) INCLUDE ( [col3], [col4] ) ;");

            AssertSql(Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn("col1","col2")
                .Include("col3")
                .Include("col4"),
                "CREATE UNIQUE CLUSTERED INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] ASC ) INCLUDE ( [col3], [col4] ) ;");
        }


        [TestMethod]
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

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE UNIQUE CLUSTERED INDEX [if] ON [foo].[bar] ( [col1] ASC, [col2] DESC ) INCLUDE ( [col3], [col4] ) WHERE [col1] >= 123 ;", command.CommandText);

        }

        [TestMethod]
        public void DropIndex()
        {
            var statement = Sql.DropIndex("if", "foo.bar");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP INDEX [if] ON [foo].[bar];", command.CommandText);

        }

        [TestMethod]
        public void AlterIndexRebuild()
        {
            var statement = Sql.AlterIndex("if", "foo.bar").Rebuild();

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER INDEX [if] ON [foo].[bar] REBUILD;", command.CommandText);

        }

        [TestMethod]
        public void AlterIndexAllRebuild()
        {
            var statement = Sql.AlterIndexAll("foo.bar").Rebuild();

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER INDEX ALL ON [foo].[bar] REBUILD;", command.CommandText);

        }

        [TestMethod]
        public void AlterIndexDisable()
        {
            var statement = Sql.AlterIndex("if", "foo.bar").Disable();

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER INDEX [if] ON [foo].[bar] DISABLE;", command.CommandText);

        }
        [TestMethod]
        public void AlterIndexReorg()
        {
            var statement = Sql.AlterIndex("if", "foo.bar").Reorganize();

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER INDEX [if] ON [foo].[bar] REORGANIZE;", command.CommandText);

        }

    }
}
