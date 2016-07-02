using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.PostgreSql
{
    /// <summary>
    /// Summary description for Indexes
    /// </summary>
    [TestClass]
    public class Indexes : PostgreSqlProviderTests
    {
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
            Assert.AreEqual("CREATE UNIQUE INDEX \"if\" ON \"foo\".\"bar\" ( \"col1\" ASC, \"col2\" DESC ) WHERE \"col1\" >= 123;", text);

        }

        [TestMethod]
        public void DropIndex()
        {
            var statement = Sql.DropIndex("index1", "tbl1");

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("DROP INDEX \"tbl1\".\"index1\";", text);

        }

        [TestMethod]
        public void DropIndexIfExists()
        {
            var statement = Sql.DropIndex("if", "foo.bar", true);

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("DROP INDEX IF EXISTS \"foo\".\"if\";", text);

        }

    }
}
