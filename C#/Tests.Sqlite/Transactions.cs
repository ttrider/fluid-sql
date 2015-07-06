using System;
using System.Data.SQLite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;

namespace Tests.Sqlite
{
    [TestClass]
    public class Transactions
    {
        public static SqliteProvider Provider = new SqliteProvider();
        [TestMethod]
        public IStatement BeginCommitTransactions()
        {
            var statement = Sql.Statements(Sql.BeginTransaction(), Sql.CommitTransaction());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("BEGIN TRANSACTION;\r\nCOMMIT TRANSACTION;", text); return statement;
        }

        [TestMethod]
        public IStatement BeginRollbackTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(),
                Sql.Savepoint("foo"),
                Sql.ReleaseToSavepoint("foo"),
                Sql.CommitTransaction());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("BEGIN TRANSACTION;\r\nSAVEPOINT \"foo\";\r\nRELEASE SAVEPOINT \"foo\";\r\nCOMMIT TRANSACTION;", text); return statement;

        }
    }
}
