// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.FluidSql
{
    public class Delete
    {
        [Fact]
        public void Delete1()
        {
            var statement = Sql.Delete.Top(1).From(Sql.Name("foo.bar"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DELETE TOP ( 1 ) FROM [foo].[bar];", command.CommandText);
        }

        [Fact]
        public void Delete1P()
        {
            var statement = Sql.Delete.Top(1, true).From(Sql.NameAs("foo.bar", "f"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DELETE TOP ( 1 ) PERCENT [f] FROM [foo].[bar] AS [f];", command.CommandText);
        }

        [Fact]
        public void DeleteOutput()
        {
            var statement = Sql.Delete.From(Sql.Name("foo.bar")).Output(Sql.Name("INSERTED.*"), Sql.Name("DELETED.*"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DELETE FROM [foo].[bar] OUTPUT [INSERTED].*, [DELETED].*;", command.CommandText);
        }

        [Fact]
        public void DeleteOutput2()
        {
            var statement = Sql.Delete.From(Sql.NameAs("foo.bar", "s"))
                .Output(Sql.Name("INSERTED.*"), Sql.Name("DELETED.*"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DELETE [s] OUTPUT [INSERTED].*, [DELETED].* FROM [foo].[bar] AS [s];", command.CommandText);
        }

        [Fact]
        public void DeleteWhere()
        {
            var statement = Sql.Delete.From(Sql.Name("foo.bar")).Where(Sql.Name("f").IsEqual(Sql.Name("b")));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DELETE FROM [foo].[bar] WHERE [f] = [b];", command.CommandText);
        }

        [Fact]
        public void DeleteJoin()
        {
            var sourceTable = Sql.NameAs("foo", "bar");

            var statement = Sql.Delete.From(sourceTable)
                .InnerJoin(Sql.NameAs("reftable", "ref"), Sql.Name("bar", "id").IsEqual(Sql.Name("ref", "id")))
                .Top(5)
                .Output(Sql.Name("deleted", "*"))
                .Where(Sql.Name("ref", "id").NotEqual(Sql.Scalar(10)));


            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "DELETE TOP ( 5 ) [bar] OUTPUT [deleted].* FROM [foo] AS [bar] INNER JOIN [reftable] AS [ref] ON [bar].[id] = [ref].[id] WHERE [ref].[id] <> 10;",
                command.CommandText);
        }
    }
}