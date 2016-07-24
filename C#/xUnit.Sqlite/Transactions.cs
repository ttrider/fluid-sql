// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using TTRider.FluidSql.Providers.Sqlite;
using Xunit;

namespace xUnit.Sqlite
{
    public class Transactions
    {
        public static SqliteProvider Provider = new SqliteProvider();

        [Fact]
        public IStatement BeginCommitTransactions()
        {
            var statement = Sql.Statements(Sql.BeginTransaction(), Sql.CommitTransaction());

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal("BEGIN TRANSACTION;\r\nCOMMIT TRANSACTION;", text);
            return statement;
        }

        [Fact]
        public IStatement BeginRollbackTransactions()
        {
            var statement = Sql.Statements(
                Sql.BeginTransaction(),
                Sql.Savepoint("foo"),
                Sql.ReleaseToSavepoint("foo"),
                Sql.CommitTransaction());

            var text = Provider.GenerateStatement(statement);

            Assert.NotNull(text);
            Assert.Equal(
                "BEGIN TRANSACTION;\r\nSAVEPOINT \"foo\";\r\nRELEASE SAVEPOINT \"foo\";\r\nCOMMIT TRANSACTION;", text);
            return statement;
        }
    }
}