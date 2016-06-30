using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.ProvidersEndToEnd
{
    [TestClass]
    public class SqliteTests
    {
        private const string ConnectionString = @"Data Source =:memory:; Version = 3; New = True;";
        static readonly IProvider Provider = new SqliteProvider();


        [TestMethod]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatement();
            Common.GenerateAndExecute(Provider, statement, ConnectionString);
        }
    }
}