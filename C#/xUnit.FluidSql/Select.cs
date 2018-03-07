// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using System.Data.Common;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.MySql;
using Xunit;

namespace xUnit.FluidSql
{
    public class SelectTest : SqlProviderTests
    {
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
                "SELECT N'this is the string with '' inside';"
                );
        }

        /*[Fact]
        public void Select1Async()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1));

            var command = Utilities.GetCommandAsync(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT 1;", command.CommandText);
        }*/

        [Fact]
        public void SelectStarFromSysObjects()
        {
            var statement = Sql.Select.From("sys.objects");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [sys].[objects];", command.CommandText);
        }

        [Fact]
        public void SelectFromSelectStarFromSysObjects()
        {
            var subselect = Sql.Select.From("sys.objects");

            var statement = Sql.Select.From(subselect.As("foo"));


            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM ( SELECT * FROM [sys].[objects] ) AS [foo];", command.CommandText);
        }

        [Fact]
        public void SelectStarFromSysObjectsAsO()
        {
            var statement = Sql.Select.From("sys.objects", "O");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [sys].[objects] AS [O];", command.CommandText);
        }

        [Fact]
        public void SelectStarFromSysObjectsAsO2()
        {
            var statement = Sql.Select.From(Sql.Name("sys.objects").As("O"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [sys].[objects] AS [O];", command.CommandText);
        }

        [Fact]
        public void SelectSkipSchema()
        {
            var statement = Sql.Select.From(Sql.Name("DB", null, "objects"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [DB]..[objects];", command.CommandText);
        }

        [Fact]
        public void SelectSkipDbAndSchema()
        {
            var statement = Sql.Select.From(Sql.Name("server", null, null, "objects"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [server]...[objects];", command.CommandText);
        }

        [Fact]
        public void SelectStarFromSysObjectsInto()
        {
            var statement = Sql.Select.From("sys.objects").Into(Sql.Name("#mytable"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * INTO [#mytable] FROM [sys].[objects];", command.CommandText);
        }

        [Fact]
        public void SelectCountStarFromSysObjects()
        {
            var statement = Sql.Select.Output(Sql.Function("COUNT", Sql.Star())).From("sys.objects");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT COUNT( * ) FROM [sys].[objects];", command.CommandText);
        }

        [Fact]
        public void SelectFoo()
        {
            var statement = Sql.Select.Output(Sql.Scalar("Foo1"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT N'Foo1';", command.CommandText);
        }

        [Fact]
        public void SelectAssignFooIntoBar()
        {
            var statement = Sql.Select.Assign(Sql.Name("@Foo"), Sql.Scalar("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT @Foo = N'bar';", command.CommandText);
        }

        [Fact]
        public void SetFooIntoBar()
        {
            var statement = Sql.Set(Sql.Name("@Foo"), Sql.Scalar("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SET @Foo = N'bar';", command.CommandText);
        }

        [Fact]
        public void SelectFunction()
        {
            var statement = Sql.Select.Output(Sql.Function("GETUTCDATE").As("Foo"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT GETUTCDATE( ) AS [Foo];", command.CommandText);
        }

        [Fact]
        public void SetPlus()
        {
            var statement = Sql.PlusSet(Sql.Name("@Foo"), Sql.Scalar(10));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SET @Foo += 10;", command.CommandText);
        }

        [Fact]
        public void SetMinus()
        {
            var statement = Sql.MinusSet(Sql.Name("@Foo"), Sql.Scalar(10));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SET @Foo -= 10;", command.CommandText);
        }

        [Fact]
        public void SetMultiply()
        {
            var statement = Sql.MultiplySet(Sql.Name("@Foo"), Sql.Scalar(10));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SET @Foo *= 10;", command.CommandText);
        }

        [Fact]
        public void AssignFooIntoBar()
        {
            var statement = Sql.Assign(Sql.Name("@Foo"), Sql.Scalar("bar"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SET @Foo = N'bar';", command.CommandText);
        }

        [Fact]
        public void SelectParam1()
        {
            var statement = Sql.Select.Output(Parameter.Any("@param1"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT @param1;", command.CommandText);

            //command.Parameters.SetValue("@param1", "Foo");

            //var result = command.ExecuteScalar();
            //Assert.Equal(result, "Foo");
        }

        [Fact]
        public void SelectParam1WithDefault()
        {
            var statement = Sql.Select.Output(Parameter.Any("@param1").DefaultValue("Foo"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT @param1;", command.CommandText);

            //var result = command.ExecuteScalar();
            //Assert.Equal(result, "Foo");
        }

        [Fact]
        public void SelectParam1WithDefault2()
        {
            var statement = Sql.Select.Output(Parameter.Any("@param1", "Foo"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT @param1;", command.CommandText);

            //var result = command.ExecuteScalar();
            //Assert.Equal(result, "Foo");
        }

        [Fact]
        public void SelectParam1As()
        {
            var statement = Sql.Select.Output(Parameter.Any("@param1").As("p1"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT @param1 AS [p1];", command.CommandText);

            //command.Parameters.SetValue("@param1", "Foo");

            //var result = command.ExecuteScalar();
            //Assert.Equal(result, "Foo");
        }

        [Fact]
        public void Select1Top1()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).As("foo")).Top(1);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT TOP ( 1 ) 1 AS [foo];", command.CommandText);
        }

        [Fact]
        public void Select1OffsetFetch()
        {
            var statement = Sql.Select.From(Sql.Name("sys.objects").As("foo")).OrderBy("name").Offset(100).FetchNext(10);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT * FROM [sys].[objects] AS [foo] ORDER BY [name] ASC OFFSET 100 ROWS FETCH NEXT 10 ROWS ONLY;",
                command.CommandText);
        }

        [Fact]
        public void Select1Top1Param()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).As("foo")).Top(Parameter.Int("@top").DefaultValue(10));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);

            var param = command.Parameters[0] as DbParameter;
            Assert.NotNull(param);
            Assert.Equal(param.Value, 10);

            Assert.Equal("SELECT TOP ( @top ) 1 AS [foo];", command.CommandText);
        }

        [Fact]
        public void Select1Top1ParamP()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).As("foo"))
                .Top(Parameter.Int("@top").DefaultValue(10), true, true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);

            var param = command.Parameters[0] as DbParameter;
            Assert.NotNull(param);
            Assert.Equal(param.Value, 10);

            Assert.Equal("SELECT TOP ( @top ) PERCENT WITH TIES 1 AS [foo];", command.CommandText);
        }

        [Fact]
        public void Select1Top1Percent()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT TOP ( 1 ) PERCENT 1;", command.CommandText);
        }

        [Fact]
        public void Select1Top1PercentWithTies()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, true, true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT TOP ( 1 ) PERCENT WITH TIES 1;", command.CommandText);
        }

        [Fact]
        public void Select1Top1WithTies()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, false, true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT TOP ( 1 ) WITH TIES 1;", command.CommandText);
        }

        [Fact]
        public void SelectCTE()
        {
            var statement = Sql.With("SO").As(Sql.Select.From("sys.objects")).Select().From("SO");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("WITH [SO] AS ( SELECT * FROM [sys].[objects] ) SELECT * FROM [SO];", command.CommandText);
        }

        [Fact]
        public void SelectCTE2()
        {
            var statement =
                Sql.With("SO", "name", "object_id")
                    .As(Sql.Select.Output("name", "object_id").From("sys.objects"))
                    .Select()
                    .From("SO");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "WITH [SO] ( [name], [object_id] ) AS ( SELECT [name], [object_id] FROM [sys].[objects] ) SELECT * FROM [SO];",
                command.CommandText);
        }

        [Fact]
        public void SelectCTE3()
        {
            var columns = new List<string> { "name", "object_id" };

            var statement =
                Sql.With("SO", columns).As(Sql.Select.Output(columns).From("sys.objects")).Select().From("SO");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "WITH [SO] ( [name], [object_id] ) AS ( SELECT [name], [object_id] FROM [sys].[objects] ) SELECT * FROM [SO];",
                command.CommandText);
        }

        [Fact]
        public void SelectStar()
        {
            var statement = Sql.Select.Output(Sql.Star());

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT *;", command.CommandText);
        }

        [Fact]
        public void SelectStarPlus()
        {
            var statement = Sql.Select.Output(Sql.Star()).Distinct();

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT DISTINCT *;", command.CommandText);
        }

        [Fact]
        public void SelectStarPlusPlus()
        {
            var statement = Sql.Select.Output(Sql.Star("src")).From("sys.objects", "src");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT [src].* FROM [sys].[objects] AS [src];", command.CommandText);
        }

        [Fact]
        public void SelectGroupBy()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src", "object_id"))
                    .From("sys.objects", "src")
                    .GroupBy(Sql.Name("src", "object_id"), Sql.Name("src", "name"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name];",
                command.CommandText);
        }

        [Fact]
        public void SelectGroupBy2()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src", "object_id"))
                    .From("sys.objects", "src")
                    .GroupBy("src.object_id", "src.name");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name];",
                command.CommandText);
        }

        [Fact]
        public void SelectGroupBy3()
        {
            var columns = new List<string> { "src.object_id", "src.name" };
            var statement = Sql.Select.Output(Sql.Name("src", "object_id")).From("sys.objects", "src").GroupBy(columns);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name];",
                command.CommandText);
        }

        [Fact]
        public void SelectGroupBy4()
        {
            var columns = new List<Name> { "src.object_id", "src.name" };
            var statement = Sql.Select.Output(Sql.Name("src", "object_id")).From("sys.objects", "src").GroupBy(columns);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name];",
                command.CommandText);
        }

        [Fact]
        public void SelectGroupByHaving()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src", "object_id"))
                    .From("sys.objects", "src")
                    .GroupBy(Sql.Name("src", "object_id"), Sql.Name("src", "name"))
                    .Having(Sql.Name("src", "object_id").IsNull());

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name] HAVING [src].[object_id] IS NULL;",
                command.CommandText);
        }

        [Fact]
        public void SelectOrderBy()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src.object_id"))
                    .From("sys.objects", "src")
                    .OrderBy(Sql.Name("src", "object_id"), Direction.Asc)
                    .OrderBy(Sql.Name("src", "name"), Direction.Desc);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] ORDER BY [src].[object_id] ASC, [src].[name] DESC;",
                command.CommandText);
        }

        [Fact]
        public void SelectOrderBy2()
        {
            var statement =
                Sql.Select.Output(Sql.Name("src.object_id"))
                    .From("sys.objects", "src")
                    .OrderBy(Sql.Name("src", "object_id"), Direction.Asc)
                    .OrderBy(Sql.Order("src.name", Direction.Desc));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] ORDER BY [src].[object_id] ASC, [src].[name] DESC;",
                command.CommandText);
        }

        [Fact]
        public void SelectOrderBy3()
        {
            var orders = new List<Order> { Sql.Order("src.object_id"), Sql.Order("src.name", Direction.Desc) };

            var statement = Sql.Select.Output(Sql.Name("src.object_id")).From("sys.objects", "src").OrderBy(orders);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [src].[object_id] FROM [sys].[objects] AS [src] ORDER BY [src].[object_id] ASC, [src].[name] DESC;",
                command.CommandText);
        }

        [Fact]
        public void SelectUnionSelect()
        {
            var statement =
                Sql.Select.Output(Sql.Star())
                    .From("sys.objects")
                    .Union(Sql.Select.Output(Sql.Star()).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [sys].[objects] UNION SELECT * FROM [sys].[objects];", command.CommandText);
        }

        [Fact]
        public void SelectUnionSelectWrapped()
        {
            var statement =
                Sql.Select.Output(Sql.Star())
                    .From("sys.objects")
                    .Union(Sql.Select.Output(Sql.Star()).From("sys.objects"))
                    .WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [wrap].* FROM ( SELECT * FROM [sys].[objects] UNION SELECT * FROM [sys].[objects] ) AS [wrap];",
                command.CommandText);
        }

        [Fact]
        public void SelectWrapped2()
        {
            var statement = Sql.Select.Output(Sql.Name("Name")).From("sys.objects").WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT [wrap].[Name] FROM ( SELECT [Name] FROM [sys].[objects] ) AS [wrap];",
                command.CommandText);
        }

        [Fact]
        public void SelectWrapped3()
        {
            var statement = Sql.Select.Output(Sql.Name("Name").As("nm")).From("sys.objects").WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT [wrap].[nm] FROM ( SELECT [Name] AS [nm] FROM [sys].[objects] ) AS [wrap];",
                command.CommandText);
        }

        [Fact]
        public void SelectUnionAllSelect()
        {
            var statement =
                Sql.Select.Output(Sql.Star())
                    .From("sys.objects")
                    .UnionAll(Sql.Select.Output(Sql.Star()).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [sys].[objects] UNION ALL SELECT * FROM [sys].[objects];",
                command.CommandText);
        }

        [Fact]
        public void SelectExceptSelect()
        {
            var statement =
                Sql.Select.Output(Sql.Star())
                    .From("sys.objects")
                    .Except(Sql.Select.Output(Sql.Star()).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM [sys].[objects] EXCEPT SELECT * FROM [sys].[objects];", command.CommandText);
        }

        [Fact]
        public void SelectIntersectSelect()
        {
            var statement =
                Sql.Select.Output(Sql.Star("First"))
                    .From("sys.objects", "First")
                    .Intersect(Sql.Select.Output(Sql.Star("Second")).From("sys.objects", "Second"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [First].* FROM [sys].[objects] AS [First] INTERSECT SELECT [Second].* FROM [sys].[objects] AS [Second];",
                command.CommandText);
        }


        [Fact]
        public void SelectIntersectSelectMysql()
        {
            var inAlias = "v0";
            var personAlias = "v1";
            var nodeHashParameter = Sql.Parameter.UniqueIdentifier("@p2");
            nodeHashParameter.DefaultValue = "af8cb312-3fa8-f9ca-aa8d-4e0e67ef27e1";
            var tableName = Sql.Name("system", "PHPerson_NodeMagic");

            var leftStatement = Sql.Select
                .From(Sql.Name("PHS", "person").As(personAlias))
                .Output(Sql.Name(personAlias, "person_id").As("PHS_PHPerson_person_id"));

            var rightStatement = Sql.Select
                .From(tableName)
                .Where(Sql.Name("nodeHash").IsEqual(nodeHashParameter))
                .Output(Sql.NameAs("person_id", "PHS_PHPerson_person_id"));


          var statement = leftStatement.Intersect(rightStatement);

            var insertStatemt = Sql.Insert
                    .Into(tableName)
                    .From(Sql.Select
                        .Distinct()
                        .From(statement, inAlias)
                        .Output(nodeHashParameter)
                        .Output(Sql.NameAs(inAlias, "PHS_PHPerson_person_id", "PHS_PHPerson_person_id")));

            var command =new MySqlProvider().GetCommand(statement, string.Empty);
            var insertCommand = new MySqlProvider().GetCommand(insertStatemt, string.Empty);

            Assert.NotNull(command);
            Assert.NotNull(insertCommand);
            Assert.Equal("SELECT `v1`.`person_id` AS `PHS_PHPerson_person_id` FROM `PHS`.`person` AS `v1` WHERE EXISTS( ( SELECT * FROM `system`.`PHPerson_NodeMagic` WHERE `v1`.`person_id` = `system`.`PHPerson_NodeMagic`.`person_id` AND `PHPerson_NodeMagic`.`nodeHash` = @p2 ) );",
                command.CommandText);
            Assert.Equal("INSERT INTO `system`.`PHPerson_NodeMagic` SELECT DISTINCT @p2, `v0`.`PHS_PHPerson_person_id` AS `PHS_PHPerson_person_id` FROM ( SELECT `v1`.`person_id` AS `PHS_PHPerson_person_id` FROM `PHS`.`person` AS `v1` WHERE EXISTS( ( SELECT * FROM `system`.`PHPerson_NodeMagic` WHERE `v1`.`person_id` = `system`.`PHPerson_NodeMagic`.`person_id` AND `PHPerson_NodeMagic`.`nodeHash` = @p2 ) ) ) AS `v0`;",
                insertCommand.CommandText);
        }

        [Fact]
        public void SelectIntersectWrapped()
        {
            var statement =
                Sql.Select.Output(Sql.Star("First"))
                    .From("sys.objects", "First")
                    .Intersect(Sql.Select.Output(Sql.Star("Second")).From("sys.objects", "Second"))
                    .WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT [wrap].* FROM ( SELECT [First].* FROM [sys].[objects] AS [First] INTERSECT SELECT [Second].* FROM [sys].[objects] AS [Second] ) AS [wrap];",
                command.CommandText);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT * FROM [sys].[objects] WHERE [name] LIKE N'foo' + N'%' AND [name] LIKE N'%' + N'foo' AND [name] LIKE N'%' + N'foo' + N'%' AND [name] LIKE N'%foo%';",
                command.CommandText);
        }

        [Fact]
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

            Assert.NotNull(command);
            Assert.Equal(
                "SELECT * FROM [sys].[objects] WHERE [object_id] IS NOT NULL AND [object_id] IS NULL AND ( [object_id] + 1 - 1 * 1 / 1 % 1 & 1 | 1 ^ 1 ) < 1;",
                command.CommandText);
        }

        [Fact]
        public void BeginCommitTransactions()
        {
            var statement = Sql.Statements(Sql.BeginTransaction(), Sql.CommitTransaction());

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION;\r\nCOMMIT TRANSACTION;", command.CommandText);
        }

        [Fact]
        public void BeginRollbackTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION;\r\nSAVE TRANSACTION;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;",
                command.CommandText);
        }

        [Fact]
        public void BeginRollbackMarkedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("foo"), "marked"),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "BEGIN TRANSACTION [foo] WITH MARK N'marked';\r\nSAVE TRANSACTION;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;",
                command.CommandText);
        }

        [Fact]
        public void BeginRollbackNamedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("t")),
                Sql.SaveTransaction(Sql.Name("s")),
                Sql.RollbackTransaction(Sql.Name("s")),
                Sql.CommitTransaction(Sql.Name("t")));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "BEGIN TRANSACTION [t];\r\nSAVE TRANSACTION [s];\r\nROLLBACK TRANSACTION [s];\r\nCOMMIT TRANSACTION [t];",
                command.CommandText);
        }

        [Fact]
        public void BeginRollbackParameterTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Parameter.Any("@t")),
                Sql.SaveTransaction(Parameter.Any("@s")),
                Sql.RollbackTransaction(Parameter.Any("@s")),
                Sql.CommitTransaction(Parameter.Any("@t")));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "BEGIN TRANSACTION @t;\r\nSAVE TRANSACTION @s;\r\nROLLBACK TRANSACTION @s;\r\nCOMMIT TRANSACTION @t;",
                command.CommandText);
        }

        [Fact]
        public void DeclareSelectNameFromSysObjects()
        {
            var statement = Sql.Declare(Parameter.NVarChar("@name"),
                Sql.Select.Top(1).Output(Sql.Name("name")).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DECLARE @name NVARCHAR ( MAX ) = ( SELECT TOP ( 1 ) [name] FROM [sys].[objects] );",
                command.CommandText);
        }

        [Fact]
        public void IfThenElse()
        {
            var statement = Sql.If(Sql.Function("EXISTS",
                Sql.Select.From("tempdb.sys.tables").Where(Sql.Name("name").IsEqual(Parameter.NVarChar("@name")))))
                .Then(Sql.Select.Output(Sql.Scalar(1)))
                .Else(Sql.Select.Output(Sql.Scalar(2)))
                ;

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "IF EXISTS( ( SELECT * FROM [tempdb].[sys].[tables] WHERE [name] = @name ) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;\r\nELSE\r\nBEGIN;\r\nSELECT 2;\r\nEND;",
                command.CommandText);
        }

        [Fact]
        public void DropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("DROP TABLE [some].[table];", command.CommandText);
        }

        [Fact]
        public void CommentOutDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).CommentOut();

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("/* DROP TABLE [some].[table] */", command.CommandText);
        }

        [Fact]
        public void CommentOutName()
        {
            //AssertSqlToken(Sql.Name("some.table").CommentOut(), "/* [some].[table] */");
        }

        [Fact]
        public void StringifyDropTable()
        {
            var statement = Sql.DropTable(Sql.Name("some.table")).Stringify();

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("N' DROP TABLE [some].[table]';", command.CommandText);
        }

        [Fact]
        public void StringifyName()
        {
            //AssertSqlToken(Sql.Name("some.table").Stringify(), "N' [some].[table]'");
        }

        [Fact]
        public void DropTableExists()
        {
            var statement = Sql.DropTable(Sql.Name("some.table"), true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF OBJECT_ID ( N'[some].[table]', N'U' ) IS NOT NULL DROP TABLE [some].[table];",
                command.CommandText);
        }

        [Fact]
        public void IfExists()
        {
            var statement = Sql.If(Sql.Exists(Sql.Select.From("sys.objects"))).Then(Sql.Select.Output(Sql.Scalar(1)));


            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF EXISTS ( SELECT * FROM [sys].[objects] )\r\nBEGIN;\r\nSELECT 1;\r\nEND;",
                command.CommandText);
        }

        [Fact]
        public void IfNotExists()
        {
            var statement =
                Sql.If(Sql.Exists(Sql.Select.From("sys.objects")).Not()).Then(Sql.Select.Output(Sql.Scalar(1)));


            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF NOT ( EXISTS ( SELECT * FROM [sys].[objects] ) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;",
                command.CommandText);
        }

        [Fact]
        public void IfNotExists2()
        {
            var statement = Sql.If(Sql.NotExists(Sql.Select.From("sys.objects"))).Then(Sql.Select.Output(Sql.Scalar(1)));


            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF NOT ( EXISTS ( SELECT * FROM [sys].[objects] ) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;",
                command.CommandText);
        }

        [Fact]
        public void IfNotExists3()
        {
            var statement =
                Sql.If(Sql.Not(Sql.Exists(Sql.Select.From("sys.objects")))).Then(Sql.Select.Output(Sql.Scalar(1)));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF NOT ( EXISTS ( SELECT * FROM [sys].[objects] ) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;",
                command.CommandText);
        }

        [Fact]
        public void IfSome()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Select.Output(Sql.Scalar(1)));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nSELECT 1;\r\nEND;", command.CommandText);
        }

        [Fact]
        public void IfBreak()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Break);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nBREAK;\r\nEND;", command.CommandText);
        }

        [Fact]
        public void IfThrow()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Throw(123, "123", 1));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nTHROW 123, N'123', 1;\r\nEND;",
                command.CommandText);
        }

        [Fact]
        public void IfThrow2()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Throw(Sql.Scalar(123), Sql.Name("@fooMsg"), Sql.Scalar(1)));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nTHROW 123, @fooMsg, 1;\r\nEND;",
                command.CommandText);
        }

        [Fact]
        public void IfContinue()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.Some(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Continue);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF 3 < ANY ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nCONTINUE;\r\nEND;", command.CommandText);
        }

        [Fact]
        public void IfAll()
        {
            var statement =
                Sql.If(Sql.Scalar(3).Less(Sql.All(Sql.Select.From("foo").Output(Sql.Name("a")))))
                    .Then(Sql.Select.Output(Sql.Scalar(1)));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("IF 3 < ALL ( SELECT [a] FROM [foo] )\r\nBEGIN;\r\nSELECT 1;\r\nEND;", command.CommandText);
        }

        [Fact]
        public void SelectOperationEq()
        {
            var statement = Sql.Select.Output(Sql.Name("foo").PlusEqual(Sql.Scalar(1))).From("sys.objects");
            var command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal("SELECT [foo] += 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").MinusEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal("SELECT [foo] -= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").MultiplyEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal("SELECT [foo] *= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").DivideEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal("SELECT [foo] /= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").ModuloEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal("SELECT [foo] %= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").BitwiseAndEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal("SELECT [foo] &= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").BitwiseOrEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal("SELECT [foo] |= 1 FROM [sys].[objects];", command.CommandText);
            statement = Sql.Select.Output(Sql.Name("foo").BitwiseXorEqual(Sql.Scalar(1))).From("sys.objects");
            command = Utilities.GetCommand(statement);
            Assert.NotNull(command);
            Assert.Equal("SELECT [foo] ^= 1 FROM [sys].[objects];", command.CommandText);
        }

        [Fact]
        public void GotoLabel()
        {
            var statement =
                Sql.Statements(
                    Sql.Goto("foo"),
                    Sql.Select.Output(Sql.Scalar(1)),
                    Sql.Label("foo"),
                    Sql.Select.Output(Sql.Scalar(2)));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("GOTO foo;\r\nSELECT 1;\r\nfoo:\r\nSELECT 2;", command.CommandText);
        }

        [Fact]
        public void Return()
        {
            var statement =
                Sql.Return();

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("RETURN;", command.CommandText);
        }

        [Fact]
        public void Return2()
        {
            var statement =
                Sql.Return(2);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("RETURN 2;", command.CommandText);
        }

        [Fact]
        public void ReturnVar()
        {
            var statement =
                Sql.Return(Sql.Name("@var"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("RETURN @var;", command.CommandText);
        }

        [Fact]
        public void TryCatch()
        {
            var statement =
                Sql.Try(Sql.Select.Output(Sql.Scalar(1))).Catch(Sql.Throw());

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRY\r\nSELECT 1;\r\nEND TRY\r\nBEGIN CATCH\r\nTHROW;\r\nEND CATCH;",
                command.CommandText);
        }

        [Fact]
        public void While()
        {
            var statement =
                Sql.While(Sql.Name("@i").Less(Sql.Scalar(10))).Do(Sql.Set("@i", Sql.Name("@i").Plus(Sql.Scalar(1))));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("WHILE @i < 10\r\nBEGIN;\r\nSET @i = @i + 1;\r\nEND;", command.CommandText);
        }

        [Fact]
        public void Snippet01()
        {
            var statement = Sql.SnippetStatement("SELECT * FROM sys.objects");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM sys.objects;", command.CommandText);
        }

        [Fact]
        public void Snippet02()
        {
            var statement = Sql.TemplateStatement("SELECT {0},{0} FROM sys.objects", Sql.Name("foo"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT  [foo], [foo]  FROM sys.objects;", command.CommandText);
        }

        [Fact]
        public void SelectParameters()
        {
            AssertSql(
                Sql.Select.Output(Sql.Parameter.Int("@foo")),
                "SELECT @foo;"
                );
        }

        [Fact]
        public void SelectIntoParameters()
        {
            AssertSql(
                Sql.Select.From(Sql.Name("foo")).Set(Sql.Parameter.Int("@foo"), Sql.Name("bar")).Top(1),
                "SELECT TOP ( 1 ) @foo = [bar] FROM [foo];"
                );
        }

        [Fact]
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

        [Fact]
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

        [Fact]
        public void Cast()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.Scalar(123).CastAs(CommonDbType.Int).As("foo"))
                    ,
                "SELECT CAST ( 123 AS INT ) AS [foo];");
        }
    }
}