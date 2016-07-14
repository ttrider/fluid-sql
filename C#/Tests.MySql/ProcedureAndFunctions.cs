using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using TTRider.FluidSql;

namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for ProcedureAndFunctions
    /// </summary>
    [TestClass]
    public class ProcedureAndFunctions : MySqlProviderTests
    {
        [TestMethod]
        public void CreateProcedure()
        {
            var statement = Sql.CreateProcedure("proc00", true)
                .Parameters(Parameter.Text("columnValue"))
                .As(
                    Sql.Select.Output(Sql.Name("id")).From(Sql.Name("t1"))
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS `proc00`;\r\nDELIMITER $$\r\nCREATE PROCEDURE `proc00`\r\n(\r\nIN `columnValue` TEXT\r\n)\r\nBEGIN\r\nSELECT `id` FROM `t1`;\r\nEND  $$;\r\nDELIMITER ;", command.CommandText);
        }

        [TestMethod]
        public void CreateProcedureWithIn()
        {
            var statement = Sql.CreateProcedure("SelectWhereProc", true)
                .Parameters(Parameter.Int("idValue"))
                .As(
                    Sql.Select.Output(Sql.Name("id")).From(Sql.Name("t1")).Where(Sql.Name("id").IsEqual(Sql.Name("idValue")))
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS `SelectWhereProc`;\r\nDELIMITER $$\r\nCREATE PROCEDURE `SelectWhereProc`\r\n(\r\nIN `idValue` INTEGER\r\n)\r\nBEGIN\r\nSELECT `id` FROM `t1` WHERE `id` = `idValue`;\r\nEND  $$;\r\nDELIMITER ;", command.CommandText);
        }

        [TestMethod]
        public void CreateProcedureWithOut()
        {
            var statement = Sql.CreateProcedure("SelectWhereProc", true)
                .InputParameters(Parameter.Text("nameValue"))
                .OutputParameters(Parameter.Int("idCount"))
                .As(
                    Sql.Select.Output(Sql.Function("COUNT", Sql.Star())).From(Sql.Name("t1")).Into(Sql.Name("idCount")).Where(Sql.Name("name").IsEqual(Sql.Name("nameValue")))
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS `SelectWhereProc`;\r\nDELIMITER $$\r\nCREATE PROCEDURE `SelectWhereProc`\r\n(\r\nIN `nameValue` TEXT,\r\nOUT `idCount` INTEGER\r\n)\r\nBEGIN\r\nSELECT COUNT( * ) FROM `t1` WHERE `name` = `nameValue`;\r\nEND  $$;\r\nDELIMITER ;", command.CommandText);
        }

        [TestMethod]
        public void AlterProcedure()
        {
            var statement = Sql.AlterProcedure("proc00", true)
               .Parameters(Parameter.Text("columnValue"))
               .As(
                   Sql.Select.Output(Sql.Name("id")).From(Sql.Name("t1"))
               );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS `proc00`;\r\nDELIMITER $$\r\nCREATE PROCEDURE `proc00`\r\n(\r\nIN `columnValue` TEXT\r\n)\r\nBEGIN\r\nSELECT `id` FROM `t1`;\r\nEND  $$;\r\nDELIMITER ;", command.CommandText);
        }

        [TestMethod]
        public void DropProcedure()
        {
            var statement = Sql.DropProcedure("proc00", true);

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS `proc00`;", command.CommandText);
        }

        [TestMethod]
        public void ExecuteProcedure()
        {
            var statement = Sql.ExecuteStoredProcedure("proc00",
                Parameter.Int("@i01").DefaultValue(123));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CALL proc00 ( @i01 );", command.CommandText);
        }

        [TestMethod]
        public void ExecuteProcedureWithInParam()
        {
            var statement = Sql.ExecuteStoredProcedure("SelectWhereProc",
                Parameter.Int().Value(1));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CALL SelectWhereProc ( 1 );", command.CommandText);
        }

        [TestMethod]
        public void ExecuteProcedureWithOutParam()
        {
            var statement = Sql.ExecuteStoredProcedure("SelectWhereProc",
                Parameter.Text().Value("name1"), Parameter.Int("@total"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CALL SelectWhereProc ( N'name1', @total );", command.CommandText);
        }

        [TestMethod]
        public void CreateFunction()
        {
            var statement = Sql.CreateFunction("func00", true)
                .ReturnValue(Parameter.Int())
                .As(
                    Sql.Return(1)
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP FUNCTION IF EXISTS `func00`;\r\nDELIMITER $$\r\nCREATE FUNCTION `func00`\r\n( )\r\nRETURNS INTEGER\r\nBEGIN\r\nRETURN 1;\r\nEND  $$;\r\nDELIMITER ;", command.CommandText);
        }

       /* [TestMethod]
        public void CreateFunctionWithInputParam()
        {
            var statement = Sql.CreateFunction("func00", true)
                .Parameters(Parameter.Int("@i01"),
                    Parameter.DateTime("@sd02"))
                /*.InputOutputParameters(Parameter.Int("@i03"))
                .OutputParameters(Parameter.String("@s04"), Parameter.Bit("@b05"))
                .ReturnValue(Parameter.Int("@retVal"))*/
                /*.As(
                    Sql.Select.Output(Sql.Star()).From("target_tbl"),
                    Sql.Select.Output(Sql.Star()).From("source_tbl")
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(@"IF OBJECT_ID ( N'[func00]', N'FN' ) IS NULL
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

        [TestMethod]
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

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual(@"IF OBJECT_ID ( N'[func00]', N'FN' ) IS NULL
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
        }*/

        [TestMethod]
        public void AlterFunction()
        {
            var statement = Sql.AlterFunction("func00", true)
                .ReturnValue(Parameter.Int())
                .As(
                    Sql.Return(1)
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP FUNCTION IF EXISTS `func00`;\r\nDELIMITER $$\r\nCREATE FUNCTION `func00`\r\n( )\r\nRETURNS INTEGER\r\nBEGIN\r\nRETURN 1;\r\nEND  $$;\r\nDELIMITER ;", command.CommandText);
        }

        [TestMethod]
        public void DropFunction()
        {
            var statement = Sql.DropFunction("func00", true);

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP FUNCTION IF EXISTS `func00`;", command.CommandText);
        }

        [TestMethod]
        public void ExecuteFunction()
        {
            var statement = Sql.ExecuteFunction("func00",
                Parameter.Int("@i01").Value(123),
                Parameter.DateTime("@sd02"),
                Parameter.Int("@retVal").ParameterDirection(ParameterDirection.ReturnValue)
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("func00 ( 123, @sd02 );", command.CommandText);
        }
    }
}
