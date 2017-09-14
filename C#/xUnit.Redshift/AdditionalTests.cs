// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.Redshift
{
    public class AdditionalTests : RedshiftSqlProviderTests
    {
        [Fact]
        public void DeclareStatement()
        {
            var statement = Sql.Declare(Parameter.Int("test_variable"));
            var text = Provider.GenerateStatement(statement);
            Assert.NotNull(text);
            Assert.Equal("DECLARE test_variable INTEGER;", text);
        }

        [Fact]
        public void DeclareNextStatement()
        {
            /*var statement = Sql.Declare(Parameter.Text("test_text"), null, false);
            var text = Provider.GenerateStatement(statement);
            Assert.NotNull(text);
            Assert.Equal("test_text TEXT;", text);*/
        }

        [Fact]
        public void SetStatement()
        {
            /*var statement = Sql.Set(Sql.Name("test_text"), Sql.Scalar("test_text"));
            var text = Provider.GenerateStatement(statement);
            Assert.NotNull(text);
            Assert.Equal("\"test_text\" := 'test_text';", text);*/
        }

        [Fact]
        public void BitwiseNotToken()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).BitwiseNot());
            var text = Provider.GenerateStatement(statement);
            Assert.NotNull(text);
            Assert.Equal("SELECT ~ 1;", text);
        }

        [Fact]
        public void UnaryMinusToken()
        {
            var statement = Sql.Select.Output(Sql.Scalar(1).Minus());
            var text = Provider.GenerateStatement(statement);
            Assert.NotNull(text);
            Assert.Equal("SELECT - 1;", text);
        }

        [Fact]
        public void NotToken()
        {
            var statement = Sql.Select.Output(Sql.Scalar(true).Not());
            var text = Provider.GenerateStatement(statement);
            Assert.NotNull(text);
            Assert.Equal("SELECT NOT ( TRUE );", text);
        }

        [Fact]
        public void AllToken()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("Customers").Where(Sql.Name("rating").Greater(Sql.All(Sql.Select.Output("rating").From("Customers_new"))));
            var text = Provider.GenerateStatement(statement);
            Assert.NotNull(text);
            Assert.Equal("SELECT * FROM \"Customers\" WHERE \"rating\" > ALL ( SELECT \"rating\" FROM \"Customers_new\" );", text);
        }

        [Fact]
        public void AnyToken()
        {
            var statement = Sql.Select.Output(Sql.Star()).From("Customers").Where(Sql.Name("rating").Greater(Sql.Any(Sql.Select.Output("rating").From("Customers_new"))));
            var text = Provider.GenerateStatement(statement);
            Assert.NotNull(text);
            Assert.Equal("SELECT * FROM \"Customers\" WHERE \"rating\" > ANY ( SELECT \"rating\" FROM \"Customers_new\" );", text);
        }
    }
}
