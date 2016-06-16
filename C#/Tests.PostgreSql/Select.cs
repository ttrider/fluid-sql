using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Common;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;

namespace Tests.PostgreSql
{
    [TestClass]
    public partial class Select : PostgreSqlProviderTests
    {
        [TestMethod]
        public void SelectStarFromTable()
        {
            var statement = Sql.Select.From("table1");

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM \"table1\";", text);
        }

        [TestMethod]
        public void SelectColumnsFromTable()
        {
            var statement = Sql.Select.Output(Sql.Name("id"), Sql.Name("value")).From("table1");

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\", \"value\" FROM \"table1\";", text);
        }

        [TestMethod]
        public void SelectWhereStarFromTable()
        {
            var statement = Sql.Select.From("table1").Where(Sql.Name("id").IsEqual(Sql.Scalar(100))); ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM \"table1\" WHERE \"id\" = 100;", text);
        }

        [TestMethod]
        public void SelectOrderBy()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").OrderBy(Sql.Name("id"), Direction.Asc);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\" FROM \"table1\" ORDER BY \"id\" ASC;", text);
        }

        [TestMethod]
        public void SelectLimit()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").Limit(1);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\" FROM \"table1\" LIMIT 1;", text);
        }

        [TestMethod]
        public void SelectLimitP()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("tbl7").Limit(20, true);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\" FROM \"tbl7\" LIMIT ( SELECT ( SELECT COUNT( * ) FROM \"tbl7\" ) * 20 / 100 );", text);
        }

        [TestMethod]
        public void SelectGroupBy()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").GroupBy(Sql.Name("id")).OrderBy(Sql.Name("id"), Direction.Asc);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\" FROM \"table1\" GROUP BY \"id\" ORDER BY \"id\" ASC;", text);
        }

        [TestMethod]
        public void SelectGroupByHaving()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").GroupBy(Sql.Name("id"), Sql.Name("value")).Having(Sql.Name("value").IsEqual(Sql.Scalar("val8")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\" FROM \"table1\" GROUP BY \"id\", \"value\" HAVING \"value\" = 'val8';", text);
        }

        [TestMethod]
        public void SelectInnerJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .InnerJoin(Sql.Name("table2").As("tbl2"), Sql.Name("tbl1.id").IsEqual(Sql.Name("tbl2.id"))),
            "SELECT * FROM \"table1\" AS \"tbl1\" INNER JOIN \"table2\" AS \"tbl2\" ON \"tbl1\".\"id\" = \"tbl2\".\"id\";");
        }

        [TestMethod]
        public void SelectLeftOuterJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .LeftOuterJoin(Sql.Name("table2").As("tbl2"), Sql.Name("tbl1.id").IsEqual(Sql.Name("tbl2.id"))),
            "SELECT * FROM \"table1\" AS \"tbl1\" LEFT OUTER JOIN \"table2\" AS \"tbl2\" ON \"tbl1\".\"id\" = \"tbl2\".\"id\";");
        }

        [TestMethod]
        public void SelectRightOuterJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .RightOuterJoin(Sql.Name("table2").As("tbl2"), Sql.Name("tbl1.id").IsEqual(Sql.Name("tbl2.id"))),
            "SELECT * FROM \"table1\" AS \"tbl1\" RIGHT OUTER JOIN \"table2\" AS \"tbl2\" ON \"tbl1\".\"id\" = \"tbl2\".\"id\";");
        }

        [TestMethod]
        public void SelectFullJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .FullOuterJoin(Sql.Name("table2").As("tbl2"), Sql.Name("tbl1.id").IsEqual(Sql.Name("tbl2.id"))),
            "SELECT * FROM \"table1\" AS \"tbl1\" FULL OUTER JOIN \"table2\" AS \"tbl2\" ON \"tbl1\".\"id\" = \"tbl2\".\"id\";");
        }

        [TestMethod]
        public void SelectCrossJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .CrossJoin(Sql.Name("table2").As("tbl2")),
            "SELECT * FROM \"table1\" AS \"tbl1\" CROSS JOIN \"table2\" AS \"tbl2\";");
        }

        [TestMethod]
        public void SelectUnionSelect()
        {
            AssertSql(
                Sql.Select.Output(Sql.Name("id")).From("table1").Union(Sql.Select.Output(Sql.Name("id")).From("table2")),
            "SELECT \"id\" FROM \"table1\" UNION SELECT \"id\" FROM \"table2\";");
        }

        [TestMethod]
        public void SelectCountStarFromTable()
        {
            var statement = Sql.Select.Output(Sql.Function("COUNT", Sql.Star())).From("tbl1");

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT COUNT( * ) FROM \"tbl1\";", command.CommandText);
        }
    }
}
