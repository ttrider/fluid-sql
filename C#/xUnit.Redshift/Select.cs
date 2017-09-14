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
    public class Select : RedshiftSqlProviderTests
    {
        [Fact]
        public void SelectStarFromTable()
        {
            var statement = Sql.Select.From("table1");

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("SELECT * FROM \"table1\";", text);
        }

        [Fact]
        public void SelectColumnsFromTable()
        {
            var statement = Sql.Select.Output(Sql.Name("id"), Sql.Name("value")).From("table1");

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("SELECT \"id\", \"value\" FROM \"table1\";", text);
        }

        [Fact]
        public void SelectWhereStarFromTable()
        {
            var statement = Sql.Select.From("table1").Where(Sql.Name("id").IsEqual(Sql.Scalar(100))); ;

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("SELECT * FROM \"table1\" WHERE \"id\" = 100;", text);
        }

        [Fact]
        public void SelectOrderBy()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").OrderBy(Sql.Name("id"), Direction.Asc);

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("SELECT \"id\" FROM \"table1\";", text);
        }

        [Fact]
        public void SelectLimit()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").Limit(1);

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("SELECT \"id\" FROM \"table1\" LIMIT 1;", text);
        }

        [Fact]
        public void SelectLimitP()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("tbl7").Limit(20, true);

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("SELECT \"id\" FROM \"tbl7\" LIMIT ( SELECT ( SELECT COUNT( * ) FROM \"tbl7\" ) * 20 / 100 );", text);
        }

        [Fact]
        public void SelectGroupBy()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").GroupBy(Sql.Name("id")).OrderBy(Sql.Name("id"), Direction.Asc);

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("SELECT \"id\" FROM \"table1\";", text);
        }

        [Fact]
        public void SelectGroupByHaving()
        {
            var statement = Sql.Select.Output(Sql.Name("id")).From("table1").GroupBy(Sql.Name("id"), Sql.Name("value")).Having(Sql.Name("value").IsEqual(Sql.Scalar("val8")));

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("SELECT \"id\" FROM \"table1\";", text);
        }

        [Fact]
        public void SelectInnerJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .InnerJoin(Sql.Name("table2").As("tbl2"), Sql.Name("tbl1.id").IsEqual(Sql.Name("tbl2.id"))),
            "SELECT * FROM \"table1\" AS \"tbl1\" INNER JOIN \"table2\" AS \"tbl2\" ON \"tbl1\".\"id\" = \"tbl2\".\"id\";");
        }

        [Fact]
        public void SelectLeftOuterJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .LeftOuterJoin(Sql.Name("table2").As("tbl2"), Sql.Name("tbl1.id").IsEqual(Sql.Name("tbl2.id"))),
            "SELECT * FROM \"table1\" AS \"tbl1\" LEFT OUTER JOIN \"table2\" AS \"tbl2\" ON \"tbl1\".\"id\" = \"tbl2\".\"id\";");
        }

        [Fact]
        public void SelectRightOuterJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .RightOuterJoin(Sql.Name("table2").As("tbl2"), Sql.Name("tbl1.id").IsEqual(Sql.Name("tbl2.id"))),
            "SELECT * FROM \"table1\" AS \"tbl1\" RIGHT OUTER JOIN \"table2\" AS \"tbl2\" ON \"tbl1\".\"id\" = \"tbl2\".\"id\";");
        }

        [Fact]
        public void SelectFullJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .FullOuterJoin(Sql.Name("table2").As("tbl2"), Sql.Name("tbl1.id").IsEqual(Sql.Name("tbl2.id"))),
            "SELECT * FROM \"table1\" AS \"tbl1\" FULL OUTER JOIN \"table2\" AS \"tbl2\" ON \"tbl1\".\"id\" = \"tbl2\".\"id\";");
        }

        [Fact]
        public void SelectCrossJoin()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("table1", "tbl1")
                .CrossJoin(Sql.Name("table2").As("tbl2")),
            "SELECT * FROM \"table1\" AS \"tbl1\" CROSS JOIN \"table2\" AS \"tbl2\";");
        }

        [Fact]
        public void SelectCountStarFromTable()
        {
            var statement = Sql.Select.Output(Sql.Function("COUNT", Sql.Star())).From("tbl1");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT COUNT( * ) FROM \"tbl1\";", command.CommandText);
        }

        #region Unit tests from FluidSql
        [Fact]
        public void Select1()
        {
            AssertSql(
                Sql.Select.Output(Sql.Scalar(1)),
                "SELECT 1;"
                );
        }

        [Fact]
        public void SelectLiteralString()
        {
            AssertSql(
                Sql.Select.Output(Sql.Scalar("this is the string with ' inside")),
                "SELECT 'this is the string with '' inside';"
                );
        }

        [Fact]
        public void SelectStarFromPublicTable()
        {
            var statement = Sql.Select.From("public.tbl1");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\";", command.CommandText);
        }

        [Fact]
        public void SelectFromSelectStarFromPublicTable()
        {
            var subselect = Sql.Select.From("public.tbl1");

            var statement = Sql.Select.From(subselect.As("foo"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM ( SELECT * FROM \"public\".\"tbl1\" ) AS \"foo\";", command.CommandText);
        }

        [Fact]
        public void SelectStarFromPublicTableAsO()
        {
            var statement = Sql.Select.From("public.tbl1", "O");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\" AS \"O\";", command.CommandText);
        }

        [Fact]
        public void SelectStarFromPublicTableAsO2()
        {
            var statement = Sql.Select.From(Sql.Name("public.tbl1").As("O"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\" AS \"O\";", command.CommandText);
        }

        [Fact]
        public void SelectSkipSchema()
        {
            var statement = Sql.Select.From(Sql.Name("DB", null, "objects"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"DB\"..\"objects\";", command.CommandText);
        }

        [Fact]
        public void SelectSkipDbAndSchema()
        {
            var statement = Sql.Select.From(Sql.Name("server", null, null, "objects"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"server\"...\"objects\";", command.CommandText);
        }

        [Fact]
        public void SelectStarFromPublicTableInto()
        {
            var statement = Sql.Select.From("tbl1").Into(Sql.Name("tbl12"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * INTO \"tbl12\" FROM \"tbl1\";", command.CommandText);
        }

        [Fact]
        public void SelectCountStarFromPublicTable()
        {
            var statement = Sql.Select.Output(Sql.Function("COUNT", Sql.Star())).From("public.tbl1");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT COUNT( * ) FROM \"public\".\"tbl1\";", command.CommandText);
        }

        [Fact]
        public void SelectFoo()
        {
            var statement = Sql.Select.Output(Sql.Scalar("Foo1"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT 'Foo1';", command.CommandText);
        }

        [Fact]
        public void SelectFunction()
        {
            var statement = Sql.Select.Output(Sql.Function("NOW").As("Foo"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT NOW( ) AS \"Foo\";", command.CommandText);
        }
        
        [Fact]
        public void Select1Top1()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).As("foo")).Top(1);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT 1 AS \"foo\" LIMIT 1;", command.CommandText);
        }

        [Fact]
        public void Select1OffsetFetch()
        {
            var statement = Sql.Select.From(Sql.Name("public.tbl1").As("foo")).OrderBy("C1").Offset(100).FetchNext(10);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\" AS \"foo\" LIMIT 10 OFFSET 100;", command.CommandText);
        }
        
        [Fact]
        public void Select1Top1Percent()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, true).From("tbl1");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT 1 FROM \"tbl1\" LIMIT ( SELECT ( SELECT COUNT( * ) FROM \"tbl1\" ) * 1 / 100 );", command.CommandText);
        }

        [Fact]
        public void SelectCTE()
        {
            var statement = Sql.With("tbl12").As(Sql.Select.From("public.tbl1")).Select().From("tbl12");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("WITH \"tbl12\" AS ( SELECT * FROM \"public\".\"tbl1\" ) SELECT * FROM \"tbl12\";", command.CommandText);
        }

        [Fact]
        public void SelectCTE2()
        {
            var statement = Sql.With("tbl12", "C1", "C2").As(Sql.Select.Output("C1", "C2").From("public.tbl1")).Select().From("tbl12");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("WITH \"tbl12\" ( \"C1\", \"C2\" ) AS ( SELECT \"C1\", \"C2\" FROM \"public\".\"tbl1\" ) SELECT * FROM \"tbl12\";", command.CommandText);
        }

        [Fact]
        public void SelectCTE3()
        {
            var columns = new List<string> { "C1", "C2" };

            var statement = Sql.With("tbl12", columns).As(Sql.Select.Output(columns).From("public.tbl1")).Select().From("tbl12");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("WITH \"tbl12\" ( \"C1\", \"C2\" ) AS ( SELECT \"C1\", \"C2\" FROM \"public\".\"tbl1\" ) SELECT * FROM \"tbl12\";", command.CommandText);
        }

        [Fact]
        public void SelectStar()
        {
            var statement = Sql.Select.Output(Sql.Star());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT *;", command.CommandText);
        }

        [Fact]
        public void SelectStarPlus()
        {
            var statement = Sql.Select.Output(Sql.Star()).Distinct();

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT DISTINCT *;", command.CommandText);
        }

        [Fact]
        public void SelectStarPlusPlus()
        {
            var statement = Sql.Select.Output(Sql.Star("tl1")).From("public.tbl1", "tl1");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"tl1\".* FROM \"public\".\"tbl1\" AS \"tl1\";", command.CommandText);
        }

        [Fact]
        public void SelectGroupBy1()
        {
            var statement = Sql.Select.Output(Sql.Name("at1", "C1")).From("public.tbl1", "at1").GroupBy(Sql.Name("at1", "C1"), Sql.Name("at1", "C2"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"at1\".\"C1\" FROM \"public\".\"tbl1\" AS \"at1\";", command.CommandText);
        }

        [Fact]
        public void SelectGroupBy2()
        {
            var statement = Sql.Select.Output(Sql.Name("at1", "C1")).From("public.tbl1", "at1").GroupBy("at1.C1", "at1.C2");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"at1\".\"C1\" FROM \"public\".\"tbl1\" AS \"at1\";", command.CommandText);
        }

        [Fact]
        public void SelectGroupBy3()
        {
            var columns = new List<string> { "at1.C1", "at1.C2" };
            var statement = Sql.Select.Output(Sql.Name("at1", "C1")).From("public.tbl1", "at1").GroupBy(columns);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"at1\".\"C1\" FROM \"public\".\"tbl1\" AS \"at1\";", command.CommandText);
        }

        [Fact]
        public void SelectGroupBy4()
        {
            var columns = new List<Name> { "at1.C1", "at1.C2" };
            var statement = Sql.Select.Output(Sql.Name("at1", "C1")).From("public.tbl1", "at1").GroupBy(columns);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"at1\".\"C1\" FROM \"public\".\"tbl1\" AS \"at1\";", command.CommandText);
        }

        [Fact]
        public void SelectGroupByHaving1()
        {
            var statement = Sql.Select.Output(Sql.Name("src", "C1")).From("public.tbl1", "src").GroupBy(Sql.Name("src", "C1"), Sql.Name("src", "C2")).Having(Sql.Name("src", "C1").IsNull());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"src\".\"C1\" FROM \"public\".\"tbl1\" AS \"src\";", command.CommandText);
        }
        
        [Fact]
        public void SelectOrderBy1()
        {
            var statement = Sql.Select.Output(Sql.Name("src.C1")).From("public.tbl1", "src").OrderBy(Sql.Name("src", "C1"), Direction.Asc).OrderBy(Sql.Name("src", "C2"), Direction.Desc);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"src\".\"C1\" FROM \"public\".\"tbl1\" AS \"src\";", command.CommandText);
        }

        [Fact]
        public void SelectOrderBy2()
        {

            var statement = Sql.Select.Output(Sql.Name("src.C1")).From("public.tbl1", "src").OrderBy(Sql.Name("src", "C1"), Direction.Asc).OrderBy(Sql.Order("src.C2", Direction.Desc));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"src\".\"C1\" FROM \"public\".\"tbl1\" AS \"src\";", command.CommandText);
        }

        [Fact]
        public void SelectOrderBy3()
        {
            var orders = new List<Order> { Sql.Order("src.C1"), Sql.Order("src.C2", Direction.Desc) };

            var statement = Sql.Select.Output(Sql.Name("src.C1")).From("public.tbl1", "src").OrderBy(orders);

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"src\".\"C1\" FROM \"public\".\"tbl1\" AS \"src\";", command.CommandText);
        }

        [Fact]
        public void SelectUnionSelect()
        {
            AssertSql(
                Sql.Select.Output(Sql.Name("id")).From("table1").Union(Sql.Select.Output(Sql.Name("id")).From("table2")),
            "SELECT \"id\" FROM \"table1\" UNION SELECT \"id\" FROM \"table2\";");
        }

        [Fact]
        public void SelectUnionSelectWrapped()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("public.tbl1").Union(Sql.Select.Output(Sql.Star()).From("public.tbl1")).WrapAsSelect("wrap");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"wrap\".* FROM ( SELECT * FROM \"public\".\"tbl1\" UNION SELECT * FROM \"public\".\"tbl1\" ) AS \"wrap\";", command.CommandText);
        }
        
        [Fact]
        public void SelectWrapped2()
        {
            var statement = Sql.Select.Output(Sql.Name("C1")).From("public.tbl1").WrapAsSelect("wrap");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"wrap\".\"C1\" FROM ( SELECT \"C1\" FROM \"public\".\"tbl1\" ) AS \"wrap\";", command.CommandText);
        }

        [Fact]
        public void SelectWrapped3()
        {
            var statement = Sql.Select.Output(Sql.Name("C1").As("nm")).From("public.tbl1").WrapAsSelect("wrap");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"wrap\".\"nm\" FROM ( SELECT \"C1\" AS \"nm\" FROM \"public\".\"tbl1\" ) AS \"wrap\";", command.CommandText);
        }

        [Fact]
        public void SelectUnionAllSelect()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("public.tbl1").UnionAll(Sql.Select.Output(Sql.Star()).From("public.tbl2"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\" UNION ALL SELECT * FROM \"public\".\"tbl2\";", command.CommandText);
        }

        [Fact]
        public void SelectUnionDistinctSelect()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("public.tbl1").UnionDistinct(Sql.Select.Output(Sql.Star()).From("public.tbl2"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\" UNION SELECT * FROM \"public\".\"tbl2\";", command.CommandText);
        }

        [Fact]
        public void SelectExceptSelect()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("public.tbl1").Except(Sql.Select.Output(Sql.Star()).From("public.tbl2"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\" EXCEPT SELECT * FROM \"public\".\"tbl2\";", command.CommandText);
        }

        [Fact]
        public void SelectExceptAllSelect()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("public.tbl1").ExceptAll(Sql.Select.Output(Sql.Star()).From("public.tbl2"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\" EXCEPT ALL SELECT * FROM \"public\".\"tbl2\";", command.CommandText);
        }

        [Fact]
        public void SelectExceptDistinctSelect()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("public.tbl1").ExceptDistinct(Sql.Select.Output(Sql.Star()).From("public.tbl2"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\" EXCEPT SELECT * FROM \"public\".\"tbl2\";", command.CommandText);
        }
        [Fact]
        public void SelectIntersectSelect()
        {
            var statement = Sql.Select.Output(Sql.Star("First")).From("public.tbl1", "First").Intersect(Sql.Select.Output(Sql.Star("Second")).From("public.tbl2", "Second"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"First\".* FROM \"public\".\"tbl1\" AS \"First\" INTERSECT SELECT \"Second\".* FROM \"public\".\"tbl2\" AS \"Second\";", command.CommandText);
        }

        [Fact]
        public void SelectIntersectAllSelect()
        {
            var statement = Sql.Select.Output(Sql.Star("First")).From("public.tbl1", "First").IntersectAll(Sql.Select.Output(Sql.Star("Second")).From("public.tbl2", "Second"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"First\".* FROM \"public\".\"tbl1\" AS \"First\" INTERSECT ALL SELECT \"Second\".* FROM \"public\".\"tbl2\" AS \"Second\";", command.CommandText);
        }

        [Fact]
        public void SelectIntersectDistinctSelect()
        {
            var statement = Sql.Select.Output(Sql.Star("First")).From("public.tbl1", "First").IntersectDistinct(Sql.Select.Output(Sql.Star("Second")).From("public.tbl2", "Second"));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"First\".* FROM \"public\".\"tbl1\" AS \"First\" INTERSECT SELECT \"Second\".* FROM \"public\".\"tbl2\" AS \"Second\";", command.CommandText);
        }

        [Fact]
        public void SelectIntersectWrapped()
        {
            var statement =
                Sql.Select.Output(Sql.Star("First"))
                    .From("public.tbl1", "First")
                    .Intersect(Sql.Select.Output(Sql.Star("Second")).From("public.tbl2", "Second"))
                    .WrapAsSelect("wrap");

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT \"wrap\".* FROM ( SELECT \"First\".* FROM \"public\".\"tbl1\" AS \"First\" INTERSECT SELECT \"Second\".* FROM \"public\".\"tbl2\" AS \"Second\" ) AS \"wrap\";", command.CommandText);
        }

        [Fact]
        public void SelectInnerJoin1()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .InnerJoin(Sql.Name("public.tbl2").As("t"), Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" INNER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .InnerJoin(Sql.Name("public.tbl2"), "t", Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" INNER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .InnerJoin("public.tbl2", "t", Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" INNER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");
        }

        [Fact]
        public void SelectLeftOuterJoin1()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .LeftOuterJoin(Sql.Name("public.tbl2").As("t"), Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" LEFT OUTER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .LeftOuterJoin(Sql.Name("public.tbl2"), "t", Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" LEFT OUTER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .LeftOuterJoin("public.tbl2", "t", Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" LEFT OUTER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");
        }

        [Fact]
        public void SelectRightOuterJoin1()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .RightOuterJoin(Sql.Name("public.tbl2").As("t"), Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" RIGHT OUTER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .RightOuterJoin(Sql.Name("public.tbl2"), "t", Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" RIGHT OUTER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .RightOuterJoin("public.tbl2", "t", Sql.Name("t.C1").IsEqual(Sql.Name("o.C1"))),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" RIGHT OUTER JOIN \"public\".\"tbl2\" AS \"t\" ON \"t\".\"C1\" = \"o\".\"C1\";");
        }

        [Fact]
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
        }

        [Fact]
        public void SelectCrossJoin1()
        {
            AssertSql(
                Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .CrossJoin(Sql.Name("public.tbl2").As("t")),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" CROSS JOIN \"public\".\"tbl2\" AS \"t\";");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .CrossJoin(Sql.Name("public.tbl2"), "t"),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" CROSS JOIN \"public\".\"tbl2\" AS \"t\";");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .CrossJoin("public.tbl2", "t"),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" CROSS JOIN \"public\".\"tbl2\" AS \"t\";");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1", "o")
                .CrossJoin("public.tbl2"),
            "SELECT * FROM \"public\".\"tbl1\" AS \"o\" CROSS JOIN \"public\".\"tbl2\";");

        }

        [Fact]
        public void SelectSupeExpression()
        {
            var statement = Sql.Select
                .Output(Sql.Star())
                .From("public.tbl1")
                .Where(
                    Sql.Name("C1")
                        .IsNotNull()
                        .And(
                            Sql.Name("C1").IsNull()
                        .And(
                            Sql.Group(
                            Sql.Name("C1")
                            .Plus(Sql.Scalar(1))
                            .Minus(Sql.Scalar(1))
                            .Multiply(Sql.Scalar(1))
                            .Modulo(Sql.Scalar(1))
                        ).Less(Sql.Scalar(1)))));


            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM \"public\".\"tbl1\" WHERE \"C1\" IS NOT NULL AND \"C1\" IS NULL AND ( \"C1\" + 1 - 1 * 1 % 1 ) < 1;", command.CommandText);
        }
        #endregion
    }
}
