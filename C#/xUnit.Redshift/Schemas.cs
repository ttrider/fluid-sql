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
    public class Schemas : RedshiftSqlProviderTests
    {
        [Fact]
        public void CreateSimpleSchema()
        {
            var statement = Sql.CreateSchema(Sql.Name("CM"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE SCHEMA \"CM\";", command.CommandText);
        }

        [Fact]
        public void CreateSimpleSchemaIfNotExist()
        {
            var statement = Sql.CreateSchema(Sql.Name("CM"), true);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE SCHEMA IF NOT EXISTS \"CM\";", command.CommandText);
        }

        [Fact]
        public void CreateSimpleSchemaAuthorization()
        {
            var statement = Sql.CreateSchema(Sql.Name("CM")).Authorization("admin");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE SCHEMA \"CM\" AUTHORIZATION \"admin\";", command.CommandText);
        }

        [Fact]
        public void DropSchema()
        {
            var statement = Sql.DropSchema(Sql.Name("CM"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DROP SCHEMA \"CM\";", command.CommandText);
        }

        [Fact]
        public void DropSchemaIfExists()
        {
            var statement = Sql.DropSchema(Sql.Name("CM"), true);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DROP SCHEMA IF EXISTS \"CM\";", command.CommandText);
        }

        [Fact]
        public void DropSchemaCascade()
        {
            var statement = Sql.DropSchema(Sql.Name("CM")).Cascade();

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DROP SCHEMA \"CM\" CASCADE;", command.CommandText);
        }

        [Fact]
        public void DropSchemaRestrict()
        {
            var statement = Sql.DropSchema(Sql.Name("CM")).Restrict();

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DROP SCHEMA \"CM\" RESTRICT;", command.CommandText);
        }

        [Fact]
        public void AlterSchemaRenameTo()
        {
            var statement = Sql.AlterSchema(Sql.Name("CM")).RenameTo("CM1");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("ALTER SCHEMA \"CM\" RENAME TO \"CM1\";", command.CommandText);
        }

        [Fact]
        public void AlterSchemaRenameToName()
        {
            var statement = Sql.AlterSchema(Sql.Name("CM")).RenameTo(Sql.Name("CM1"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("ALTER SCHEMA \"CM\" RENAME TO \"CM1\";", command.CommandText);
        }

        [Fact]
        public void AlterSchemaOwnerTo()
        {
            var statement = Sql.AlterSchema(Sql.Name("CM")).OwnerTo("admin");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("ALTER SCHEMA \"CM\" OWNER TO \"admin\";", command.CommandText);
        }

        [Fact]
        public void AlterSchemaOwnerToName()
        {
            var statement = Sql.AlterSchema(Sql.Name("CM")).OwnerTo(Sql.Name("admin"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("ALTER SCHEMA \"CM\" OWNER TO \"admin\";", command.CommandText);
        }
    }
}
