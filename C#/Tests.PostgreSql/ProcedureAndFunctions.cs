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

        [TestMethod]
        public void PrepareStatement1()
        {
            AssertSql(Sql.Prepare().Name(Sql.Name("tempStr")).From(Sql.Select.Output(Sql.Now())), "PREPARE \"tempStr\" AS SELECT NOW ( );");
            AssertSql(Sql.Prepare(Sql.Name("tempStr"), Sql.Select.Output(Sql.Now())), "PREPARE \"tempStr\" AS SELECT NOW ( );");
        }

        [TestMethod]
        public void PrepareStatement2()
        {
            //PREPARE fooplan (int, text, bool, numeric) AS
            //INSERT INTO foo VALUES($1, $2, $3, $4);
            var statement = Sql.Prepare(Sql.Name("target_prepare"),
                Sql.Insert.Into(Sql.Name("target_tbl")).Values("$1", "$2"), 
                Parameter.Int("$1"), 
                Parameter.Text("$2"));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("PREPARE \"target_prepare\" (  INTEGER,  TEXT ) AS INSERT INTO \"target_tbl\" VALUES ( $1, $2 );", command.CommandText);
        }

        [TestMethod]
        public void ExecuteFromVariable()
        {
            var statement = Sql.Execute(Sql.Name("temp_statement"));
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("EXECUTE \"temp_statement\";", command.CommandText);
        }

        [TestMethod]
        public void ExecuteFromVariableWithParam()
        {
            var statement = Sql.Execute(Sql.Name("temp_statement"), Parameter.Any("@a1"), Parameter.Any("@a2"));
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("EXECUTE \"temp_statement\" ( @a1, @a2 );", command.CommandText);
        }

        [TestMethod]
        public void ExecuteFromStatement()
        {
            var statement = Sql.Execute(
                Sql.Insert.Into(Sql.Name("target_tbl")).Values("$1", "$2"),
                Parameter.Int("$1").Value(1000),
                Parameter.Text("$2").Value("Test1000"));
            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("PREPARE \"temp_execute\" (  INTEGER,  TEXT ) AS INSERT INTO \"target_tbl\" VALUES ( $1, $2 );\r\nEXECUTE \"temp_execute\" ( 1000, 'Test1000' );\r\nDEALLOCATE \"temp_execute\";", command.CommandText);
        }
    }
}
