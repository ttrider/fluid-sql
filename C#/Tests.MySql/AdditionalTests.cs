using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.MySqlTests
{
    /// <summary>
    /// Summary description for AdditionalTests
    /// </summary>
    [TestClass]
    public class AdditionalTests: MySqlProviderTests
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
        public void DeclareWithInitStatement()
        {
            var statement = Sql.Declare(Parameter.Text("test_text"), Sql.Scalar("test"));
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("DECLARE test_text TEXT DEFAULT N'test';", text);
        }

        [TestMethod]
        public void SetStatement()
        {
            var statement = Sql.Set(Sql.Name("@test_text"), Sql.Scalar("test_text"));
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("SET @test_text = N'test_text';", text);
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
            Assert.AreEqual("SELECT * FROM `Customers` WHERE `rating` > ALL ( SELECT `rating` FROM `Customers_new` );", text);
        }

        [TestMethod]
        public void AnyToken()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("Customers").Where(Sql.Name("rating").Greater(Sql.Any(Sql.Select.Output("rating").From("Customers_new"))));
            var text = Provider.GenerateStatement(statement);
            Assert.IsNotNull(text);
            Assert.AreEqual("SELECT * FROM `Customers` WHERE `rating` > ANY ( SELECT `rating` FROM `Customers_new` );", text);
        }
    }
}
