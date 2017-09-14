// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using TTRider.FluidSql;
using Xunit;

namespace xUnit.FluidSql
{
    public class Insert : SqlProviderTests
    {
        [Fact]
        public void InsertDefault()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).DefaultValues();

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("INSERT INTO [foo].[bar] DEFAULT VALUES;", command.CommandText);
        }

        [Fact]
        public void InsertFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).From(Sql.Select.From(Sql.Name("bar.foo")));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("INSERT INTO [foo].[bar] SELECT * FROM [bar].[foo];", command.CommandText);
        }

        [Fact]
        public void InsertFrom2()
        {
            var statement = Sql.Insert.Into("foo.bar").From(Sql.Select.From("bar.foo"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("INSERT INTO [foo].[bar] SELECT * FROM [bar].[foo];", command.CommandText);
        }

        [Fact]
        public void InsertTopFrom()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar")).Top(1).From(Sql.Select.From(Sql.Name("bar.foo")));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("INSERT TOP ( 1 ) INTO [foo].[bar] SELECT * FROM [bar].[foo];", command.CommandText);
        }

        [Fact]
        public void InsertColumnsFrom00()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .From(Sql.Select.From(Sql.Name("bar.foo")))
                .Columns("id", "value");

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("INSERT INTO [foo].[bar] ( [id], [value] ) SELECT * FROM [bar].[foo];", command.CommandText);
        }

        [Fact]
        public void InsertColumnsFrom01()
        {
            var statement =
                Sql.Insert.Into(Sql.Name("foo.bar"))
                    .From(Sql.Select.From(Sql.Name("bar.foo")))
                    .Columns(Sql.Name("id"), Sql.Name("value"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("INSERT INTO [foo].[bar] ( [id], [value] ) SELECT * FROM [bar].[foo];", command.CommandText);
        }

        [Fact]
        public void InsertColumnsFromOutput()
        {
            AssertSql(
                Sql.Insert.Into(Sql.Name("foo.bar"))
                    .From(Sql.Select.From(Sql.Name("bar.foo")))
                    .Columns(Sql.Name("id"), Sql.Name("value"))
                    .Output(Sql.Name("inserted", "id")),
                "INSERT INTO [foo].[bar] ( [id], [value] ) OUTPUT [inserted].[id] SELECT * FROM [bar].[foo];");

            AssertSql(
                Sql.Insert.Into(Sql.Name("foo.bar"))
                    .From(Sql.Select.From(Sql.Name("bar.foo")))
                    .Columns(new List<Name> { Sql.Name("id"), Sql.Name("value") })
                    .Output(Sql.Name("inserted", "id")),
                "INSERT INTO [foo].[bar] ( [id], [value] ) OUTPUT [inserted].[id] SELECT * FROM [bar].[foo];");

            AssertSql(
                Sql.Insert.Into(Sql.Name("foo.bar"))
                    .From(Sql.Select.From(Sql.Name("bar.foo")))
                    .Columns(new List<string> { "id", "value" })
                    .Output(Sql.Name("inserted", "id")),
                "INSERT INTO [foo].[bar] ( [id], [value] ) OUTPUT [inserted].[id] SELECT * FROM [bar].[foo];");

            AssertSql(
                Sql.Insert.Into(Sql.Name("foo.bar"))
                    .From(Sql.Select.From(Sql.Name("bar.foo")))
                    .Columns("id", "value")
                    .Output(Sql.Name("inserted", "id")),
                "INSERT INTO [foo].[bar] ( [id], [value] ) OUTPUT [inserted].[id] SELECT * FROM [bar].[foo];");
        }

        [Fact]
        public void InsertColumnsFromOutputInto()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .From(Sql.Select.From(Sql.Name("bar.foo")))
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .OutputInto(Sql.Name("@t"), Sql.Name("inserted", "id"));

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "INSERT INTO [foo].[bar] ( [id], [value] ) OUTPUT [inserted].[id] INTO @t SELECT * FROM [bar].[foo];",
                command.CommandText);
        }

        [Fact]
        public void InsertColumnsValues00()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .Values(Sql.Scalar(123), Sql.Scalar("val0"))
                .Values(234, "val1")
                .Values(new List<Scalar> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { 345, "val2" });

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "INSERT INTO [foo].[bar] ( [id], [value] ) VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, N'val2' ), ( 345, N'val2' ), ( 345, N'val2' );",
                command.CommandText);
        }

        [Fact]
        public void InsertIdentityColumnsValues()
        {
            var statement = Sql.Insert.Into(Sql.Name("foo.bar"))
                .IdentityInsert()
                .Columns(Sql.Name("id"), Sql.Name("value"))
                .Values(Sql.Scalar(123), Sql.Scalar("val0"))
                .Values(234, "val1")
                .Values(new List<Scalar> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { Sql.Scalar(345), Sql.Scalar("val2") })
                .Values(new List<object> { 345, "val2" });

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(
                "SET IDENTITY_INSERT [foo].[bar] ON;\r\nINSERT INTO [foo].[bar] ( [id], [value] ) VALUES ( 123, N'val0' ), ( 234, N'val1' ), ( 345, N'val2' ), ( 345, N'val2' ), ( 345, N'val2' );\r\nSET IDENTITY_INSERT [foo].[bar] OFF;",
                command.CommandText);
        }
    }
}