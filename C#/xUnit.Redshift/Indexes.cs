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
    public class Indexes : RedshiftSqlProviderTests
    {
        [Fact]
        public void CreateIndex()
        {
            var statement = Sql.CreateIndex("if", "foo.bar")
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                ;

            var text = Provider.GenerateStatement((CreateIndexStatement)statement);

            Assert.Equal("", text);

            //Assert.NotNull(text);
            //Assert.Equal("CREATE INDEX \"if\" ON \"foo\".\"bar\" ( \"col1\" ASC, \"col2\" DESC );", text);

        }

        [Fact]
        public void AlterIndex()
        {
            var statement = Sql.AlterIndex("if", "foo.bar")
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.Equal("", text);

            //Assert.NotNull(text);
            //Assert.Equal("DROP INDEX IF EXISTS \"if\";\r\nCREATE INDEX \"if\" ON \"foo\".\"bar\" ( \"col1\" ASC, \"col2\" DESC );", text);

        }

        [Fact]
        public void CreateUniqueIndex()
        {
            var statement = Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                ;

            var text = Provider.GenerateStatement((CreateIndexStatement)statement);

            Assert.Equal("", text);

            //Assert.NotNull(text);
            //Assert.Equal("CREATE UNIQUE INDEX \"if\" ON \"foo\".\"bar\" ( \"col1\" ASC, \"col2\" DESC );", text);

        }

        [Fact]
        public void CreateUniqueIndexConditional()
        {
            var statement = Sql.CreateIndex("if", "foo.bar")
                .Unique()
                .Clustered()
                .OnColumn("col1")
                .OnColumn(Sql.Name("col2"), Direction.Desc)
                .Where(Sql.Name("col1").GreaterOrEqual(Sql.Scalar(123)));

            var text = Provider.GenerateStatement(statement);

            Assert.Equal("", text);

            //Assert.NotNull(text);
            //Assert.Equal("CREATE UNIQUE INDEX \"if\" ON \"foo\".\"bar\" ( \"col1\" ASC, \"col2\" DESC ) WHERE \"col1\" >= 123;", text);

        }

        [Fact]
        public void DropIndex()
        {
            var statement = Sql.DropIndex("index1", "tbl1");

            var text = Provider.GenerateStatement(statement);

            Assert.Equal("", text);

            //Assert.NotNull(text);
            //Assert.Equal("DROP INDEX \"tbl1\".\"index1\";", text);

        }

        [Fact]
        public void DropIndexIfExists()
        {
            var statement = Sql.DropIndex("if", "foo.bar", true);

            var text = Provider.GenerateStatement(statement);

            Assert.Equal("", text);

            //Assert.NotNull(text);
            //Assert.Equal("DROP INDEX IF EXISTS \"foo\".\"if\";", text);

        }

    }
}
