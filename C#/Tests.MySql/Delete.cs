using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for Delete
    /// </summary>
    [TestClass]
    public class Delete : MySqlProviderTests
    {
        [TestMethod]
        public void DeleteFromTable()
        {
            var statement = Sql.Delete.From(Sql.Name("test_delete_tbl"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM `test_delete_tbl`;", text);
        }

        /*[TestMethod]
        public void DeleteReturningStar()
        {
            var statement = Sql.Delete.From(Sql.Name("table1")).Output(Sql.Star());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM \"table1\" RETURNING *;", text);
        }

        /*[TestMethod]
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
        }*/

       
        [TestMethod]
        public void DeleteInnerJoin()
        {
            var statement = Sql.Delete.From(Sql.NameAs("t1", "tbl1"))
               .InnerJoin(Sql.NameAs("t2", "tbl2"), Sql.Name("tbl1", "id").IsEqual(Sql.Name("tbl2", "id")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE `tbl1` FROM `t1` AS `tbl1` INNER JOIN `t2` AS `tbl2` ON `tbl1`.`id` = `tbl2`.`id`;", text);
        }

        [TestMethod]
        public void DeleteFromAlias()
        {
            var statement = Sql.Delete.From(Sql.Name("t1").As("tbl1"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE `tbl1` FROM `t1` AS `tbl1`;", text);
        }

        [TestMethod]
        public void DeleteUsing()
        {
            var statement = Sql.Delete.From(Sql.Name("t1")).Using(Sql.Name("t2"), Sql.Name("t3"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM `t1` USING `t1`, `t2`, `t3`;", text);
        }

        [TestMethod]
        public void DeleteUsingAliases()
        {
            var statement = Sql.Delete.From(Sql.Name("t1").As("tbl1")).Using(Sql.Name("t2").As("tbl2"), Sql.Name("t3").As("tbl3"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM `tbl1` USING `t1` AS `tbl1`, `t2` AS `tbl2`, `t3` AS `tbl3`;", text);
        }

        [TestMethod]
        public void DeleteWhereSimple()
        {
            var statement = Sql.Delete.From(Sql.Name("t1")).Where(Sql.Name("id").IsEqual(Sql.Scalar(1)));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM `t1` WHERE `id` = 1;", text);
        }

        [TestMethod]
        public void DeleteWhereConditions()
        {
            var statement = Sql.Delete.From(Sql.Name("t2")).Where(Sql.Name("id").IsEqual(Sql.Scalar(1)).Or(Sql.Name("name").IsEqual(Sql.Scalar("name2"))));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM `t2` WHERE `id` = 1 OR `name` = N'name2';", text);
        }

        [TestMethod]
        public void DeleteWhereSelect()
        {
            var statement = Sql.Delete.From(Sql.Name("t1")).Where(Sql.Name("id").In(Sql.Select.Output(Sql.Name("id")).From("t3")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("DELETE FROM `t1` WHERE `id` IN ( ( SELECT `id` FROM `t3` ) );", text);
        }

        #region Unit tests from FluidSql
        [TestMethod]
        public void Delete1()
        {
            var statement = Sql.Delete.Top(1).From(Sql.Name("test_db.t3"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DELETE FROM `test_db`.`t3` LIMIT 1;", command.CommandText);
        }

        /*[TestMethod]
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
        }*/

        [TestMethod]
        public void DeleteWhere()
        {
            var statement = Sql.Delete.From(Sql.Name("test_db.t3")).Where(Sql.Name("value").IsEqual(Sql.Scalar("value8")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DELETE FROM `test_db`.`t3` WHERE `value` = N'value8';", command.CommandText);
        }


        [TestMethod]
        public void DeleteJoin()
        {
            var sourceTable = Sql.NameAs("t1", "tbl1");

            var statement = Sql.Delete.From(sourceTable)
                .InnerJoin(Sql.NameAs("t2", "tbl2"), Sql.Name("tbl1", "id").IsEqual(Sql.Name("tbl2", "id")))
                .Where(Sql.Name("tbl1", "id").NotEqual(Sql.Scalar(1)));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DELETE `tbl1` FROM `t1` AS `tbl1` INNER JOIN `t2` AS `tbl2` ON `tbl1`.`id` = `tbl2`.`id` WHERE `tbl1`.`id` <> 1;", command.CommandText);
        }
        #endregion
    }
}
