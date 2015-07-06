using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.Sqlite
{
    public static class SqliteAssert
    {
        public static SqliteProvider Provider = new SqliteProvider();

        public static void AreEqual(IStatement statement, string script)
        {

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual(script, text); 
        }


    }
}
