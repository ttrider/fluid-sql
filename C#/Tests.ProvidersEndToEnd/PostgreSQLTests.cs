using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.PostgreSQL;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.ProvidersEndToEnd
{
    [TestClass]
    public class PostgreSQLTests
    {
        // create database fluidtest

        static readonly IProvider Provider = new PostgreSQLProvider();
        private const string ConnectionString = @"Server=127.0.0.1;Port=5432;Database=fluidtest;Integrated Security=true;";


        [TestMethod]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatement();
            Common.GenerateAndExecute(Provider, statement, ConnectionString);
        }
    }
}
