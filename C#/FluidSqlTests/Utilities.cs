using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.SqlServer;

namespace FluidSqlTests
{
    public static class Utilities
    {
        public static string ConnectionString = @"Data Source=.;Integrated Security=True";

        public static SqlServerProvider SqlProvider = new SqlServerProvider();

        public static IDbCommand GetCommand(IStatement statement)
        {
            return  SqlProvider.GetCommand(ConnectionString, statement);
        }
        public static IDbCommand GetCommandAsync(IStatement statement)
        {
            var t = SqlProvider.GetCommandAsync(ConnectionString, statement);
            t.Wait();
            return t.Result;
        }
    }
}
