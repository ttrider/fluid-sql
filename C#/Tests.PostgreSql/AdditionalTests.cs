using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.PostgreSQL;

namespace Tests.PostgreSql
{
    /// <summary>
    /// Summary description for AdditionalTest
    /// </summary>
    [TestClass]
    public class AdditionalTests : PostgreSqlProviderTests
    {
        [TestMethod]
        public void DeclareStatement()
        {
            var statement = Sql.Declare(Parameter.Int("test_variable"));
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("DECLARE test_variable INTEGER;", text);
        }

        [TestMethod]
        public void DeclareNextStatement()
        {
            var statement = Sql.Declare(Parameter.Text("test_text"), null, false);
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("test_text TEXT;", text);
        }

        [TestMethod]
        public void SetStatement()
        {
            var statement = Sql.Set(Sql.Name("test_text"), Sql.Scalar("test_text"));
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("\"test_text\" := 'test_text';", text);
        }

        [TestMethod]
        public void BitwiseNotToken()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).BitwiseNot());
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT ~ 1;", text);
        }

        [TestMethod]
        public void UnaryMinusToken()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).Minus());
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT - 1;", text);
        }

        [TestMethod]
        public void NotToken()
        {
            var statement = Sql.Select.Output(Sql.Scalar(true).Not());
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT NOT ( True );", text);
        }

        [TestMethod]
        public void AllToken()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("Customers").Where(Sql.Name("rating").Greater(Sql.All(Sql.Select.Output("rating").From("Customers_new"))));
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM \"Customers\" WHERE \"rating\" > ALL ( SELECT \"rating\" FROM \"Customers_new\" );", text);
        }

        [TestMethod]
        public void AnyToken()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("Customers").Where(Sql.Name("rating").Greater(Sql.Any(Sql.Select.Output("rating").From("Customers_new"))));
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM \"Customers\" WHERE \"rating\" > ANY ( SELECT \"rating\" FROM \"Customers_new\" );", text);
        }
    }
}
