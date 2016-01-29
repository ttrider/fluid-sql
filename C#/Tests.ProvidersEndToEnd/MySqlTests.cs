using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.MySql;

namespace Tests.ProvidersEndToEnd
{
    [TestClass]
    public class MySqlTests
    {
        static readonly IProvider Provider = new MySqlProvider();
        private const string ConnectionString = @"Data Source =:memory:; Version = 3; New = True;";


        [TestMethod]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatement();
            Common.GenerateAndExecute(Provider, statement, ConnectionString);
        }
    }
}
