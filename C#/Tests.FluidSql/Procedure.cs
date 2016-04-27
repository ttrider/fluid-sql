using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace FluidSqlTests
{
    [TestClass]
    public class Procedure
    {
        [TestMethod]
        public void CreateProcedure()
        {
            var statement = Sql.CreateProcedure("proc00", true)
                .Parameters(Parameter.Int("@i01"), Parameter.DateTime("@sd02").DefaultValue(DateTime.Parse("2016-01-01")))
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

            Assert.IsNotNull(command);
            Assert.AreEqual(@"IF OBJECT_ID ( N'[proc00]', N'P' ) IS NULL
BEGIN;
EXEC (N' CREATE PROCEDURE [proc00]
(
@i01 INT OUTPUT,
@sd02 DATETIME = N''2016-01-01T00:00:00'' OUTPUT,
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

        [TestMethod]
        public void AlterProcedure()
        {
            var statement = Sql.AlterProcedure("proc00", true)
                .Parameters(Parameter.Int("@i01"), Parameter.DateTime("@sd02").DefaultValue(DateTime.Parse("2016-01-01")))
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

            Assert.IsNotNull(command);
            Assert.AreEqual(@"IF OBJECT_ID ( N'[proc00]', N'P' ) IS NULL
BEGIN;
EXEC (N' CREATE PROCEDURE [proc00]
(
@i01 INT OUTPUT,
@sd02 DATETIME = N''2016-01-01T00:00:00'' OUTPUT,
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
@i01 INT OUTPUT,
@sd02 DATETIME = N''2016-01-01T00:00:00'' OUTPUT,
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

        [TestMethod]
        public void DropProcedure()
        {
            var statement = Sql.DropProcedure("proc00", true);

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(@"IF OBJECT_ID ( N'[proc00]', N'P' ) IS NOT NULL
BEGIN;
EXEC (N' DROP PROCEDURE [proc00]' );
END;", command.CommandText);
        }

        [TestMethod]
        public void ExecuteProcedure()
        {
            var statement = Sql.ExecuteStoredProcedure("proc00",
                Parameter.Int("@i01").DefaultValue(123),
                Parameter.DateTime("@sd02").DefaultValue(DateTime.Parse("2016-01-01")),
                Parameter.Int("@retVal").ParameterDirection(ParameterDirection.ReturnValue)
                );

            var command = Utilities.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(@"EXEC @retVal = [proc00] @i01 = @i01, @sd02 = @sd02;", command.CommandText);
        }

    }
}
