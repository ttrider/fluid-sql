using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TTRider.FluidSql;

namespace Tests.MySqlTests
{
    [TestClass]
    public class Update : MySqlProviderTests
    {
        [TestMethod]
        public void UpdateWhere()
        {
            var statement = Sql.Update("t1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Scalar("value1")))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);

            Assert.AreEqual("UPDATE `t1` SET `id` = 100 WHERE `value` = N'value1';", text);
        }

        [TestMethod]
        public void UpdateAlias()
        {
            var statement = Sql.Update(Sql.Name("t2").As("tbl1"))
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Scalar("value10")))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);

            Assert.AreEqual("UPDATE `t2` AS `tbl1` SET `id` = 100 WHERE `value` = N'value10';", text);
        }

        /*[TestMethod]
        public void UpdateReturning()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Name("val0"))).Output(Sql.Name("id"), Sql.Name("value"))
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);

            Assert.AreEqual("UPDATE \"table1\" SET \"id\" = 100 WHERE \"value\" = \"val0\" RETURNING \"id\", \"value\";", text);
        }

        [TestMethod]
        public void UpdateReturningStar()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Name("val0"))).Output(Sql.Star())
                ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);

            Assert.AreEqual("UPDATE \"table1\" SET \"id\" = 100 WHERE \"value\" = \"val0\" RETURNING *;", text);
        }

        [TestMethod]
        public void UpdateReturningAlias()
        {
            var statement = Sql.Update("table1")
                .Set(Sql.Name("id"), Sql.Scalar(100))
                .Where(Sql.Name("value").IsEqual(Sql.Scalar("val0"))).Output(Sql.Name("id").As("aid"));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);

            Assert.AreEqual("UPDATE \"table1\" SET \"id\" = 100 WHERE \"value\" = 'val0' RETURNING \"id\" AS \"aid\";", text);
        }*/        

        #region Unit tests from FluidSql
        [TestMethod]
        public void UpdateDefault()
        {
            var statement = Sql.Update("t1").Set(Sql.Name("id"), Sql.Scalar(100)).Where(Sql.Name("id").IsEqual(Sql.Scalar(1)));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE `t1` SET `id` = 100 WHERE `id` = 1;", command.CommandText);
        }

        /*[TestMethod]
        public void UpdateOutput()
        {
            var statement = Sql.Update("tbl1").Set(Sql.Name("C2"), Sql.Scalar("temp2")).Where(Sql.Name("C1").IsEqual(Sql.Scalar(100))).Output(Sql.Name("inserted", "C2"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE \"tbl1\" SET \"C2\" = 'temp2' WHERE \"C1\" = 100 RETURNING \"C2\";", command.CommandText);
        }

        [TestMethod]
        public void UpdateOutputInto()
        {
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b"))).OutputInto("@tempt", Sql.Name("inserted", "a"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b' RETURNING \"a\" INTO \"tempt\";", command.CommandText);
        }

        [TestMethod]
        public void UpdateOutputInto2()
        {
            var columns = new List<Name> { "inserted.a" };
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b"))).OutputInto("@tempt", columns);

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b' RETURNING \"a\" INTO \"tempt\";", command.CommandText);
        }

        [TestMethod]
        public void UpdateOutputInto3()
        {
            var columns = new List<string> { "inserted.a" };
            var statement = Sql.Update("foo.bar").Set(Sql.Name("a"), Sql.Scalar(1)).Where(Sql.Name("z").IsEqual(Sql.Scalar("b"))).OutputInto("@tempt", columns);

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE \"foo\".\"bar\" SET \"a\" = 1 WHERE \"z\" = 'b' RETURNING \"a\" INTO \"tempt\";", command.CommandText);
        }*/

        [TestMethod]
        public void UpdateJoinOutputInto()
        {
            var statement = Sql.Update("t1")
                .Set(Sql.Name("t1.id"), Sql.Scalar(44))
                .Set(Sql.Name("t1.name"), Sql.Scalar("test44"))
                .Where(Sql.Name("t1.id").IsEqual(Sql.Scalar(2)))
                .InnerJoin(Sql.Name("t2"), Sql.Name("t1", "id").IsEqual("t2.id"))
                ;

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("UPDATE `t1` INNER JOIN `t2` ON `t1`.`id` = `t2`.`id` SET `t1`.`id` = 44, `t1`.`name` = N'test44' WHERE `t1`.`id` = 2;", command.CommandText);
        }
        #endregion
    }
}
