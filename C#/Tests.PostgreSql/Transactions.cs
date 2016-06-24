using TTRider.FluidSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.PostgreSql
{
    [TestClass]
    public class Transactions : PostgreSqlProviderTests
    {
        [TestMethod]
        public void BeginTransaction()
        {
            var statement = Sql.Statements(Sql.BeginTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION;", command.CommandText);
        }

        [TestMethod]
        public void BeginSerializableTransaction()
        {
            var statement = Sql.Statements(Sql.BeginSerializableTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION ISOLATION LEVEL SERIALIZABLE;", command.CommandText);
        }

        [TestMethod]
        public void BeginRepeatebleReadTransaction()
        {
            var statement = Sql.Statements(Sql.BeginRepeatebleReadTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION ISOLATION LEVEL REPEATABLE READ;", command.CommandText);
        }

        [TestMethod]
        public void BeginReadCommitedTransaction()
        {
            var statement = Sql.Statements(Sql.BeginReadCommitedTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION ISOLATION LEVEL READ COMMITED;", command.CommandText);
        }

        [TestMethod]
        public void BeginReadUnCommitedTransaction()
        {
            var statement = Sql.Statements(Sql.BeginReadUnCommitedTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION ISOLATION LEVEL READ UNCOMMITED;", command.CommandText);
        }

        [TestMethod]
        public void BeginReadOnlyTransaction()
        {
            var statement = Sql.Statements(Sql.BeginReadOnlyTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION READ ONLY;", command.CommandText);
        }

        [TestMethod]
        public void BeginReadWriteTransaction()
        {
            var statement = Sql.Statements(Sql.BeginReadWriteTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION READ WRITE;", command.CommandText);
        }

        #region Unit tests from FluidSql
        [TestMethod]
        public void BeginCommitTransactions()
        {
            var statement = Sql.Statements(Sql.BeginTransaction(), Sql.CommitTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION;\r\nCOMMIT TRANSACTION;", command.CommandText);
        }
        
        [TestMethod]
        public void BeginRollbackTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION;\r\nSAVEPOINT;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;", command.CommandText);
        }

        [TestMethod]
        public void BeginRollbackMarkedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("foo"), "marked"),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION \"foo\";\r\nSAVEPOINT;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;", command.CommandText);
            
        }
        
        [TestMethod]
        public void BeginRollbackNamedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("t")),
                Sql.SaveTransaction(Sql.Name("s")),
                Sql.RollbackTransaction(Sql.Name("s")),
                Sql.CommitTransaction(Sql.Name("t")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION \"t\";\r\nSAVEPOINT \"s\";\r\nROLLBACK TRANSACTION TO SAVEPOINT \"s\";\r\nCOMMIT TRANSACTION \"t\";", command.CommandText);
        }

        [TestMethod]
        public void BeginRollbackParameterTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Parameter.Any("@t")),
                Sql.SaveTransaction(Parameter.Any("@s")),
                Sql.RollbackTransaction(Parameter.Any("@s")),
                Sql.CommitTransaction(Parameter.Any("@t")));

            var command = Provider.GetCommand(statement);

            Assert.IsNotNull(command);
            Assert.AreEqual("BEGIN TRANSACTION @t;\r\nSAVEPOINT @s;\r\nROLLBACK TRANSACTION TO SAVEPOINT @s;\r\nCOMMIT TRANSACTION @t;", command.CommandText);
        }       

        #endregion
    }
}
