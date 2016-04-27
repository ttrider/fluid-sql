// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace FluidSqlTests
{
    [TestClass]
    public class SelectTest : SqlProviderTests
    {
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
        public void Select1Async()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1));

            var command = Utilities.GetCommandAsync(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT 1;", command.CommandText);
        }

        [TestMethod]
        public void SelectStarFromSysObjects()
        {
            var statement = Sql.Select.From("sys.objects");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects];", command.CommandText);
        }

        [TestMethod]
        public void SelectFromSelectStarFromSysObjects()
        {
            var subselect = Sql.Select.From("sys.objects");

            var statement = Sql.Select.From(subselect.As("foo"));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM ( SELECT * FROM [sys].[objects] ) AS [foo];", command.CommandText);
        }

        [TestMethod]
        public void SelectStarFromSysObjectsAsO()
        {
            var statement = Sql.Select.From("sys.objects", "O");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] AS [O];", command.CommandText);
        }

        [TestMethod]
        public void SelectStarFromSysObjectsAsO2()
        {
            var statement = Sql.Select.From(Sql.Name("sys.objects").As("O"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] AS [O];", command.CommandText);
        }

        [TestMethod]
        public void SelectSkipSchema()
        {
            var statement = Sql.Select.From(Sql.Name("DB", null, "objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [DB]..[objects];", command.CommandText);
        }

        [TestMethod]
        public void SelectSkipDbAndSchema()
        {
            var statement = Sql.Select.From(Sql.Name("server", null, null, "objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [server]...[objects];", command.CommandText);
        }

        [TestMethod]
        public void SelectStarFromSysObjectsInto()
        {
            var statement = Sql.Select.From("sys.objects").Into(Sql.Name("#mytable"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * INTO [#mytable] FROM [sys].[objects];", command.CommandText);
        }

        [TestMethod]
        public void SelectCountStarFromSysObjects()
        {
            var statement = Sql.Select.Output(Sql.Function("COUNT", Sql.Star())).From("sys.objects");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT COUNT( * ) FROM [sys].[objects];", command.CommandText);
        }

        [TestMethod]
        public void SelectFoo()
        {
            var statement = Sql.Select.Output(Sql.Scalar("Foo1"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT N'Foo1';", command.CommandText);
        }

        [TestMethod]
        public void SelectAssignFooIntoBar()
        {
            var statement = Sql.Select.Assign(Sql.Name("@Foo"), Sql.Scalar("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT @Foo = N'bar';", command.CommandText);
        }


        [TestMethod]
        public void SetFooIntoBar()
        {
            var statement = Sql.Set(Sql.Name("@Foo"), Sql.Scalar("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SET @Foo = N'bar';", command.CommandText);
        }

        [TestMethod]
        public void SelectFunction()
        {
            var statement = Sql.Select.Output(Sql.Function("GETUTCDATE").As("Foo"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT GETUTCDATE( ) AS [Foo];", command.CommandText);
        }

        [TestMethod]
        public void SetPlus()
        {
            var statement = Sql.PlusSet(Sql.Name("@Foo"), Sql.Scalar(10));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SET @Foo += 10;", command.CommandText);
        }

        [TestMethod]
        public void SetMinus()
        {
            var statement = Sql.MinusSet(Sql.Name("@Foo"), Sql.Scalar(10));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SET @Foo -= 10;", command.CommandText);
        }

        [TestMethod]
        public void SetMultiply()
        {
            var statement = Sql.MultiplySet(Sql.Name("@Foo"), Sql.Scalar(10));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SET @Foo *= 10;", command.CommandText);
        }

        [TestMethod]
        public void AssignFooIntoBar()
        {
            var statement = Sql.Assign(Sql.Name("@Foo"), Sql.Scalar("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SET @Foo = N'bar';", command.CommandText);
        }

        [TestMethod]
        public void SelectParam1()
        {
            var statement = Sql.Select.Output(Parameter.Any("@param1"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT @param1;", command.CommandText);

            command.Parameters.SetValue("@param1", "Foo");

            var result = command.ExecuteScalar();
            Assert.AreEqual(result, "Foo");
        }

        [TestMethod]
        public void SelectParam1WithDefault()
        {
            var statement = Sql.Select.Output(Parameter.Any("@param1").DefaultValue("Foo"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT @param1;", command.CommandText);

            var result = command.ExecuteScalar();
            Assert.AreEqual(result, "Foo");
        }

        [TestMethod]
        public void SelectParam1WithDefault2()
        {
            var statement = Sql.Select.Output(Parameter.Any("@param1", "Foo"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT @param1;", command.CommandText);

            var result = command.ExecuteScalar();
            Assert.AreEqual(result, "Foo");
        }

        [TestMethod]
        public void SelectParam1As()
        {
            var statement = Sql.Select.Output(Parameter.Any("@param1").As("p1"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT @param1 AS [p1];", command.CommandText);

            command.Parameters.SetValue("@param1", "Foo");

            var result = command.ExecuteScalar();
            Assert.AreEqual(result, "Foo");
        }

        [TestMethod]
        public void Select1Top1()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).As("foo")).Top(1);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT TOP ( 1 ) 1 AS [foo];", command.CommandText);
        }

        [TestMethod]
        public void Select1OffsetFetch()
        {
            var statement = Sql.Select.From(Sql.Name("sys.objects").As("foo")).OrderBy("name").Offset(100).FetchNext(10);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT * FROM [sys].[objects] AS [foo] ORDER BY [name] ASC OFFSET 100 ROWS FETCH NEXT 10 ROWS ONLY;",
                command.CommandText);
        }

        [TestMethod]
        public void Select1Top1Param()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).As("foo")).Top(Parameter.Int("@top").DefaultValue(10));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);

            var param = command.Parameters[0] as DbParameter;
            Assert.IsNotNull(param);
            Assert.AreEqual(param.Value, 10);

            Assert.AreEqual("SELECT TOP ( @top ) 1 AS [foo];", command.CommandText);
        }

        [TestMethod]
        public void Select1Top1ParamP()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).As("foo"))
                .Top(Parameter.Int("@top").DefaultValue(10), true, true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);

            var param = command.Parameters[0] as DbParameter;
            Assert.IsNotNull(param);
            Assert.AreEqual(param.Value, 10);

            Assert.AreEqual("SELECT TOP ( @top ) PERCENT WITH TIES 1 AS [foo];", command.CommandText);
        }

        [TestMethod]
        public void Select1Top1Percent()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT TOP ( 1 ) PERCENT 1;", command.CommandText);
        }

        [TestMethod]
        public void Select1Top1PercentWithTies()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, true, true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT TOP ( 1 ) PERCENT WITH TIES 1;", command.CommandText);
        }

        [TestMethod]
        public void Select1Top1WithTies()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, false, true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT TOP ( 1 ) WITH TIES 1;", command.CommandText);
        }


        [TestMethod]
        public void SelectCTE()
        {
            var statement = Sql.With("SO").As(Sql.Select.From("sys.objects")).Select().From("SO");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("WITH [SO] AS ( SELECT * FROM [sys].[objects] ) SELECT * FROM [SO];", command.CommandText);
        }

        [TestMethod]
        public void SelectCTE2()
        {
            var statement =
                Sql.With("SO", "name", "object_id")
                    .As(Sql.Select.Output("name", "object_id").From("sys.objects"))
                    .Select()
                    .From("SO");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "WITH [SO] ( [name], [object_id] ) AS ( SELECT [name], [object_id] FROM [sys].[objects] ) SELECT * FROM [SO];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectCTE3()
        {
            var columns = new List<string> { "name", "object_id" };

            var statement =
                Sql.With("SO", columns).As(Sql.Select.Output(columns).From("sys.objects")).Select().From("SO");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "WITH [SO] ( [name], [object_id] ) AS ( SELECT [name], [object_id] FROM [sys].[objects] ) SELECT * FROM [SO];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectStar()
        {
            var statement = Sql.Select.Output(Sql.Star());

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT *;", command.CommandText);
        }

        [TestMethod]
        public void SelectStarPlus()
        {
            var statement = Sql.Select.Output(Sql.Star()).Distinct();

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT DISTINCT *;", command.CommandText);
        }

        [TestMethod]
        public void SelectStarPlusPlus()
        {
            var statement = Sql.Select.Output(Sql.Star("src")).From("sys.objects", "src");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [src].* FROM [sys].[objects] AS [src];", command.CommandText);
        }


        [TestMethod]
        public void SelectGroupBy()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src", "object_id"))
                    .From("sys.objects", "src")
                    .GroupBy(Sql.Name("src", "object_id"), Sql.Name("src", "name"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectGroupBy2()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src", "object_id"))
                    .From("sys.objects", "src")
                    .GroupBy("src.object_id", "src.name");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectGroupBy3()
        {
            var columns = new List<string> { "src.object_id", "src.name" };
            var statement = Sql.Select.Output(Sql.Name("src", "object_id")).From("sys.objects", "src").GroupBy(columns);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectGroupBy4()
        {
            var columns = new List<Name> { "src.object_id", "src.name" };
            var statement = Sql.Select.Output(Sql.Name("src", "object_id")).From("sys.objects", "src").GroupBy(columns);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectGroupByHaving()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src", "object_id"))
                    .From("sys.objects", "src")
                    .GroupBy(Sql.Name("src", "object_id"), Sql.Name("src", "name"))
                    .Having(Sql.Name("src", "object_id").IsNull());

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name] HAVING [src].[object_id] IS NULL;",
                command.CommandText);
        }

        [TestMethod]
        public void SelectOrderBy()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src.object_id"))
                    .From("sys.objects", "src")
                    .OrderBy(Sql.Name("src", "object_id"), Direction.Asc)
                    .OrderBy(Sql.Name("src", "name"), Direction.Desc);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] ORDER BY [src].[object_id] ASC, [src].[name] DESC;",
                command.CommandText);
        }

        [TestMethod]
        public void SelectOrderBy2()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src.object_id"))
                    .From("sys.objects", "src")
                    .OrderBy(Sql.Name("src", "object_id"), Direction.Asc)
                    .OrderBy(Sql.Order("src.name", Direction.Desc));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] ORDER BY [src].[object_id] ASC, [src].[name] DESC;",
                command.CommandText);
        }

        [TestMethod]
        public void SelectOrderBy3()
        {
            var orders = new List<Order> { Sql.Order("src.object_id"), Sql.Order("src.name", Direction.Desc) };

            var statement = Sql.Select.Output(Sql.Name("src.object_id")).From("sys.objects", "src").OrderBy(orders);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] ORDER BY [src].[object_id] ASC, [src].[name] DESC;",
                command.CommandText);
        }

        //[TestMethod]
        //public void SelectFromSelect()
        //{
        //    var statement = Sql.Select.OutputStar().From(Sql.Select.OutputStar().From("sys.objects"));

        //    var command = Utilities.GetCommand(statement);

        //    Assert.IsNotNull(command);
        //    Assert.AreEqual("SELECT [src].[object_id] FROM [sys].[objects] AS [src] ORDER BY [src].[object_id] ASC, [src].[name] DESC", command.CommandText);
        //}

        [TestMethod]
        public void SelectUnionSelect()
        {
            var statement =
                Sql.Select.Output(Sql.Star())
                    .From("sys.objects")
                    .Union(Sql.Select.Output(Sql.Star()).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] UNION SELECT * FROM [sys].[objects];", command.CommandText);
        }

        [TestMethod]
        public void SelectUnionSelectWrapped()
        {
            var statement =
                Sql.Select.Output(Sql.Star())
                    .From("sys.objects")
                    .Union(Sql.Select.Output(Sql.Star()).From("sys.objects"))
                    .WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [wrap].* FROM ( SELECT * FROM [sys].[objects] UNION SELECT * FROM [sys].[objects] ) AS [wrap];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectWrapped2()
        {
            var statement = Sql.Select.Output(Sql.Name("Name")).From("sys.objects").WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [wrap].[Name] FROM ( SELECT [Name] FROM [sys].[objects] ) AS [wrap];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectWrapped3()
        {
            var statement = Sql.Select.Output(Sql.Name("Name").As("nm")).From("sys.objects").WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [wrap].[nm] FROM ( SELECT [Name] AS [nm] FROM [sys].[objects] ) AS [wrap];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectUnionAllSelect()
        {
            var statement =
                Sql.Select.Output(Sql.Star())
                    .From("sys.objects")
                    .UnionAll(Sql.Select.Output(Sql.Star()).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] UNION ALL SELECT * FROM [sys].[objects];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectExceptSelect()
        {
            var statement =
                Sql.Select.Output(Sql.Star())
                    .From("sys.objects")
                    .Except(Sql.Select.Output(Sql.Star()).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] EXCEPT SELECT * FROM [sys].[objects];", command.CommandText);
        }

        [TestMethod]
        public void SelectIntersectSelect()
        {
            var statement =
                Sql.Select.Output(Sql.Star("First"))
                    .From("sys.objects", "First")
                    .Intersect(Sql.Select.Output(Sql.Star("Second")).From("sys.objects", "Second"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [First].* FROM [sys].[objects] AS [First] INTERSECT SELECT [Second].* FROM [sys].[objects] AS [Second];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectIntersectWrapped()
        {
            var statement =
                Sql.Select.Output(Sql.Star("First"))
                    .From("sys.objects", "First")
                    .Intersect(Sql.Select.Output(Sql.Star("Second")).From("sys.objects", "Second"))
                    .WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT [wrap].* FROM ( SELECT [First].* FROM [sys].[objects] AS [First] INTERSECT SELECT [Second].* FROM [sys].[objects] AS [Second] ) AS [wrap];",
                command.CommandText);
        }

        [TestMethod]
        public void SelectInnerJoin()
        {
            AssertSql(
                Sql.Select
                    .Output(Sql.Star())
                    .From("sys.objects", "o")
                    .InnerJoin(Sql.Name("sys.tables").As("t"), Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .InnerJoin(Sql.Name("sys.tables"), "t", Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .InnerJoin("sys.tables", "t", Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .InnerJoin("sys.tables", Sql.Name("object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] INNER JOIN [sys].[tables] ON [object_id] = [o].[object_id];");
        }

        [TestMethod]
        public void SelectLeftOuterJoin()
        {
            AssertSql(
                Sql.Select
                    .Output(Sql.Star())
                    .From("sys.objects", "o")
                    .LeftOuterJoin(Sql.Name("sys.tables").As("t"),
                        Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] LEFT OUTER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .LeftOuterJoin(Sql.Name("sys.tables"), "t", Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] LEFT OUTER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .LeftOuterJoin("sys.tables", "t", Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] LEFT OUTER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .LeftOuterJoin("sys.tables", Sql.Name("object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] LEFT OUTER JOIN [sys].[tables] ON [object_id] = [o].[object_id];");
        }

        [TestMethod]
        public void SelectRightOuterJoin()
        {
            AssertSql(
                Sql.Select
                    .Output(Sql.Star())
                    .From("sys.objects", "o")
                    .RightOuterJoin(Sql.Name("sys.tables").As("t"),
                        Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] RIGHT OUTER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .RightOuterJoin(Sql.Name("sys.tables"), "t", Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] RIGHT OUTER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .RightOuterJoin("sys.tables", "t", Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] RIGHT OUTER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .RightOuterJoin("sys.tables", Sql.Name("object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] RIGHT OUTER JOIN [sys].[tables] ON [object_id] = [o].[object_id];");
        }


        [TestMethod]
        public void SelectFullOuterJoin()
        {
            AssertSql(
                Sql.Select
                    .Output(Sql.Star())
                    .From("sys.objects", "o")
                    .FullOuterJoin(Sql.Name("sys.tables").As("t"),
                        Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] FULL OUTER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .FullOuterJoin(Sql.Name("sys.tables"), "t", Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] FULL OUTER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .FullOuterJoin("sys.tables", "t", Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] FULL OUTER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .FullOuterJoin("sys.tables", Sql.Name("object_id").IsEqual(Sql.Name("o.object_id"))),
                "SELECT * FROM [sys].[objects] AS [o] FULL OUTER JOIN [sys].[tables] ON [object_id] = [o].[object_id];");
        }


        [TestMethod]
        public void SelectCrossJoin()
        {
            AssertSql(
                Sql.Select
                    .Output(Sql.Star())
                    .From("sys.objects", "o")
                    .CrossJoin(Sql.Name("sys.tables").As("t")),
                "SELECT * FROM [sys].[objects] AS [o] CROSS JOIN [sys].[tables] AS [t];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .CrossJoin(Sql.Name("sys.tables"), "t"),
                "SELECT * FROM [sys].[objects] AS [o] CROSS JOIN [sys].[tables] AS [t];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .CrossJoin("sys.tables", "t"),
                "SELECT * FROM [sys].[objects] AS [o] CROSS JOIN [sys].[tables] AS [t];");

            AssertSql(Sql.Select
                .Output(Sql.Star())
                .From("sys.objects", "o")
                .CrossJoin("sys.tables"),
                "SELECT * FROM [sys].[objects] AS [o] CROSS JOIN [sys].[tables];");
        }


        [TestMethod]
        public void SelectLike()
        {
            var statement = Sql.Select
                .Output(Sql.Star())
                .From("sys.objects")
                .Where(
                    Sql.Name("name").StartsWith(Sql.Scalar("foo"))
                        .And(
                            Sql.Name("name").EndsWith(Sql.Scalar("foo")))
                        .And(
                            Sql.Name("name").Contains(Sql.Scalar("foo")))
                        .And(
                            Sql.Name("name").Like(Sql.Scalar("%foo%"))));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT * FROM [sys].[objects] WHERE [name] LIKE N'foo' + N'%' AND [name] LIKE N'%' + N'foo' AND [name] LIKE N'%' + N'foo' + N'%' AND [name] LIKE N'%foo%';",
                command.CommandText);
        }

        [TestMethod]
        public void SelectSupeExpression()
        {
            var statement = Sql.Select
                .Output(Sql.Star())
                .From("sys.objects")
                .Where(
                    Sql.Name("object_id")
                        .IsNotNull()
                        .And(
                            Sql.Name("object_id").IsNull()
                                .And(
                                    Sql.Group(
                                        Sql.Name("object_id")
                                            .Plus(Sql.Scalar(1))
                                            .Minus(Sql.Scalar(1))
                                            .Multiply(Sql.Scalar(1))
                                            .Divide(Sql.Scalar(1))
                                            .Modulo(Sql.Scalar(1))
                                            .BitwiseAnd(Sql.Scalar(1))
                                            .BitwiseOr(Sql.Scalar(1))
                                            .BitwiseXor(Sql.Scalar(1))
                                        ).Less(Sql.Scalar(1)))));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "SELECT * FROM [sys].[objects] WHERE [object_id] IS NOT NULL AND [object_id] IS NULL AND ( [object_id] + 1 - 1 * 1 / 1 % 1 & 1 | 1 ^ 1 ) < 1;",
                command.CommandText);
        }

        [TestMethod]
        public void BeginCommitTransactions()
        {
            var statement = Sql.Statements(Sql.BeginTransaction(), Sql.CommitTransaction());

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION;\r\nCOMMIT TRANSACTION;", command.CommandText);
        }

        [TestMethod]
        public void BeginRollbackTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION;\r\nSAVE TRANSACTION;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;",
                command.CommandText);
        }

        [TestMethod]
        public void BeginRollbackMarkedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("foo"), "marked"),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "BEGIN TRANSACTION [foo] WITH MARK N'marked';\r\nSAVE TRANSACTION;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;",
                command.CommandText);
        }

        [TestMethod]
        public void BeginRollbackNamedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("t")),
                Sql.SaveTransaction(Sql.Name("s")),
                Sql.RollbackTransaction(Sql.Name("s")),
                Sql.CommitTransaction(Sql.Name("t")));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "BEGIN TRANSACTION [t];\r\nSAVE TRANSACTION [s];\r\nROLLBACK TRANSACTION [s];\r\nCOMMIT TRANSACTION [t];",
                command.CommandText);
        }

        [TestMethod]
        public void BeginRollbackParameterTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Parameter.Any("@t")),
                Sql.SaveTransaction(Parameter.Any("@s")),
                Sql.RollbackTransaction(Parameter.Any("@s")),
                Sql.CommitTransaction(Parameter.Any("@t")));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "BEGIN TRANSACTION @t;\r\nSAVE TRANSACTION @s;\r\nROLLBACK TRANSACTION @s;\r\nCOMMIT TRANSACTION @t;",
                command.CommandText);
        }

        [TestMethod]
        public void DeclareSelectNameFromSysObjects()
        {
            var statement = Sql.Declare(Parameter.NVarChar("@name"),
                Sql.Select.Top(1).Output(Sql.Name("name")).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DECLARE @name NVARCHAR ( MAX ) = ( SELECT TOP ( 1 ) [name] FROM [sys].[objects] );",
                command.CommandText);
        }

        [TestMethod]
        public void IfThenElse()
        {
            var statement = Sql.If(Sql.Function("EXISTS",
                Sql.Select.From("tempdb.sys.tables").Where(Sql.Name("name").IsEqual(Parameter.NVarChar("@name")))))
                .Then(Sql.Select.Output(Sql.Scalar(1)))
                .Else(Sql.Select.Output(Sql.Scalar(2)))
                ;

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(
                "IF EXISTS( ( SELECT * FROM [tempdb].[sys].[tables] WHERE [name] = @name ) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;\r\nELSE\r\nBEGIN;\r\nSELECT 2;\r\nEND;",
                command.CommandText);
        }


        [TestMethod]
        public void DropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP TABLE [some].[table];", command.CommandText);
        }

        [TestMethod]
        public void CommentOutDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).CommentOut();

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("/* DROP TABLE [some].[table] */", command.CommandText);
        }

        [TestMethod]
        public void CommentOutName()
        {
            AssertSqlToken(Sql.Name("some.table").CommentOut(), "/* [some].[table] */");
        }

        [TestMethod]
        public void StringifyDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).Stringify();

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("N' DROP TABLE [some].[table]';", command.CommandText);
        }

        [TestMethod]
        public void StringifyName()
        {
            AssertSqlToken(Sql.Name("some.table").Stringify(), "N' [some].[table]'");
        }

        [TestMethod]
        public void DropTableExists()
        {
            var statement = Sql.DropTable(Sql.Name("some.table"), true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF OBJECT_ID ( N'[some].[table]', N'U' ) IS NOT NULL DROP TABLE [some].[table];",
                command.CommandText);
        }

        [TestMethod]
        public void IfExists()
        {
            var statement = Sql.If(Sql.Exists(Sql.Select.From("sys.objects"))).Then(Sql.Select.Output(Sql.Scalar(1)));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF EXISTS ( SELECT * FROM [sys].[objects] )\r\nBEGIN;\r\nSELECT 1;\r\nEND;",
                command.CommandText);
        }

        [TestMethod]
        public void IfNotExists()
        {
            var statement =
                Sql.If(Sql.Exists(Sql.Select.From("sys.objects")).Not()).Then(Sql.Select.Output(Sql.Scalar(1)));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF NOT ( EXISTS ( SELECT * FROM [sys].[objects] ) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;",
                command.CommandText);
        }

        [TestMethod]
        public void IfNotExists2()
        {
            var statement = Sql.If(Sql.NotExists(Sql.Select.From("sys.objects"))).Then(Sql.Select.Output(Sql.Scalar(1)));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF NOT ( EXISTS ( SELECT * FROM [sys].[objects] ) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;",
                command.CommandText);
        }

        [TestMethod]
        public void IfNotExists3()
        {
            var statement =
                Sql.If(Sql.Not(Sql.Exists(Sql.Select.From("sys.objects")))).Then(Sql.Select.Output(Sql.Scalar(1)));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF NOT ( EXISTS ( SELECT * FROM [sys].[objects] ) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;",
                command.CommandText);
        }

        //[TestMethod]
        //public void CreateTable()
        //{
        //    var statement = Sql.CreateTable(Sql.Name("some.table"));

        //    var command = Utilities.GetCommand(statement);

        //    Assert.IsNotNull(command);
        //    Assert.AreEqual("CREATE TABLE [some].[table] ();", command.CommandText);
        //}

        [TestMethod]
        public void IfSome()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Select.Output(Sql.Scalar(1)));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nSELECT 1;\r\nEND;", command.CommandText);
        }

        [TestMethod]
        public void IfBreak()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Break);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nBREAK;\r\nEND;", command.CommandText);
        }

        [TestMethod]
        public void IfThrow()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Throw(123, "123", 1));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nTHROW 123, N'123', 1;\r\nEND;",
                command.CommandText);
        }

        [TestMethod]
        public void IfThrow2()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Throw(Sql.Scalar(123), Sql.Name("@fooMsg"), Sql.Scalar(1)));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nTHROW 123, @fooMsg, 1;\r\nEND;",
                command.CommandText);
        }


        [TestMethod]
        public void IfContinue()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Continue);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nCONTINUE;\r\nEND;", command.CommandText);
        }

        [TestMethod]
        public void IfAll()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.All(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Select.Output(Sql.Scalar(1)));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF 3 < ALL ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nSELECT 1;\r\nEND;", command.CommandText);
        }


        [TestMethod]
        public void SelectOperationEq()
        {
            var statement = Sql.Select.Output(Sql.Name("foo").PlusEqual(Sql.Scalar(1))).From("sys.objects");
            var command = Utilities.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [foo] += 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").MinusEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [foo] -= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").MultiplyEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [foo] *= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").DivideEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [foo] /= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").ModuloEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [foo] %= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").BitwiseAndEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [foo] &= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").BitwiseOrEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [foo] |= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").BitwiseXorEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [foo] ^= 1 FROM [sys].[objects];", command.CommandText);
        }

        [TestMethod]
        public void GotoLabel()
        {
            var statement =
                Sql.Statements(
                    Sql.Goto("foo"),
                    Sql.Select.Output(Sql.Scalar(1)),
                    Sql.Label("foo"),
                    Sql.Select.Output(Sql.Scalar(2)));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("GOTO foo;\r\nSELECT 1;\r\nfoo:\r\nSELECT 2;", command.CommandText);
        }

        [TestMethod]
        public void Return()
        {
            var statement =
                Sql.Return();

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("RETURN;", command.CommandText);
        }

        [TestMethod]
        public void Return2()
        {
            var statement =
                Sql.Return(2);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("RETURN 2;", command.CommandText);
        }

        [TestMethod]
        public void ReturnVar()
        {
            var statement =
                Sql.Return(Sql.Name("@var"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("RETURN @var;", command.CommandText);
        }

        [TestMethod]
        public void TryCatch()
        {
            var statement =
                Sql.Try(Sql.Select.Output(Sql.Scalar(1))).Catch(Sql.Throw());

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRY\r\nSELECT 1;\r\nEND TRY\r\nBEGIN CATCH\r\nTHROW;\r\nEND CATCH;",
                command.CommandText);
        }


        [TestMethod]
        public void While()
        {
            var statement =
                Sql.While(Sql.Name("@i").Less(Sql.Scalar(10))).Do(Sql.Set("@i", Sql.Name("@i").Plus(Sql.Scalar(1))));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("WHILE @i < 10\r\nBEGIN;\r\nSET @i = @i + 1;\r\nEND;", command.CommandText);
        }


        [TestMethod]
        public void Snippet01()
        {
            var statement = Sql.SnippetStatement("SELECT * FROM sys.objects");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM sys.objects;", command.CommandText);
        }

        [TestMethod]
        public void Snippet02()
        {
            var statement = Sql.TemplateStatement("SELECT {0},{0} FROM sys.objects", Sql.Name("foo"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT  [foo], [foo]  FROM sys.objects;", command.CommandText);
        }


        [TestMethod]
        public void SelectParameters()
        {
            AssertSql(
                Sql.Select.Output(Sql.Parameter.Int("@foo")),
                "SELECT @foo;"
                );
        }

        [TestMethod]
        public void SelectIntoParameters()
        {
            AssertSql(
                Sql.Select.From(Sql.Name("foo")).Set(Sql.Parameter.Int("@foo"), Sql.Name("bar")).Top(1),
                "SELECT TOP ( 1 ) @foo = [bar] FROM [foo];"
                );
        }

        [TestMethod]
        public void CaseOnValue()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.Case.On(Parameter.Int("@foo"))
                        .When(Sql.Scalar(123), Sql.Scalar("123"))
                        .When(Sql.Scalar(234), Sql.Scalar("234"))
                        .Else(Sql.Scalar("345"))),
                "SELECT CASE @foo WHEN 123 THEN N'123' WHEN 234 THEN N'234' ELSE N'345' END;"
                );
        }

        [TestMethod]
        public void Case()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.Case.When(Sql.Scalar(123).IsEqual(Parameter.Int("@foo")), Sql.Scalar("123"))
                        .When(Sql.Scalar(234).IsEqual(Parameter.Int("@foo")), Sql.Scalar("234"))
                        .Else(Sql.Scalar("345"))),
                "SELECT CASE WHEN 123 = @foo THEN N'123' WHEN 234 = @foo THEN N'234' ELSE N'345' END;"
                );
        }
    }
}