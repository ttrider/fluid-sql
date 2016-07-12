using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TTRider.FluidSql;

namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for Select
    /// </summary>
    [TestClass]
    public class Select : MySqlProviderTests
    {
        [TestMethod]
        public void SelectStarFromTable()
        {
            var statement = Sql.Select.From("test_select_tbl");

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM `test_select_tbl`;", text);
        }

        [TestMethod]
        public void SelectColumnsFromTable()
        {
            var statement = Sql.Select.Output(Sql.Name("number_value"), Sql.Name("text_value")).From("test_select_tbl");

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT `number_value`, `text_value` FROM `test_select_tbl`;", text);
        }

        [TestMethod]
        public void SelectWhereStarFromTable()
        {
            var statement = Sql.Select.From("test_select_tbl").Where(Sql.Name("number_value").IsEqual(Sql.Scalar(1))); ;

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM `test_select_tbl` WHERE `number_value` = 1;", text);
        }

        [TestMethod]
        public void SelectOrderBy()
        {
            var statement = Sql.Select.Output(Sql.Name("number_value")).From("test_select_tbl").OrderBy(Sql.Name("number_value"), Direction.Asc);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT `number_value` FROM `test_select_tbl` ORDER BY `number_value` ASC;", text);
        }

        [TestMethod]
        public void SelectLimit()
        {
            var statement = Sql.Select.Output(Sql.Name("number_value")).From("test_select_tbl").Limit(2);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT `number_value` FROM `test_select_tbl` LIMIT 2;", text);
        }

        /*[TestMethod]
        public void SelectLimitP()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("tbl7").Limit(20, true);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT \"id\" FROM \"tbl7\" LIMIT ( SELECT ( SELECT COUNT( * ) FROM \"tbl7\" ) * 20 / 100 );", text);
        }*/

        [TestMethod]
        public void SelectGroupBy()
        {
            var statement = Sql.Select.Output(Sql.Name("number_value")).From("test_select_tbl").GroupBy(Sql.Name("number_value")).OrderBy(Sql.Name("number_value"), Direction.Desc);

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT `number_value` FROM `test_select_tbl` GROUP BY `number_value` ORDER BY `number_value` DESC;", text);
        }

        [TestMethod]
        public void SelectGroupByHaving()
        {
            var statement = Sql.Select.Output(Sql.Name("number_value")).From("test_select_tbl").GroupBy(Sql.Name("number_value"), Sql.Name("text_value")).Having(Sql.Name("text_value").IsEqual(Sql.Scalar("test1")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT `number_value` FROM `test_select_tbl` GROUP BY `number_value`, `text_value` HAVING `text_value` = N'test1';", text);
        }

        [TestMethod]
        public void SelectInnerJoin()
        {
            var statement = Sql.Select
                .Output(Sql.Star())
                .From("test_select_tbl", "tbl1")
                .InnerJoin(Sql.Name("test_select2_tbl").As("tbl2"), Sql.Name("tbl1.number_value").IsEqual(Sql.Name("tbl2.number_value")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM `test_select_tbl` AS `tbl1` INNER JOIN `test_select2_tbl` AS `tbl2` ON `tbl1`.`number_value` = `tbl2`.`number_value`;", text);
        }

        [TestMethod]
        public void SelectLeftOuterJoin()
        {
            var statement = Sql.Select
                .Output(Sql.Star())
                .From("test_select_tbl", "tbl1")
                .LeftOuterJoin(Sql.Name("test_select2_tbl").As("tbl2"), Sql.Name("tbl1.number_value").IsEqual(Sql.Name("tbl2.number_value")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM `test_select_tbl` AS `tbl1` LEFT OUTER JOIN `test_select2_tbl` AS `tbl2` ON `tbl1`.`number_value` = `tbl2`.`number_value`;", text);
        }

        [TestMethod]
        public void SelectRightOuterJoin()
        {
            var statement = Sql.Select
               .Output(Sql.Star())
               .From("test_select_tbl", "tbl1")
               .RightOuterJoin(Sql.Name("test_select2_tbl").As("tbl2"), Sql.Name("tbl1.number_value").IsEqual(Sql.Name("tbl2.number_value")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM `test_select_tbl` AS `tbl1` RIGHT OUTER JOIN `test_select2_tbl` AS `tbl2` ON `tbl1`.`number_value` = `tbl2`.`number_value`;", text);
        }

        /*[TestMethod]
        public void SelectFullJoin()
        {
            var statement = Sql.Select
               .Output(Sql.Star())
               .From("test_select_tbl", "tbl1")
               .FullOuterJoin(Sql.Name("test_select2_tbl").As("tbl2"), Sql.Name("tbl1.number_value").IsEqual(Sql.Name("tbl2.number_value")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM `test_select_tbl` AS `tbl1` FULL OUTER JOIN `test_select2_tbl` AS `tbl2` ON `tbl1`.`number_value` = `tbl2`.`number_value`;", text);
        }*/

        [TestMethod]
        public void SelectCrossJoin()
        {
            var statement = Sql.Select
                .Output(Sql.Star())
                .From("test_select_tbl", "tbl1")
                .CrossJoin(Sql.Name("test_select2_tbl").As("tbl2"));
            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM `test_select_tbl` AS `tbl1` CROSS JOIN `test_select2_tbl` AS `tbl2`;", text);
        }

        [TestMethod]
        public void SelectCountStarFromTable()
        {
            var statement = Sql.Select.Output(Sql.Function("COUNT", Sql.Star())).From("test_select_tbl");

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT COUNT( * ) FROM `test_select_tbl`;", command.CommandText);
        }

         #region Unit tests from FluidSql
         [TestMethod]
         public void Select1()
         {
             AssertSql(
                 Sql.Select.Output(Sql.Scalar(1)),
                 "SELECT 1;"
                 );
         }

         [TestMethod]
         public void SelectLiteralString()
         {
             AssertSql(
                 Sql.Select.Output(Sql.Scalar("this is the string with ' inside")),
                 "SELECT N'this is the string with '' inside';"
                 );
         }

         [TestMethod]
         public void SelectStarFromPublicTable()
         {
             var statement = Sql.Select.From("test_db.test_select_tbl");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM `test_db`.`test_select_tbl`;", command.CommandText);
         }

         [TestMethod]
         public void SelectFromSelectStarFromPublicTable()
         {
             var subselect = Sql.Select.From("test_db.test_select_tbl");

             var statement = Sql.Select.From(subselect.As("foo"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM ( SELECT * FROM `test_db`.`test_select_tbl` ) AS `foo`;", command.CommandText);
         }

         [TestMethod]
         public void SelectStarFromPublicTableAsO()
         {
             var statement = Sql.Select.From("test_db.test_select_tbl", "O");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM `test_db`.`test_select_tbl` AS `O`;", command.CommandText);
         }

         [TestMethod]
         public void SelectStarFromPublicTableAsO2()
         {
             var statement = Sql.Select.From(Sql.Name("test_db.test_select_tbl").As("O"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM `test_db`.`test_select_tbl` AS `O`;", command.CommandText);
         }

        //TODO: Schema and DB for MySql are synonims

         /*[TestMethod]
         public void SelectSkipSchema()
         {
             var statement = Sql.Select.From(Sql.Name("DB", null, "objects"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM `DB`..`objects`;", command.CommandText);
         }

         [TestMethod]
         public void SelectSkipDbAndSchema()
         {
             var statement = Sql.Select.From(Sql.Name("server", null, null, "objects"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM `server`...`objects`;", command.CommandText);
         }

         [TestMethod]
         public void SelectStarFromPublicTableInto()
         {
             var statement = Sql.Select.From("test_select_tbl").Into(Sql.Name("test_select2_tbl"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * INTO \"tbl12\" FROM \"tbl1\";", command.CommandText);
         }*/

         [TestMethod]
         public void SelectCountStarFromPublicTable()
         {
             var statement = Sql.Select.Output(Sql.Function("COUNT", Sql.Star())).From("test_db.test_select_tbl");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT COUNT( * ) FROM `test_db`.`test_select_tbl`;", command.CommandText);
         }

         [TestMethod]
         public void SelectFoo()
         {
             var statement = Sql.Select.Output(Sql.Scalar("Foo1"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT N'Foo1';", command.CommandText);
         }

         [TestMethod]
         public void SelectFunction()
         {
             var statement = Sql.Select.Output(Sql.Function("NOW").As("Foo"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT NOW( ) AS `Foo`;", command.CommandText);
         }

         [TestMethod]
         public void Select1Top1()
         {
             var statement = Sql.Select.Output(Sql.Scalar(1).As("foo")).Top(1);

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT 1 AS `foo` LIMIT 1;", command.CommandText);
         }

         [TestMethod]
         public void Select1OffsetFetch()
         {
             var statement = Sql.Select.From(Sql.Name("test_db.test_select_tbl").As("foo")).OrderBy("number_value").Offset(2).FetchNext(10);

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM `test_db`.`test_select_tbl` AS `foo` ORDER BY `number_value` ASC LIMIT 10 OFFSET 2;", command.CommandText);
         }

        /*[TestMethod]
         public void Select1Top1Percent()
         {
             var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, true).From("tbl1");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT 1 FROM \"tbl1\" LIMIT ( SELECT ( SELECT COUNT( * ) FROM \"tbl1\" ) * 1 / 100 );", command.CommandText);
         }

         [TestMethod]
         public void SelectCTE()
         {
             var statement = Sql.With("tbl1").As(Sql.Select.From("test_db.test_select_tbl")).Select().From("tbl1");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("WITH \"tbl12\" AS ( SELECT * FROM \"public\".\"tbl1\" ) SELECT * FROM \"tbl12\";", command.CommandText);
         }

         [TestMethod]
         public void SelectCTE2()
         {
             var statement = Sql.With("tbl12", "C1", "C2").As(Sql.Select.Output("C1", "C2").From("public.tbl1")).Select().From("tbl12");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("WITH \"tbl12\" ( \"C1\", \"C2\" ) AS ( SELECT \"C1\", \"C2\" FROM \"public\".\"tbl1\" ) SELECT * FROM \"tbl12\";", command.CommandText);
         }

         [TestMethod]
         public void SelectCTE3()
         {
             var columns = new List<string> { "C1", "C2" };

             var statement = Sql.With("tbl12", columns).As(Sql.Select.Output(columns).From("public.tbl1")).Select().From("tbl12");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("WITH \"tbl12\" ( \"C1\", \"C2\" ) AS ( SELECT \"C1\", \"C2\" FROM \"public\".\"tbl1\" ) SELECT * FROM \"tbl12\";", command.CommandText);
         }*/

         [TestMethod]
         public void SelectStar()
         {
             var statement = Sql.Select.Output(Sql.Star());

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT *;", command.CommandText);
         }

         [TestMethod]
         public void SelectStarPlus()
         {
             var statement = Sql.Select.Output(Sql.Star()).Distinct();

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT DISTINCT *;", command.CommandText);
         }

         [TestMethod]
         public void SelectStarPlusPlus()
         {
             var statement = Sql.Select.Output(Sql.Star("tl1")).From("test_db.test_select_tbl", "tl1");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `tl1`.* FROM `test_db`.`test_select_tbl` AS `tl1`;", command.CommandText);
         }

         [TestMethod]
         public void SelectGroupBy1()
         {
             var statement = Sql.Select.Output(Sql.Name("at1", "number_value")).From("test_db.test_select_tbl", "at1").GroupBy(Sql.Name("at1", "number_value"), Sql.Name("at1", "text_value"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `at1`.`number_value` FROM `test_db`.`test_select_tbl` AS `at1` GROUP BY `at1`.`number_value`, `at1`.`text_value`;", command.CommandText);
         }

         [TestMethod]
         public void SelectGroupBy2()
         {
             var statement = Sql.Select.Output(Sql.Name("at1", "number_value")).From("test_db.test_select_tbl", "at1").GroupBy("at1.number_value", "at1.text_value");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `at1`.`number_value` FROM `test_db`.`test_select_tbl` AS `at1` GROUP BY `at1`.`number_value`, `at1`.`text_value`;", command.CommandText);
         }

         [TestMethod]
         public void SelectGroupBy3()
         {
             var columns = new List<string> { "at1.number_value", "at1.text_value" };
             var statement = Sql.Select.Output(Sql.Name("at1", "number_value")).From("test_db.test_select_tbl", "at1").GroupBy(columns);

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `at1`.`number_value` FROM `test_db`.`test_select_tbl` AS `at1` GROUP BY `at1`.`number_value`, `at1`.`text_value`;", command.CommandText);
         }

         [TestMethod]
         public void SelectGroupBy4()
         {
             var columns = new List<Name> { "at1.number_value", "at1.text_value" };
             var statement = Sql.Select.Output(Sql.Name("at1", "number_value")).From("test_db.test_select_tbl", "at1").GroupBy(columns);

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `at1`.`number_value` FROM `test_db`.`test_select_tbl` AS `at1` GROUP BY `at1`.`number_value`, `at1`.`text_value`;", command.CommandText);
         }

         [TestMethod]
         public void SelectGroupByHaving1()
         {
             var statement = Sql.Select.Output(Sql.Name("src", "number_value")).From("test_db.test_select_tbl", "src").GroupBy(Sql.Name("src", "number_value"), Sql.Name("src", "text_value")).Having(Sql.Name("src", "number_value").IsNull());

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `src`.`number_value` FROM `test_db`.`test_select_tbl` AS `src` GROUP BY `src`.`number_value`, `src`.`text_value` HAVING `src`.`number_value` IS NULL;", command.CommandText);
         }

         [TestMethod]
         public void SelectOrderBy1()
         {
             var statement = Sql.Select.Output(Sql.Name("src.number_value")).From("test_db.test_select_tbl", "src").OrderBy(Sql.Name("src", "number_value"), Direction.Asc).OrderBy(Sql.Name("src", "text_value"), Direction.Desc);

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `src`.`number_value` FROM `test_db`.`test_select_tbl` AS `src` ORDER BY `src`.`number_value` ASC, `src`.`text_value` DESC;", command.CommandText);
         }

         [TestMethod]
         public void SelectOrderBy2()
         {

             var statement = Sql.Select.Output(Sql.Name("src.number_value")).From("test_db.test_select_tbl", "src").OrderBy(Sql.Name("src", "number_value"), Direction.Asc).OrderBy(Sql.Order("src.text_value", Direction.Desc));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `src`.`number_value` FROM `test_db`.`test_select_tbl` AS `src` ORDER BY `src`.`number_value` ASC, `src`.`text_value` DESC;", command.CommandText);
         }

         [TestMethod]
         public void SelectOrderBy3()
         {
             var orders = new List<Order> { Sql.Order("src.number_value"), Sql.Order("src.text_value", Direction.Desc) };

             var statement = Sql.Select.Output(Sql.Name("src.number_value")).From("test_db.test_select_tbl", "src").OrderBy(orders);

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `src`.`number_value` FROM `test_db`.`test_select_tbl` AS `src` ORDER BY `src`.`number_value` ASC, `src`.`text_value` DESC;", command.CommandText);
         }

         [TestMethod]
         public void SelectUnionSelect()
         {
             AssertSql(
                 Sql.Select.Output(Sql.Name("number_value")).From("test_select_tbl").Union(Sql.Select.Output(Sql.Name("number_value")).From("test_select2_tbl")),
             "SELECT `number_value` FROM `test_select_tbl` UNION SELECT `number_value` FROM `test_select2_tbl`;");
         }

         [TestMethod]
         public void SelectUnionSelectWrapped()
         {
             var statement = Sql.Select.Output(Sql.Star()).From("test_db.test_select_tbl").Union(Sql.Select.Output(Sql.Star()).From("test_db.test_select_tbl")).WrapAsSelect("wrap");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `wrap`.* FROM ( SELECT * FROM `test_db`.`test_select_tbl` UNION SELECT * FROM `test_db`.`test_select_tbl` ) AS `wrap`;", command.CommandText);
         }

         [TestMethod]
         public void SelectWrapped2()
         {
             var statement = Sql.Select.Output(Sql.Name("number_value")).From("test_db.test_select_tbl").WrapAsSelect("wrap");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `wrap`.`number_value` FROM ( SELECT `number_value` FROM `test_db`.`test_select_tbl` ) AS `wrap`;", command.CommandText);
         }

         [TestMethod]
         public void SelectWrapped3()
         {
             var statement = Sql.Select.Output(Sql.Name("number_value").As("nm")).From("test_db.test_select_tbl").WrapAsSelect("wrap");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT `wrap`.`nm` FROM ( SELECT `number_value` AS `nm` FROM `test_db`.`test_select_tbl` ) AS `wrap`;", command.CommandText);
         }

         [TestMethod]
         public void SelectUnionAllSelect()
         {
             var statement = Sql.Select.Output(Sql.Star()).From("test_db.test_select_tbl").UnionAll(Sql.Select.Output(Sql.Star()).From("test_db.test_select2_tbl"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM `test_db`.`test_select_tbl` UNION ALL SELECT * FROM `test_db`.`test_select2_tbl`;", command.CommandText);
         }

         [TestMethod]
         public void SelectUnionDistinctSelect()
         {
             var statement = Sql.Select.Output(Sql.Star()).From("test_db.test_select_tbl").UnionDistinct(Sql.Select.Output(Sql.Star()).From("test_db.test_select2_tbl"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM `test_db`.`test_select_tbl` UNION SELECT * FROM `test_db`.`test_select2_tbl`;", command.CommandText);
         }

         /*[TestMethod]
         public void SelectExceptSelect()
         {
             var statement = Sql.Select.Output(Sql.Star()).From("test_db.test_select_tbl").Except(Sql.Select.Output(Sql.Star()).From("test_db.test_select2_tbl"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM \"public\".\"tbl1\" EXCEPT SELECT * FROM \"public\".\"tbl2\";", command.CommandText);
         }

         [TestMethod]
         public void SelectExceptAllSelect()
         {
             var statement = Sql.Select.Output(Sql.Star()).From("test_db.test_select_tbl").ExceptAll(Sql.Select.Output(Sql.Star()).From("test_db.test_select2_tbl"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM \"public\".\"tbl1\" EXCEPT ALL SELECT * FROM \"public\".\"tbl2\";", command.CommandText);
         }

         [TestMethod]
         public void SelectExceptDistinctSelect()
         {
             var statement = Sql.Select.Output(Sql.Star()).From("test_db.test_select_tbl").ExceptDistinct(Sql.Select.Output(Sql.Star()).From("test_db.test_select2_tbl"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM \"public\".\"tbl1\" EXCEPT SELECT * FROM \"public\".\"tbl2\";", command.CommandText);
         }*/

         /*[TestMethod]
         public void SelectIntersectSelect()
         {
             var statement = Sql.Select.Output(Sql.Star("First")).From("test_db.test_select_tbl", "First").Intersect(Sql.Select.Output(Sql.Star("Second")).From("test_db.test_select2_tbl", "Second"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT \"First\".* FROM \"public\".\"tbl1\" AS \"First\" INTERSECT SELECT \"Second\".* FROM \"public\".\"tbl2\" AS \"Second\";", command.CommandText);
         }

         [TestMethod]
         public void SelectIntersectAllSelect()
         {
             var statement = Sql.Select.Output(Sql.Star("First")).From("public.tbl1", "First").IntersectAll(Sql.Select.Output(Sql.Star("Second")).From("public.tbl2", "Second"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT \"First\".* FROM \"public\".\"tbl1\" AS \"First\" INTERSECT ALL SELECT \"Second\".* FROM \"public\".\"tbl2\" AS \"Second\";", command.CommandText);
         }

         [TestMethod]
         public void SelectIntersectDistinctSelect()
         {
             var statement = Sql.Select.Output(Sql.Star("First")).From("public.tbl1", "First").IntersectDistinct(Sql.Select.Output(Sql.Star("Second")).From("public.tbl2", "Second"));

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT \"First\".* FROM \"public\".\"tbl1\" AS \"First\" INTERSECT SELECT \"Second\".* FROM \"public\".\"tbl2\" AS \"Second\";", command.CommandText);
         }

         [TestMethod]
         public void SelectIntersectWrapped()
         {
             var statement =
                 Sql.Select.Output(Sql.Star("First"))
                     .From("public.tbl1", "First")
                     .Intersect(Sql.Select.Output(Sql.Star("Second")).From("public.tbl2", "Second"))
                     .WrapAsSelect("wrap");

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT \"wrap\".* FROM ( SELECT \"First\".* FROM \"public\".\"tbl1\" AS \"First\" INTERSECT SELECT \"Second\".* FROM \"public\".\"tbl2\" AS \"Second\" ) AS \"wrap\";", command.CommandText);
         }*/


         [TestMethod]
         public void SelectInnerJoin1()
         {
             AssertSql(
                 Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .InnerJoin(Sql.Name("test_db.test_select2_tbl").As("t"), Sql.Name("t.number_value").IsEqual(Sql.Name("o.number_value"))),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` INNER JOIN `test_db`.`test_select2_tbl` AS `t` ON `t`.`number_value` = `o`.`number_value`;");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .InnerJoin(Sql.Name("test_db.test_select2_tbl"), "t", Sql.Name("t.number_value").IsEqual(Sql.Name("o.number_value"))),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` INNER JOIN `test_db`.`test_select2_tbl` AS `t` ON `t`.`number_value` = `o`.`number_value`;");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .InnerJoin("test_db.test_select2_tbl", "t", Sql.Name("t.number_value").IsEqual(Sql.Name("o.number_value"))),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` INNER JOIN `test_db`.`test_select2_tbl` AS `t` ON `t`.`number_value` = `o`.`number_value`;");
         }

         [TestMethod]
         public void SelectLeftOuterJoin1()
         {
             AssertSql(
                 Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .LeftOuterJoin(Sql.Name("test_db.test_select2_tbl").As("t"), Sql.Name("t.number_value").IsEqual(Sql.Name("o.number_value"))),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` LEFT OUTER JOIN `test_db`.`test_select2_tbl` AS `t` ON `t`.`number_value` = `o`.`number_value`;");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .LeftOuterJoin(Sql.Name("test_db.test_select2_tbl"), "t", Sql.Name("t.number_value").IsEqual(Sql.Name("o.number_value"))),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` LEFT OUTER JOIN `test_db`.`test_select2_tbl` AS `t` ON `t`.`number_value` = `o`.`number_value`;");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .LeftOuterJoin("test_db.test_select2_tbl", "t", Sql.Name("t.number_value").IsEqual(Sql.Name("o.number_value"))),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` LEFT OUTER JOIN `test_db`.`test_select2_tbl` AS `t` ON `t`.`number_value` = `o`.`number_value`;");
         }

         [TestMethod]
         public void SelectRightOuterJoin1()
         {
             AssertSql(
                 Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .RightOuterJoin(Sql.Name("test_db.test_select2_tbl").As("t"), Sql.Name("t.number_value").IsEqual(Sql.Name("o.number_value"))),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` RIGHT OUTER JOIN `test_db`.`test_select2_tbl` AS `t` ON `t`.`number_value` = `o`.`number_value`;");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .RightOuterJoin(Sql.Name("test_db.test_select2_tbl"), "t", Sql.Name("t.number_value").IsEqual(Sql.Name("o.number_value"))),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` RIGHT OUTER JOIN `test_db`.`test_select2_tbl` AS `t` ON `t`.`number_value` = `o`.`number_value`;");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .RightOuterJoin("test_db.test_select2_tbl", "t", Sql.Name("t.number_value").IsEqual(Sql.Name("o.number_value"))),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` RIGHT OUTER JOIN `test_db`.`test_select2_tbl` AS `t` ON `t`.`number_value` = `o`.`number_value`;");
         }

         /*[TestMethod]
         public void SelectFullOuterJoin()
         {
             AssertSql(
                 Sql.Select
                 .Output(Sql.Star())
                 .From("public.tbl1", "o")
                 .FullOuterJoin(Sql.Name("public.tbl2").As("t"), Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
             "SELECT * FROM \"public\".\"tbl1\" AS \"o\" FULL OUTER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("public.tbl1", "o")
                 .FullOuterJoin(Sql.Name("public.tbl2"), "t", Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
             "SELECT * FROM \"public\".\"tbl1\" AS \"o\" FULL OUTER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("public.tbl1", "o")
                 .FullOuterJoin("public.tbl2", "t", Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
             "SELECT * FROM \"public\".\"tbl1\" AS \"o\" FULL OUTER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");
         }*/

         [TestMethod]
         public void SelectCrossJoin1()
         {
             AssertSql(
                 Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .CrossJoin(Sql.Name("test_db.test_select2_tbl").As("t")),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` CROSS JOIN `test_db`.`test_select2_tbl` AS `t`;");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .CrossJoin(Sql.Name("test_db.test_select2_tbl"), "t"),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` CROSS JOIN `test_db`.`test_select2_tbl` AS `t`;");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .CrossJoin("test_db.test_select2_tbl", "t"),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` CROSS JOIN `test_db`.`test_select2_tbl` AS `t`;");

             AssertSql(Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl", "o")
                 .CrossJoin("test_db.test_select2_tbl"),
             "SELECT * FROM `test_db`.`test_select_tbl` AS `o` CROSS JOIN `test_db`.`test_select2_tbl`;");
         }

         [TestMethod]
         public void SelectSupeExpression()
         {
             var statement = Sql.Select
                 .Output(Sql.Star())
                 .From("test_db.test_select_tbl")
                 .Where(
                     Sql.Name("number_value")
                         .IsNotNull()
                         .And(
                             Sql.Name("number_value").IsNull()
                         .And(
                             Sql.Group(
                             Sql.Name("number_value")
                             .Plus(Sql.Scalar(1))
                             .Minus(Sql.Scalar(1))
                             .Multiply(Sql.Scalar(1))
                             .Modulo(Sql.Scalar(1))
                         ).Less(Sql.Scalar(1)))));


             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("SELECT * FROM `test_db`.`test_select_tbl` WHERE `number_value` IS NOT NULL AND `number_value` IS NULL AND ( `number_value` + 1 - 1 * 1 % 1 ) < 1;", command.CommandText);
         }
        #endregion

        #region OtherFunction
        [TestMethod]
        public void SetFooIntoBar()
        {
            var statement = Sql.Set(Sql.Name("@Foo"), Sql.Scalar("bar"));
            var command = Provider.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("SET @Foo = N'bar';", command.CommandText);
        }
        #endregion
    }
}
