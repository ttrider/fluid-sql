// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {
        protected override void VisitBeginTransaction(BeginTransactionStatement statement)
        {
            State.Write(Symbols.BEGIN);
            if (statement.Type.HasValue)
            {
                switch (statement.Type.Value)
                {
                    case TransactionType.Deferred:
                        State.Write(Symbols.DEFERRED);
                        break;
                    case TransactionType.Immediate:
                        State.Write(Symbols.IMMIDIATE);
                        break;
                    case TransactionType.Exclusive:
                        State.Write(Symbols.EXCLUSIVE);
                        break;
                }
            }
            State.Write(Symbols.TRANSACTION);
        }

        protected override void VisitSaveTransaction(SaveTransactionStatement statement)
        {
            State.Write(Symbols.SAVEPOINT);
            VisitTransactionName(statement);
        }

        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement)
        {
            State.Write(Symbols.ROLLBACK);
            State.Write(Symbols.TRANSACTION);

            if (statement.Name != null || statement.Parameter != null)
            {
                State.Write(Symbols.TO);
                State.Write(Symbols.SAVEPOINT);
                VisitTransactionName(statement);
            }
        }

        protected override void VisitCommitTransaction(CommitTransactionStatement statement)
        {
            if (statement.Name != null || statement.Parameter != null)
            {
                State.Write(Symbols.RELEASE);
                State.Write(Symbols.SAVEPOINT);
                VisitTransactionName(statement);
            }
            else
            {
                State.Write(Symbols.COMMIT);
                State.Write(Symbols.TRANSACTION);
            }
        }
    }
}