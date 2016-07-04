using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.PostgreSql
{
    /// <summary>
    /// Summary description for Schemas
    /// </summary>
    [TestClass]
    public class Schemas : PostgreSqlProviderTests
    {
        [TestMethod]
        public void CreateSimpleSchema()
        {
            var statement = Sql.CreateSchema(Sql.Name("CM"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE SCHEMA \"CM\";", command.CommandText);
        }

        [TestMethod]
        public void CreateSimpleSchemaIfNotExist()
        {
            var statement = Sql.CreateSchema(Sql.Name("CM"), true);

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE SCHEMA IF NOT EXISTS \"CM\";", command.CommandText);
        }

        [TestMethod]
        public void CreateSimpleSchemaAuthorization()
        {
            var statement = Sql.CreateSchema(Sql.Name("CM")).Authorization("admin");

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE SCHEMA \"CM\" AUTHORIZATION \"admin\";", command.CommandText);
        }

        [TestMethod]
        public void DropSchema()
        {
            var statement = Sql.DropSchema(Sql.Name("CM"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP SCHEMA \"CM\";", command.CommandText);
        }

        [TestMethod]
        public void DropSchemaIfExists()
        {
            var statement = Sql.DropSchema(Sql.Name("CM"), true);

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP SCHEMA IF EXISTS \"CM\";", command.CommandText);
        }

        [TestMethod]
        public void DropSchemaCascade()
        {
            var statement = Sql.DropSchema(Sql.Name("CM")).Cascade();

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP SCHEMA \"CM\" CASCADE;", command.CommandText);
        }

        [TestMethod]
        public void DropSchemaRestrict()
        {
            var statement = Sql.DropSchema(Sql.Name("CM")).Restrict();

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP SCHEMA \"CM\" RESTRICT;", command.CommandText);
        }

        [TestMethod]
        public void AlterSchemaRenameTo()
        {
            var statement = Sql.AlterSchema(Sql.Name("CM")).RenameTo("CM1");

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER SCHEMA \"CM\" RENAME TO \"CM1\";", command.CommandText);
        }

        [TestMethod]
        public void AlterSchemaRenameToName()
        {
            var statement = Sql.AlterSchema(Sql.Name("CM")).RenameTo(Sql.Name("CM1"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER SCHEMA \"CM\" RENAME TO \"CM1\";", command.CommandText);
        }

        [TestMethod]
        public void AlterSchemaOwnerTo()
        {
            var statement = Sql.AlterSchema(Sql.Name("CM")).OwnerTo("admin");

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER SCHEMA \"CM\" OWNER TO \"admin\";", command.CommandText);
        }

        [TestMethod]
        public void AlterSchemaOwnerToName()
        {
            var statement = Sql.AlterSchema(Sql.Name("CM")).OwnerTo(Sql.Name("admin"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("ALTER SCHEMA \"CM\" OWNER TO \"admin\";", command.CommandText);
        }
    }
}
