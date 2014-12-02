using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace FluidSqlTests
{
    [TestClass]
    public class SelectTest
    {
        [TestMethod]
        public void Select1()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1));

            var command = Utilities.GetCommand(statement);
            
            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT 1;", command.CommandText);
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
            var statement = Sql.Select.Output(Sql.Function("COUNT",Sql.Star())).From("sys.objects");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT  COUNT(*) FROM [sys].[objects];", command.CommandText);
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
            Assert.AreEqual("SELECT TOP (1) 1 AS [foo];", command.CommandText);
        }

        [TestMethod]
        public void Select1Top1Param()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).As("foo")).Top(1);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT TOP (1) 1 AS [foo];", command.CommandText);
        }

        [TestMethod]
        public void Select1Top1Percent()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT TOP (1) PERCENT 1;", command.CommandText);
        }

        [TestMethod]
        public void Select1Top1PercentWithTies()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, true, true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT TOP (1) PERCENT WITH TIES 1;", command.CommandText);
        }

        [TestMethod]
        public void Select1Top1WithTies()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1)).Top(1, false, true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT TOP (1) WITH TIES 1;", command.CommandText);
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
            var statement = Sql.Select.Output(Sql.Name("src", "object_id")).From("sys.objects", "src").GroupBy(Sql.Name("src", "object_id"), Sql.Name("src", "name"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [src].[object_id] FROM [sys].[objects] AS [src] GROUP BY [src].[object_id], [src].[name];", command.CommandText);
        }

        [TestMethod]
        public void SelectOrderBy()
        {
            var statement = Sql.Select.Output(Sql.Name("src.object_id")).From("sys.objects", "src").OrderBy(Sql.Name("src", "object_id"), Direction.Asc).OrderBy(Sql.Name("src", "name"), Direction.Desc);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [src].[object_id] FROM [sys].[objects] AS [src] ORDER BY [src].[object_id] ASC, [src].[name] DESC;", command.CommandText);
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
            var statement = Sql.Select.Output(Sql.Star()).From("sys.objects").Union(Sql.Select.Output(Sql.Star()).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] UNION SELECT * FROM [sys].[objects];", command.CommandText);
        }

        [TestMethod]
        public void SelectUnionSelectWrapped()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("sys.objects").Union(Sql.Select.Output(Sql.Star()).From("sys.objects")).WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [wrap].* FROM (SELECT * FROM [sys].[objects] UNION SELECT * FROM [sys].[objects]) AS [wrap];", command.CommandText);
        }

        [TestMethod]
        public void SelectWrapped2()
        {
            var statement = Sql.Select.Output(Sql.Name("Name")).From("sys.objects").WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [wrap].[Name] FROM (SELECT [Name] FROM [sys].[objects]) AS [wrap];", command.CommandText);
        }
        
        [TestMethod]
        public void SelectWrapped3()
        {
            var statement = Sql.Select.Output(Sql.Name("Name").As("nm")).From("sys.objects").WrapAsSelect("wrap");

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [wrap].[nm] FROM (SELECT [Name] AS [nm] FROM [sys].[objects]) AS [wrap];", command.CommandText);
        }       
        [TestMethod]
        public void SelectUnionAllSelect()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("sys.objects").UnionAll(Sql.Select.Output(Sql.Star()).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] UNION ALL SELECT * FROM [sys].[objects];", command.CommandText);
        }
        [TestMethod]
        public void SelectExceptSelect()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("sys.objects").Except(Sql.Select.Output(Sql.Star()).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] EXCEPT SELECT * FROM [sys].[objects];", command.CommandText);
        }
        [TestMethod]
        public void SelectIntersectSelect()
        {
            var statement = Sql.Select.Output(Sql.Star("First")).From("sys.objects","First").Intersect(Sql.Select.Output(Sql.Star("Second")).From("sys.objects", "Second"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT [First].* FROM [sys].[objects] AS [First] INTERSECT SELECT [Second].* FROM [sys].[objects] AS [Second];", command.CommandText);
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
            Assert.AreEqual("SELECT [wrap].* FROM (SELECT [First].* FROM [sys].[objects] AS [First] INTERSECT SELECT [Second].* FROM [sys].[objects] AS [Second]) AS [wrap];", command.CommandText);
        }

        [TestMethod]
        public void SelectInnerJoin()
        {
            var statement = Sql.Select

                .Output(Sql.Star())
                .From("sys.objects", "o")
                .InnerJoin(Sql.Name("sys.tables").As("t"), Sql.Name("t.object_id").IsEqual(Sql.Name("o.object_id")));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] AS [o] INNER JOIN [sys].[tables] AS [t] ON [t].[object_id] = [o].[object_id];", command.CommandText);
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
                            Sql.Name("name").Contains(Sql.Scalar("foo"))));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] WHERE [name] LIKE N'foo' + '%' AND [name] LIKE '%' + N'foo' AND [name] LIKE '%' + N'foo' + '%';", command.CommandText);
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
                            .Module(Sql.Scalar(1))
                        ).Less(Sql.Scalar(1)))));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("SELECT * FROM [sys].[objects] WHERE [object_id] IS NOT NULL AND [object_id] IS NULL AND  ([object_id] + 1 - 1 * 1 / 1 % 1 ) < 1;", command.CommandText);
        }

        [TestMethod]
        public void BeginCommitTransactions()
        {
            var statement = Sql.Statements(Sql.BeginTransaction(), Sql.CommitTransaction());

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION;\r\nCOMMIT TRANSACTION;\r\n", command.CommandText);
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
            Assert.AreEqual("BEGIN TRANSACTION;\r\nSAVE TRANSACTION;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;\r\n", command.CommandText);

        }
        [TestMethod]
        public void BeginRollbackMarkedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("foo"),"marked"),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION [foo] WITH MARK 'marked';\r\nSAVE TRANSACTION;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;\r\n", command.CommandText);

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
            Assert.AreEqual("BEGIN TRANSACTION [t];\r\nSAVE TRANSACTION [s];\r\nROLLBACK TRANSACTION [s];\r\nCOMMIT TRANSACTION [t];\r\n", command.CommandText);

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
            Assert.AreEqual("BEGIN TRANSACTION @t;\r\nSAVE TRANSACTION @s;\r\nROLLBACK TRANSACTION @s;\r\nCOMMIT TRANSACTION @t;\r\n", command.CommandText);

        }

        [TestMethod]
        public void DeclareSelectNameFromSysObjects()
        {
            var statement = Sql.Declare(Parameter.NVarChar("@name"), Sql.Select.Top(1).Output(Sql.Name("name")).From("sys.objects"));

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DECLARE @name NVARCHAR(MAX) = (SELECT TOP (1) [name] FROM [sys].[objects]);", command.CommandText);
        }

        [TestMethod]
        public void IfThenElse()
        {
            var statement = Sql.If(Sql.Function("EXISTS", Sql.Select.From("tempdb.sys.tables").Where(Sql.Name("name").IsEqual(Parameter.NVarChar("@name")))))
                .Then(Sql.Select.Output(Sql.Scalar(1)))
                .Else(Sql.Select.Output(Sql.Scalar(2)))
                ;

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF  EXISTS((SELECT * FROM [tempdb].[sys].[tables] WHERE [name] = @name))\r\nBEGIN;\r\nSELECT 1;\r\nEND;\r\nELSE\r\nBEGIN;\r\nSELECT 2;\r\nEND;\r\n", command.CommandText);
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
            Assert.AreEqual(" /* DROP TABLE [some].[table]; */ ", command.CommandText);
        }

        [TestMethod]
        public void DropTableExists()
        {
            var statement = Sql.DropTable(Sql.Name("some.table"), true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF OBJECT_ID(N'[some].[table]',N'U') IS NOT NULL DROP TABLE [some].[table];", command.CommandText);
        }

        [TestMethod]
        public void IfExists()
        {
            var statement = Sql.If(Sql.Exists(Sql.Select.From("sys.objects"))).Then(Sql.Select.Output(Sql.Scalar(1)));
            

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF  EXISTS ((SELECT * FROM [sys].[objects]) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;\r\n", command.CommandText);
        }

        [TestMethod]
        public void IfNotExists()
        {
            var statement = Sql.If(Sql.Exists(Sql.Select.From("sys.objects")).Not()).Then(Sql.Select.Output(Sql.Scalar(1)));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF  NOT EXISTS ((SELECT * FROM [sys].[objects]) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;\r\n", command.CommandText);
        }

        [TestMethod]
        public void IfNotExists2()
        {
            var statement = Sql.If(Sql.NotExists(Sql.Select.From("sys.objects"))).Then(Sql.Select.Output(Sql.Scalar(1)));


            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF  NOT EXISTS ((SELECT * FROM [sys].[objects]) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;\r\n", command.CommandText);
        }

        [TestMethod]
        public void IfNotExists3()
        {
            var statement = Sql.If(Sql.Not(Sql.Exists(Sql.Select.From("sys.objects")))).Then(Sql.Select.Output(Sql.Scalar(1)));
            
            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("IF  NOT EXISTS ((SELECT * FROM [sys].[objects]) )\r\nBEGIN;\r\nSELECT 1;\r\nEND;\r\n", command.CommandText);
        }
        //[TestMethod]
        //public void CreateTable()
        //{
        //    var statement = Sql.CreateTable(Sql.Name("some.table"));

        //    var command = Utilities.GetCommand(statement);

        //    Assert.IsNotNull(command);
        //    Assert.AreEqual("CREATE TABLE [some].[table] ();", command.CommandText);
        //}


    }
}


