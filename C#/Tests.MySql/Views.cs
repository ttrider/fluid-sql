using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TTRider.FluidSql;


namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for Views
    /// </summary>
    [TestClass]
    public class Views : MySqlProviderTests
    {
        [TestMethod]
        public void CreateView()
        {
            var statement = Sql.CreateView(Sql.Name("view1"), Sql.Select.From("test_select_tbl"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE VIEW `view1` AS SELECT * FROM `test_select_tbl`;", command.CommandText);
        }

        [TestMethod]
        public void DropView()
        {
            var statement = Sql.DropView(Sql.Name("view1"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP VIEW `view1`;", command.CommandText);
        }

        [TestMethod]
        public void DropViewIfExists()
        {
            var statement = Sql.DropView(Sql.Name("view1"), true);
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP VIEW IF EXISTS `view1`;", command.CommandText);
        }

        [TestMethod]
        public void AlertView()
        {
            var statement = Sql.AlterView(Sql.Name("view1"), Sql.Select.From("test_select2_tbl"));
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER VIEW `view1` AS SELECT * FROM `test_select2_tbl`;", command.CommandText);
        }

        [TestMethod]
        public void CreateOrAlterView()
        {
            var statement = Sql.CreateOrAlterView(Sql.Name("view1"), Sql.Select.From("test_select_tbl"));
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE OR REPLACE VIEW `view1` AS SELECT * FROM `test_select_tbl`;", command.CommandText);
        }
    }
}
