using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.SqlServer;

namespace Tests.ProvidersEndToEnd
{
    [TestClass]
    public class SqlServerTests
    {
        static readonly IProvider Provider = new SqlServerProvider();
        private const string ConnectionString = @"Data Source=(LocalDb)\v11.0; Integrated Security = true;";


        [TestMethod]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatement();
            Common.GenerateAndExecute(Provider, statement, ConnectionString);
        }
    }
}
