using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;

namespace Tests.PostgreSql
{
    [TestClass]
    public partial class Delete: PostgreSqlProviderTests
    {
        [TestMethod]
        public void DeleteFromTable()
        {
            var statement = Sql.Delete.From(Sql.Name("table1"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\";", text);
        }

        [TestMethod]
        public void DeleteReturningStar()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Output(Sql.Star());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" RETURNING *;", text);
        }

        [TestMethod]
        public void DeleteReturningColumns()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Output(Sql.Name("id"), Sql.Name("value"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" RETURNING \"id\", \"value\";", text);
        }

        [TestMethod]
        public void DeleteReturningAlias()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Output(Sql.Name("id").As("aid"), Sql.Name("value").As("avalue"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" RETURNING \"id\" AS \"aid\", \"value\" AS \"avalue\";", text);
        }

        [TestMethod]
        public void DeleteUsing()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Using(Sql.Name("table2"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" USING \"table2\";", text);
        }

        [TestMethod]
        public void DeleteInnerJoin()
        {
            var statement = Sql.Delete.From(Sql.NameAs("tbl7", "t7"))
               .InnerJoin(Sql.NameAs("tbl8", "t8"), Sql.Name("t7", "id").IsEqual(Sql.Name("t8", "id")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"tbl7\" AS \"t7\" USING \"tbl8\" AS \"t8\" WHERE \"t7\".\"id\" = \"t8\".\"id\";", text);
        }

        [TestMethod]
        public void DeleteUsingAlias()
        {
            var statement = Sql.Delete.From(Sql.Name("table1").As("tbl1")).Using(Sql.Name("table2").As("tbl2"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" AS \"tbl1\" USING \"table2\" AS \"tbl2\";", text);
        }

        [TestMethod]
        public void DeleteUsingTables()
        {
            var statement = Sql.Delete.From(Sql.Name("table1").As("tbl1")).Using(Sql.Name("table2"), Sql.Name("table3"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" AS \"tbl1\" USING \"table2\", \"table3\";", text);
        }

        [TestMethod]
        public void DeleteUsingAliases()
        {
            var statement = Sql.Delete.From(Sql.Name("table1").As("tbl1")).Using(Sql.Name("table2").As("tbl2"), Sql.Name("table3").As("tbl3"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" AS \"tbl1\" USING \"table2\" AS \"tbl2\", \"table3\" AS \"tbl3\";", text);
        }

        [TestMethod]
        public void DeleteOnly()
        {
            var statement = Sql.Delete.Only().From(Sql.Name("table1")).Where(Sql.Name("id").IsEqual(Sql.Scalar(100)));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM ONLY \"table1\" WHERE \"id\" = 100;", text);
        }

        [TestMethod]
        public void DeleteWhereSimple()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Where(Sql.Name("id").IsEqual(Sql.Scalar(100)));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" WHERE \"id\" = 100;", text);
        }

        [TestMethod]
        public void DeleteWhereCurrentOf()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).WhereCurrentOf(Sql.Name("cursor_name"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" WHERE CURRENT OF \"cursor_name\";", text);
        }

        [TestMethod]
        public void DeleteWhereConditions()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Where(Sql.Name("id").IsEqual(Sql.Scalar(100)).Or(Sql.Name("value").IsEqual(Sql.Scalar("val1"))));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" WHERE \"id\" = 100 OR \"value\" = 'val1';", text);
        }

        [TestMethod]
        public void DeleteWhereSelect()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Where(Sql.Name("id").In(Sql.Select.Output(Sql.Name("id")).From("table1")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" WHERE \"id\" IN ( ( SELECT \"id\" FROM \"table1\" ) );", text);
        }

        #region Unit tests from FluidSql
        [TestMethod]
        public void Delete1()
        {
            var statement = Sql.Delete.Top(1).From(Sql.Name("public.bar"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DELETE FROM \"public\".\"bar\" WHERE \"ctid\" = ANY ( ARRAY ( SELECT \"ctid\" FROM \"public\".\"bar\" LIMIT 1 ) );", command.CommandText);
        }

        [TestMethod]
        public void Delete1P()
        {
            var statement = Sql.Delete.Top(20, true).From(Sql.NameAs("tbl7", "f"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DELETE FROM \"tbl7\" AS \"f\" WHERE \"f\".\"ctid\" = ANY ( ARRAY ( SELECT \"ctid\" FROM \"tbl7\" AS \"f\" LIMIT ( SELECT ( SELECT COUNT( * ) FROM \"tbl7\" AS \"f\" ) * 20 / 100 ) ) );", command.CommandText);
        }

        [TestMethod]
        public void DeleteOutput()
        {
            var statement = Sql.Delete.From(Sql.Name("foo.bar")).Output(Sql.Name("DELETED.*"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DELETE FROM \"foo\".\"bar\" RETURNING *;", command.CommandText);
        }

        [TestMethod]
        public void DeleteOutput2()
        {
            var statement = Sql.Delete.From(Sql.NameAs("foo.bar", "s")).Output(Sql.Name("*"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DELETE FROM \"foo\".\"bar\" AS \"s\" RETURNING *;", command.CommandText);
        }

        [TestMethod]
        public void DeleteWhere()
        {
            var statement = Sql.Delete.From(Sql.Name("foo.bar")).Where(Sql.Name("f").IsEqual(Sql.Name("b")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DELETE FROM \"foo\".\"bar\" WHERE \"f\" = \"b\";", command.CommandText);
        }


        [TestMethod]
        public void DeleteJoin()
        {
            var sourceTable = Sql.NameAs("tbl7", "t7");

            var statement = Sql.Delete.From(sourceTable)
                .InnerJoin(Sql.NameAs("tbl8", "t8"), Sql.Name("t7", "c1").IsEqual(Sql.Name("t8", "c1")))
                .Top(5)
                .Output(Sql.Name("deleted", "*"))
                .Where(Sql.Name("t7", "c1").NotEqual(Sql.Scalar(10)));


            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DELETE FROM \"tbl7\" AS \"t7\" USING \"tbl8\" AS \"t8\" WHERE \"t7\".\"c1\" <> 10 AND \"t7\".\"c1\" = \"t8\".\"c1\" AND \"t7\".\"ctid\" = ANY ( ARRAY ( SELECT \"ctid\" FROM \"tbl7\" AS \"t7\" LIMIT 5 ) ) RETURNING *;", command.CommandText);
        }
        #endregion
    }
}
