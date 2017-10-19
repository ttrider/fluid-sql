// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using System;
using System.Data;
using TTRider.FluidSql;
using Xunit;

namespace xUnit.FluidSql
{
    public class ProcedureAndFunctions
    {
        [Fact]
        public void CreateProcedure()
        {
            var statement = Sql.CreateProcedure("proc00", true)
                .Parameters(Parameter.Int("@i01"),
                    Parameter.DateTime("@sd02").DefaultValue(DateTime.Parse("2016-01-01")))
                .InputOutputParameters(Parameter.Int("@i03"))
                .OutputParameters(Parameter.String("@s04"), Parameter.Bit("@b05"))
                .ReturnValue(Parameter.Int("@retVal"))
                .As(
                    Sql.Set("@b05", Sql.Scalar(1)),
                    Sql.Set("@s04", Sql.Scalar("outputString")),
                    Sql.Set("@i03", Sql.Name("@i01")),
                    Sql.Return(123)
                );

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"IF OBJECT_ID ( N'[proc00]', N'P' ) IS NULL
BEGIN;
EXEC (N' CREATE PROCEDURE [proc00]
(
@i01 INT,
@sd02 DATETIME = N''2016-01-01T00:00:00'',
@i03 INT OUTPUT,
@s04 NVARCHAR ( MAX ) OUTPUT,
@b05 BIT OUTPUT
)
AS
BEGIN;
SET @b05 = 1;
SET @s04 = N''outputString'';
SET @i03 = @i01;
RETURN 123;
END;
' );
END;", command.CommandText);
        }

        [Fact]
        public void AlterProcedure()
        {
            var statement = Sql.AlterProcedure("proc00", true)
                .Parameters(Parameter.Int("@i01"),
                    Parameter.DateTime("@sd02").DefaultValue(DateTime.Parse("2016-01-01")))
                .InputOutputParameters(Parameter.Int("@i03"))
                .OutputParameters(Parameter.String("@s04"), Parameter.Bit("@b05"))
                .ReturnValue(Parameter.Int("@retVal"))
                .As(
                    Sql.Set("@b05", Sql.Scalar(1)),
                    Sql.Set("@s04", Sql.Scalar("outputString")),
                    Sql.Set("@i03", Sql.Name("@i01")),
                    Sql.Return(123)
                );

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"IF OBJECT_ID ( N'[proc00]', N'P' ) IS NULL
BEGIN;
EXEC (N' CREATE PROCEDURE [proc00]
(
@i01 INT,
@sd02 DATETIME = N''2016-01-01T00:00:00'',
@i03 INT OUTPUT,
@s04 NVARCHAR ( MAX ) OUTPUT,
@b05 BIT OUTPUT
)
AS
BEGIN;
SET @b05 = 1;
SET @s04 = N''outputString'';
SET @i03 = @i01;
RETURN 123;
END;
' );
END;
ELSE BEGIN;
EXEC (N' ALTER PROCEDURE [proc00]
(
@i01 INT,
@sd02 DATETIME = N''2016-01-01T00:00:00'',
@i03 INT OUTPUT,
@s04 NVARCHAR ( MAX ) OUTPUT,
@b05 BIT OUTPUT
)
AS
BEGIN;
SET @b05 = 1;
SET @s04 = N''outputString'';
SET @i03 = @i01;
RETURN 123;
END;
' );
END;", command.CommandText);
        }

        [Fact]
        public void DropProcedure()
        {
            var statement = Sql.DropProcedure("proc00", true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"IF OBJECT_ID ( N'[proc00]', N'P' ) IS NOT NULL
BEGIN;
EXEC (N' DROP PROCEDURE [proc00]' );
END;", command.CommandText);
        }

        [Fact]
        public void ExecuteProcedure()
        {
            var statement = Sql.ExecuteStoredProcedure("proc00",
                Parameter.Int("@i01").DefaultValue(123),
                Parameter.DateTime("@sd02").DefaultValue(DateTime.Parse("2016-01-01")),
                Parameter.Int("@retVal").ParameterDirection(ParameterDirection.ReturnValue)
                );

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"EXEC @retVal = [proc00] @i01 = @i01, @sd02 = @sd02;", command.CommandText);
        }

        [Fact]
        public void CreateFunction()
        {
            var statement = Sql.CreateFunction("func00", true)
                .As(
                    Sql.Select.Output(Sql.Star()).From("target_tbl"),
                    Sql.Select.Output(Sql.Star()).From("source_tbl")
                );

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"IF OBJECT_ID ( N'[func00]', N'FN' ) IS NULL
BEGIN;
EXEC (N' CREATE FUNCTION [func00]
AS
BEGIN;
SELECT * FROM [target_tbl];
SELECT * FROM [source_tbl];
END;
' );
END;", command.CommandText);
        }

        [Fact]
        public void CreateFunctionWithInputParam()
        {
            var statement = Sql.CreateFunction("func00", true)
                .Parameters(Parameter.Int("@i01"),
                    Parameter.DateTime("@sd02"))
                /*.InputOutputParameters(Parameter.Int("@i03"))
                .OutputParameters(Parameter.String("@s04"), Parameter.Bit("@b05"))
                .ReturnValue(Parameter.Int("@retVal"))*/
                .As(
                    Sql.Select.Output(Sql.Star()).From("target_tbl"),
                    Sql.Select.Output(Sql.Star()).From("source_tbl")
                );

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"IF OBJECT_ID ( N'[func00]', N'FN' ) IS NULL
BEGIN;
EXEC (N' CREATE FUNCTION [func00]
(
@i01 INT,
@sd02 DATETIME
)
AS
BEGIN;
SELECT * FROM [target_tbl];
SELECT * FROM [source_tbl];
END;
' );
END;", command.CommandText);
        }

        [Fact]
        public void CreateFunctionWithReturnsParam()
        {
            var statement = Sql.CreateFunction("func00", true)
                .Parameters(Parameter.Int("@i01"),
                    Parameter.DateTime("@sd02"))
                .ReturnValue(Parameter.Int())
                .As(
                    Sql.Select.Output(Sql.Star()).From("target_tbl"),
                    Sql.Select.Output(Sql.Star()).From("source_tbl")
                );

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"IF OBJECT_ID ( N'[func00]', N'FN' ) IS NULL
BEGIN;
EXEC (N' CREATE FUNCTION [func00]
(
@i01 INT,
@sd02 DATETIME
)
RETURNS  INT
AS
BEGIN;
SELECT * FROM [target_tbl];
SELECT * FROM [source_tbl];
END;
' );
END;", command.CommandText);
        }

        [Fact]
        public void AlterFunction()
        {
            var statement = Sql.AlterFunction("func00", true)
                .Parameters(Parameter.Int("@i01"),
                    Parameter.DateTime("@sd02"))
                .ReturnValue(Parameter.Int())
                .As(
                    Sql.Select.Output(Sql.Star()).From("target_tbl"),
                    Sql.Select.Output(Sql.Star()).From("source_tbl")
                );

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"IF OBJECT_ID ( N'[func00]', N'FN' ) IS NULL
BEGIN;
EXEC (N' CREATE PROCEDURE [func00]
(
@i01 INT,
@sd02 DATETIME
)
AS
BEGIN;
SELECT * FROM [target_tbl];
SELECT * FROM [source_tbl];
END;
' );
END;
ELSE BEGIN;
EXEC (N' ALTER PROCEDURE [func00]
(
@i01 INT,
@sd02 DATETIME
)
AS
BEGIN;
SELECT * FROM [target_tbl];
SELECT * FROM [source_tbl];
END;
' );
END;", command.CommandText);
        }

        [Fact]
        public void DropFunction()
        {
            var statement = Sql.DropFunction("func00", true);

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"IF OBJECT_ID ( N'[func00]', N'FN' ) IS NOT NULL
BEGIN;
EXEC (N' DROP FUNCTION [func00]' );
END;", command.CommandText);
        }

        [Fact]
        public void ExecuteFunction()
        {
            var statement = Sql.ExecuteFunction("proc00",
                Parameter.Int("@i01").Value(123),
                Parameter.DateTime("@sd02"),
                Parameter.Int("@retVal").ParameterDirection(ParameterDirection.ReturnValue)
                );

            var command = Utilities.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal(@"EXEC @retVal = [proc00] ( 123, @sd02 );", command.CommandText);
        }
    }
}