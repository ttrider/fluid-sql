// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql.Providers;
using TTRider.FluidSql.Providers.SqlServer;
using xUnit.Functional;
using Xunit;

namespace Tests.ProvidersEndToEnd
{

    public class SqlServerTests
    {
        private const string ConnectionString = @"Data Source=(LocalDb)\v11.0; Integrated Security = true;";
        static readonly IProvider Provider = new SqlServerProvider();


        [Fact]
        public void SimpleStatement()
        {
            var statement = Common.CreateSimpleStatements();
            Common.GenerateAndExecute(Provider, statement, ConnectionString);
        }
    }
}