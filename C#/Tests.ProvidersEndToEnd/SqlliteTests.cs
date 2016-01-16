using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.ProvidersEndToEnd
{
    [TestClass]
    public class SqliteTests
    {
        static readonly IProvider Provider = new SqliteProvider();
        private const string ConnectionString = @"Data Source =:memory:; Version = 3; New = True;";


        [TestMethod]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatement();
            Common.GenerateAndExecute(Provider, statement, ConnectionString);
        }
    }
}
