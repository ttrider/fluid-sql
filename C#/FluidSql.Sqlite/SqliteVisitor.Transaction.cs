namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {

        protected override void VisitBeginTransaction(BeginTransactionStatement statement, VisitorState state)
        {
            state.Write(Symbols.BEGIN);
            if (statement.Type.HasValue)
            {
                switch (statement.Type.Value)
                {
                    case TransactionType.Deferred:
                        state.Write(Symbols.DEFERRED);
                        break;
                    case TransactionType.Immediate:
                        state.Write(Symbols.IMMIDIATE);
                        break;
                    case TransactionType.Exclusive:
                        state.Write(Symbols.EXCLUSIVE);
                        break;
                }
            }
            state.Write(Symbols.TRANSACTION);
        }

        protected override void VisitSaveTransaction(SaveTransactionStatement statement, VisitorState state)
        {
            state.Write(Symbols.SAVEPOINT);
            VisitTransactionName(statement, state);
        }

        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement, VisitorState state)
        {
            state.Write(Symbols.ROLLBACK);
            state.Write(Symbols.TRANSACTION);

            if (statement.Name != null || statement.Parameter != null)
            {
                state.Write(Symbols.TO);
                state.Write(Symbols.SAVEPOINT);
                VisitTransactionName(statement, state);
            }
        }

        protected override void VisitCommitTransaction(CommitTransactionStatement statement, VisitorState state)
        {
            if (statement.Name != null || statement.Parameter != null)
            {
                state.Write(Symbols.RELEASE);
                state.Write(Symbols.SAVEPOINT);
                VisitTransactionName(statement, state);
            }
            else
            {
                state.Write(Symbols.COMMIT);
                state.Write(Symbols.TRANSACTION);
            }
        }
    }
}