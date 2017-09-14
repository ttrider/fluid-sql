// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.Redshift
{
    public class Transactions : RedshiftSqlProviderTests
    {
        [Fact]
        public void BeginTransaction()
        {
            var statement = Sql.Statements(Sql.BeginTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION;", command.CommandText);
        }

        [Fact]
        public void BeginSerializableTransaction()
        {
            var statement = Sql.Statements(Sql.BeginSerializableTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION ISOLATION LEVEL SERIALIZABLE;", command.CommandText);
        }

        [Fact]
        public void BeginRepeatebleReadTransaction()
        {
            var statement = Sql.Statements(Sql.BeginRepeatebleReadTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION ISOLATION LEVEL REPEATABLE READ;", command.CommandText);
        }

        [Fact]
        public void BeginReadCommitedTransaction()
        {
            var statement = Sql.Statements(Sql.BeginReadCommitedTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION ISOLATION LEVEL READ COMMITED;", command.CommandText);
        }

        [Fact]
        public void BeginReadUnCommitedTransaction()
        {
            var statement = Sql.Statements(Sql.BeginReadUnCommitedTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION ISOLATION LEVEL READ UNCOMMITED;", command.CommandText);
        }

        [Fact]
        public void BeginReadOnlyTransaction()
        {
            var statement = Sql.Statements(Sql.BeginReadOnlyTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION READ ONLY;", command.CommandText);
        }

        [Fact]
        public void BeginReadWriteTransaction()
        {
            var statement = Sql.Statements(Sql.BeginReadWriteTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION READ WRITE;", command.CommandText);
        }

        #region Unit tests from FluidSql
        [Fact]
        public void BeginCommitTransactions()
        {
            var statement = Sql.Statements(Sql.BeginTransaction(), Sql.CommitTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION;\r\nCOMMIT TRANSACTION;", command.CommandText);
        }
        
        [Fact]
        public void BeginRollbackTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION;\r\nSAVEPOINT;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;", command.CommandText);
        }

        [Fact]
        public void BeginRollbackMarkedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("foo"), "marked"),
                Sql.SaveTransaction(),
                Sql.RollbackTransaction(),
                Sql.CommitTransaction());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION \"foo\";\r\nSAVEPOINT;\r\nROLLBACK TRANSACTION;\r\nCOMMIT TRANSACTION;", command.CommandText);
            
        }
        
        [Fact]
        public void BeginRollbackNamedTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Sql.Name("t")),
                Sql.SaveTransaction(Sql.Name("s")),
                Sql.RollbackTransaction(Sql.Name("s")),
                Sql.CommitTransaction(Sql.Name("t")));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION \"t\";\r\nSAVEPOINT \"s\";\r\nROLLBACK TRANSACTION TO SAVEPOINT \"s\";\r\nCOMMIT TRANSACTION \"t\";", command.CommandText);
        }

        [Fact]
        public void BeginRollbackParameterTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(Parameter.Any("@t")),
                Sql.SaveTransaction(Parameter.Any("@s")),
                Sql.RollbackTransaction(Parameter.Any("@s")),
                Sql.CommitTransaction(Parameter.Any("@t")));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("BEGIN TRANSACTION @t;\r\nSAVEPOINT @s;\r\nROLLBACK TRANSACTION TO SAVEPOINT @s;\r\nCOMMIT TRANSACTION @t;", command.CommandText);
        }       
        #endregion
    }
}
