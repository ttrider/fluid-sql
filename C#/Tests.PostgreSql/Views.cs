using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.PostgreSql
{
    /// <summary>
    /// Summary description for Views
    /// </summary>
    [TestClass]
    public class Views : PostgreSqlProviderTests
    {
        #region

        [TestMethod]
        public void CreateView()
        {
            var statement = Sql.CreateView(Sql.Name("view1"), Sql.Select.From("tbl1"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE VIEW \"view1\" AS SELECT * FROM \"tbl1\";", command.CommandText);
        }

        [TestMethod]
        public void DropView()
        {
            var statement = Sql.DropView(Sql.Name("view1"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP VIEW \"view1\";", command.CommandText);
        }

        [TestMethod]
        public void DropViewIfExists()
        {
            var statement = Sql.DropView(Sql.Name("view1"), true);
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP VIEW IF EXISTS \"view1\";", command.CommandText);
        }

        [TestMethod]
        public void AlertView()
        {
            var statement = Sql.AlterView(Sql.Name("foo"), Sql.Select.From("target_tbl"));
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN DROP VIEW IF EXISTS \"foo\";\r\nCREATE VIEW \"foo\" AS SELECT * FROM \"target_tbl\";\r\nEND\r\n$do$;", command.CommandText);
        }

        [TestMethod]
        public void CreateOrAlterView()
        {
            var statement = Sql.CreateOrAlterView(Sql.Name("foo"), Sql.Select.From("target_tbl"));
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DO\r\n$do$\r\nBEGIN DROP VIEW IF EXISTS \"foo\";\r\nCREATE VIEW \"foo\" AS SELECT * FROM \"target_tbl\";\r\nEND\r\n$do$;", command.CommandText);
        }
        #endregion
    }
}
