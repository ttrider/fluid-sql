using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace Tests.PostgreSql
{
    [TestClass]
    public class ProcedureAndFunctions : PostgreSqlProviderTests
    {
        [TestMethod]
        public void CreateFunction()
        {
            var statement = Sql.CreateFunction("func00", true)
                .As(
                    Sql.Insert.Into("target_tbl").Values(Sql.Scalar(1), Sql.Scalar("function_test1"))
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE OR REPLACE FUNCTION \"func00\" ( )\r\nRETURNS VOID AS $do$\r\nBEGIN\r\nINSERT INTO \"target_tbl\" VALUES ( 1, 'function_test1' );\r\nEND;\r\n$do$ LANGUAGE plpgsql;", command.CommandText);
        }

        [TestMethod]
        public void CreateFunctionWithInputParam()
        {
            var statement = Sql.CreateFunction("func00", true)
                .Parameters(Parameter.Int("i01"), Parameter.Text("sd02"))
                .As(
                    Sql.Insert.Into("target_tbl").Values(Sql.Scalar(1), Sql.Scalar("function_test1"))
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE OR REPLACE FUNCTION \"func00\" (  \"i01\" INTEGER, \"sd02\" TEXT )\r\nRETURNS VOID AS $do$\r\nBEGIN\r\nINSERT INTO \"target_tbl\" VALUES ( 1, 'function_test1' );\r\nEND;\r\n$do$ LANGUAGE plpgsql;", command.CommandText);
        }

        [TestMethod]
        public void CreateFunctionWithReturnsParam()
        {
            var statement = Sql.CreateFunction("func01", true)
                .Parameters(Parameter.Int("i01"), Parameter.Text("sd02"))
                .ReturnValue(Parameter.Int("$ret_val$"))
                .As(
                    Sql.Insert.Into("target_tbl").Values(Sql.Scalar(1), Sql.Scalar("function_test1"))
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE OR REPLACE FUNCTION \"func01\" (  \"i01\" INTEGER, \"sd02\" TEXT )\r\nRETURNS INTEGER AS $ret_val$\r\nBEGIN\r\nINSERT INTO \"target_tbl\" VALUES ( 1, 'function_test1' );\r\nEND;\r\n$ret_val$ LANGUAGE plpgsql;", command.CommandText);
        }

        [TestMethod]
        public void CreateFunctionWithDeclarations()
        {
            var statement = Sql.CreateFunction("func01", true)
                .Parameters(Parameter.Int("i01"), Parameter.Text("sd02"))
                .Declarations(Parameter.Int("dec_val"))
                .ReturnValue(Parameter.Int("$ret_val$"))
                .As(
                    Sql.Insert.Into("target_tbl").Values(Sql.Scalar(1), Sql.Scalar("function_test1"))
                );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE OR REPLACE FUNCTION \"func01\" (  \"i01\" INTEGER, \"sd02\" TEXT )\r\nRETURNS INTEGER AS $ret_val$\r\nDECLARE\r\n\"dec_val\" INTEGER;\r\nBEGIN\r\nINSERT INTO \"target_tbl\" VALUES ( 1, 'function_test1' );\r\nEND;\r\n$ret_val$ LANGUAGE plpgsql;", command.CommandText);
        }

         [TestMethod]
         public void AlterFunction()
         {
            var statement = Sql.AlterFunction("func01", true)
              .Parameters(Parameter.Int("i01"), Parameter.Text("sd02"))
              .Declarations(Parameter.Int("dec_val"))
              .ReturnValue(Parameter.Int("$ret_val$"))
              .As(
                  Sql.Insert.Into("target_tbl").Values(Sql.Scalar(1), Sql.Scalar("function_test1"))
              );

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("CREATE OR REPLACE FUNCTION \"func01\" (  \"i01\" INTEGER, \"sd02\" TEXT )\r\nRETURNS INTEGER AS $ret_val$\r\nDECLARE\r\n\"dec_val\" INTEGER;\r\nBEGIN\r\nINSERT INTO \"target_tbl\" VALUES ( 1, 'function_test1' );\r\nEND;\r\n$ret_val$ LANGUAGE plpgsql;", command.CommandText);
        }

         [TestMethod]
         public void DropFunction()
         {
             var statement = Sql.DropFunction("func00", true).ReturnValue(Parameter.Int());

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("DROP FUNCTION IF EXISTS \"func00\" ( INTEGER );", command.CommandText);
         }

        [TestMethod]
        public void DropFunctionCascade()
        {
            var statement = Sql.DropFunction("func00", true).ReturnValue(Parameter.Int()).Cascade();

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP FUNCTION IF EXISTS \"func00\" ( INTEGER ) CASCADE;", command.CommandText);
        }

        [TestMethod]
        public void DropFunctionRestrict()
        {
            var statement = Sql.DropFunction("func00", true).ReturnValue(Parameter.Int()).Restrict();

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("DROP FUNCTION IF EXISTS \"func00\" ( INTEGER ) RESTRICT;", command.CommandText);
        }
         [TestMethod]
         public void ExecuteFunction()
         {
             var statement = Sql.ExecuteFunction("func00",
                 Parameter.Int("i01").Value(123),
                 Parameter.DateTime("sd02"),
                 Parameter.Int("retVal").ParameterDirection(ParameterDirection.ReturnValue)
                 );

             var command = Provider.GetCommand(statement);

             Assert.IsNotNull(command);
             Assert.AreEqual("retVal = func00 ( 123, sd02 );", command.CommandText);
         }
    }
}
