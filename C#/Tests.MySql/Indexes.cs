using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for Indexes
    /// </summary>
    [TestClass]
    public class Indexes : MySqlProviderTests
    {
        [TestMethod]
        public void CreateIndex()
        {
            var statement = Sql.CreateIndex("if", "test_db.test_select_tbl")
                .OnColumn("number_value")
                .OnColumn(Sql.Name("text_value"), Direction.Desc)
                ;

            var text = Provider.GenerateStatement((CreateIndexStatement)statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("CREATE INDEX `if` ON `test_db`.`test_select_tbl` ( `number_value` ASC, `text_value` DESC );", text);

        }

        [TestMethod]
        public void AlterIndex()
        {
            var statement = Sql.AlterIndex("if", "test_db.test_select_tbl")
                .OnColumn("number_value")
                .OnColumn(Sql.Name("text_value"), Direction.Desc)
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DROP INDEX `if` ON `test_db`.`test_select_tbl`;\r\nCREATE INDEX `if` ON `test_db`.`test_select_tbl` ( `number_value` ASC, `text_value` DESC );", text);

        }

        [TestMethod]
        public void CreateUniqueIndex()
        {
            var statement = Sql.CreateIndex("if", "test_db.test_insert_db")
                .Unique()
                .Clustered()
                .OnColumn("number_value")
                .OnColumn(Sql.Name("text_value"), Direction.Desc)
                ;

            var text = Provider.GenerateStatement((CreateIndexStatement)statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("CREATE UNIQUE INDEX `if` ON `test_db`.`test_insert_db` ( `number_value` ASC, `text_value` DESC );", text);

        }

        /*[TestMethod]
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

        }*/

        [TestMethod]
        public void DropIndex()
        {
            var statement = Sql.DropIndex("if", "test_select_tbl");

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("DROP INDEX `if` ON `test_select_tbl`;", text);

        }

        /*[TestMethod]
        public void DropIndexIfExists()
        {
            var statement = Sql.DropIndex("if", "foo.bar", true);

            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("DROP INDEX IF EXISTS \"foo\".\"if\";", text);

        }*/

    }
}
