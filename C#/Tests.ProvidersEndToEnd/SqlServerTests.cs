// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.SqlServer;

namespace Tests.ProvidersEndToEnd
{
    [TestClass]
    public class SqlServerTests
    {
        private const string ConnectionString = @"Data Source=(LocalDb)\v11.0; Integrated Security = true;";
        static readonly IProvider Provider = new SqlServerProvider();


        [TestMethod]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatement();
            Common.GenerateAndExecute(Provider, statement, ConnectionString);
        }
    }
}