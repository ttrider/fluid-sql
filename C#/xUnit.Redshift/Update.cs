// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using TTRider.FluidSql;
using Xunit;

namespace xUnit.Redshift
{
    public class Update : RedshiftSqlProviderTests
    {
        [Fact]
        public void UpdateWhere()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Name("val0")))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);

            Assert.Equal("UPDATE \"table1\" SET \"id\" = 100 WHERE \"value\" = \"val0\";", text);
        }

        [Fact]
        public void UpdateFrom()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100)).From(Sql.Name("table2"))
                .Where(Sql.Name("value").IsEqual(Sql.Name("val0")))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);

            Assert.Equal("UPDATE \"table1\" SET \"id\" = 100 FROM \"table2\" WHERE \"value\" = \"val0\";", text);
        }

        [Fact]
        public void UpdateAlias()
        {
            var statement = Sql.Update(Sql.Name("table1").As("tbl1"))
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Name("val0")))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);

            Assert.Equal("UPDATE \"table1\" AS \"tbl1\" SET \"id\" = 100 WHERE \"value\" = \"val0\";", text);
        }

        [Fact]
        public void UpdateReturning()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Name("val0"))).Output(Sql.Name("id"), Sql.Name("value"))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);

            Assert.Equal("UPDATE \"table1\" SET \"id\" = 100 WHERE \"value\" = \"val0\" RETURNING \"id\", \"value\";", text);
        }

        [Fact]
        public void UpdateReturningStar()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Name("val0"))).Output(Sql.Star())
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);

            Assert.Equal("UPDATE \"table1\" SET \"id\" = 100 WHERE \"value\" = \"val0\" RETURNING *;", text);
        }

        [Fact]
        public void UpdateReturningAlias()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Scalar("val0"))).Output(Sql.Name("id").As("aid"));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);

            Assert.Equal("UPDATE \"table1\" SET \"id\" = 100 WHERE \"value\" = 'val0' RETURNING \"id\" AS \"aid\";", text);
        }

        [Fact]
        public void UpdateOnly()
        {
            var statement = Sql.Update("table1").Only()
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Scalar("val0")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("UPDATE ONLY \"table1\" SET \"id\" = 100 WHERE \"value\" = 'val0';", text);
        }

        [Fact]
        public void UpdateWhereCurrentOf()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .WhereCurrentOf(Sql.Name("cursor_name"));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("UPDATE \"table1\" SET \"id\" = 100 WHERE CURRENT OF \"cursor_name\";", text);
        }

        #region Unit tests from FluidSql
        [Fact]
        public void UpdateDefault()
        {
            var statement = Sql.Update("tbl1").Set(Sql.Name("C1"), Sql.Scalar(100)).Where(Sql.Name("C1").IsEqual(Sql.Scalar(1)));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("UPDATE \"tbl1\" SET \"C1\" = 100 WHERE \"C1\" = 1;", command.CommandText);
        }

        [Fact]
        public void UpdateOutput()
        {
            var statement = Sql.Update("tbl1").Set(Sql.Name("C2"), Sql.Scalar("temp2")).Where(Sql.Name("C1").IsEqual(Sql.Scalar(100))).Output(Sql.Name("inserted", "C2"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("UPDATE \"tbl1\" SET \"C2\" = 'temp2' WHERE \"C1\" = 100 RETURNING \"C2\";", command.CommandText);
        }

        [Fact]
        public void UpdateOutputInto()
        {
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b"))).OutputInto("@tempt", Sql.Name("inserted", "a"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("UPDATE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b' RETURNING \"a\" INTO \"tempt\";", command.CommandText);
        }

        [Fact]
        public void UpdateOutputInto2()
        {
            var columns = new List<Name> { "inserted.a" };
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b"))).OutputInto("@tempt", columns);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("UPDATE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b' RETURNING \"a\" INTO \"tempt\";", command.CommandText);
        }

        [Fact]
        public void UpdateOutputInto3()
        {
            var columns = new List<string> { "inserted.a" };
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b"))).OutputInto("@tempt", columns);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("UPDATE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b' RETURNING \"a\" INTO \"tempt\";", command.CommandText);
        }

        [Fact]
        public void UpdateJoinOutputInto()
        {
            var statement = Sql.Update("tbl1")
                .Set(Sql.Name("C1"), Sql.Scalar(44))
                .Set(Sql.Name("C2"), Sql.Scalar(33))
                .Where(Sql.Name("tbl1.C1").IsEqual(Sql.Scalar(2)))
                .OutputInto("@tempt", Sql.Name("inserted", "C1"))
                .InnerJoin(Sql.Name("tbl2"), Sql.Name("tbl1", "C1").IsEqual("tbl2.C1"))
                ;

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("UPDATE \"tbl1\" SET \"C1\" = 44, \"C2\" = 33 FROM \"tbl2\" WHERE \"tbl1\".\"C1\" = 2 AND \"tbl1\".\"C1\" = \"tbl2\".\"C1\" RETURNING \"C1\" INTO \"tempt\";", command.CommandText);
        }
        #endregion
    }
}
