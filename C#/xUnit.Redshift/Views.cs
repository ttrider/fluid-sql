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
    public class Views : RedshiftSqlProviderTests
    {
        [Fact]
        public void CreateView()
        {
            var statement = Sql.CreateView(Sql.Name("view1"), Sql.Select.From("tbl1"));
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("CREATE VIEW \"view1\" AS SELECT * FROM \"tbl1\";", command.CommandText);
        }

        [Fact]
        public void DropView()
        {
            var statement = Sql.DropView(Sql.Name("view1"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DROP VIEW \"view1\";", command.CommandText);
        }

        [Fact]
        public void DropViewIfExists()
        {
            var statement = Sql.DropView(Sql.Name("view1"), true);
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DROP VIEW IF EXISTS \"view1\";", command.CommandText);
        }

        [Fact]
        public void AlertView()
        {
            var statement = Sql.AlterView(Sql.Name("foo"), Sql.Select.From("target_tbl"));
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DO\r\n$do$\r\nBEGIN DROP VIEW IF EXISTS \"foo\";\r\nCREATE VIEW \"foo\" AS SELECT * FROM \"target_tbl\";\r\nEND\r\n$do$;", command.CommandText);
        }

        [Fact]
        public void CreateOrAlterView()
        {
            var statement = Sql.CreateOrAlterView(Sql.Name("foo"), Sql.Select.From("target_tbl"));
            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DO\r\n$do$\r\nBEGIN DROP VIEW IF EXISTS \"foo\";\r\nCREATE VIEW \"foo\" AS SELECT * FROM \"target_tbl\";\r\nEND\r\n$do$;", command.CommandText);
        }
    }
}
