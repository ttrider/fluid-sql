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
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("BEGIN TRANSACTION;\r\nSAVE TRANSACTION;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;", text); return statement;

        }
        [TestMethod]
        public IStatement BeginRollbackMarkedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("foo"), "marked"),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("BEGIN TRANSACTION [foo] WITH MARK 'marked';\r\nSAVE TRANSACTION;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;", text); return statement;

        }
        [TestMethod]
        public IStatement BeginRollbackNamedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("t")),
                Sql.SaveTransaction(Sql.Name("s")),
                Sql.RollbackTransaction(Sql.Name("s")),
                Sql.CommitTransaction(Sql.Name("t")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("BEGIN TRANSACTION [t];\r\nSAVE TRANSACTION [s];\r\nROLLBACK TRANSACTION [s];\r\nCOMMIT TRANSACTION [t];", text); return statement;

        }

        [TestMethod]
        public IStatement BeginRollbackParameterTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Parameter.Any("@t")),
                Sql.SaveTransaction(Parameter.Any("@s")),
                Sql.RollbackTransaction(Parameter.Any("@s")),
                Sql.CommitTransaction(Parameter.Any("@t")));

            var text = Provider.GenerateStatement(statement);

            Assert.IsNotNull(text);
            Assert.AreEqual("BEGIN TRANSACTION @t;\r\nSAVE TRANSACTION @s;\r\nROLLBACK TRANSACTION @s;\r\nCOMMIT TRANSACTION @t;", text); return statement;

        }

    }
}
