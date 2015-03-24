namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {
        protected override void VisitCreateTableStatement(CreateTableStatement statement, VisitorState state)
        {
            state.Write(Symbols.CREATE);
            if (statement.IsTableVariable || statement.IsTemporary)
            {
                state.Write(Symbols.TEMPORARY);
            }
            state.Write(Symbols.TABLE);

            if (statement.CheckIfNotExists)
            {
                state.Write(Symbols.IF);
                state.Write(Symbols.NOT);
                state.Write(Symbols.EXISTS);
            }

            VisitNameToken(statement.Name, state);

            var separator = "(";
            foreach (var column in statement.Columns)
            {
                state.Write(separator);
                separator = ",";

                state.Write("\"", column.Name, "\"");

                VisitType(column, state);

                if (column.PrimaryKeyDirection.HasValue)
                {
                    state.Write(Symbols.PRIMARY);
                    state.Write(Symbols.KEY);
                    state.Write(column.PrimaryKeyDirection.Value == Direction.Asc ? Symbols.ASC : Symbols.DESC);
                    VisitConflict(column.PrimaryKeyConflict, state);

                    if (column.Identity.On)
                    {
                        state.Write(Symbols.AUTOINCREMENT);
                    }
                }

                if (column.Null.HasValue)
                {
                    if (!column.Null.Value)
                    {
                        state.Write(Symbols.NOT);
                    }
                    state.Write(Symbols.NULL);
                    VisitConflict(column.NullConflict, state);
                }

                if (column.DefaultValue != null)
                {
                    state.Write(Symbols.DEFAULT);
                    state.Write(Symbols.OpenParenthesis);
                    VisitToken(column.DefaultValue, state);
                    state.Write(Symbols.CloseParenthesis);
                }
            }

            if (statement.PrimaryKey != null)
            {
                if (statement.PrimaryKey.Name != null)
                {
                    state.Write(Symbols.Comma);
                    state.Write(Symbols.CONSTRAINT);
                    VisitNameToken(statement.PrimaryKey.Name, state);
                }
                state.Write(Symbols.PRIMARY);
                state.Write(Symbols.KEY);
                VisitTokenSet(statement.PrimaryKey.Columns, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
                VisitConflict(statement.PrimaryKey.Conflict, state);
            }

            foreach (var unique in statement.UniqueConstrains)
            {
                state.Write(Symbols.Comma);
                state.Write(Symbols.CONSTRAINT);
                VisitNameToken(unique.Name, state);
                state.Write(Symbols.UNIQUE);
                VisitTokenSet(unique.Columns, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
                VisitConflict(unique.Conflict, state);
            }

            state.Write(Symbols.CloseParenthesis);

            // if indecies are set, create them
            if (statement.Indicies.Count > 0)
            {
                state.WriteStatementTerminator();
                foreach (var createIndexStatement in statement.Indicies)
                {
                    createIndexStatement.CheckIfNotExists |= statement.CheckIfNotExists;
                    VisitCreateIndexStatement(createIndexStatement, state);
                }
            }
        }

        protected override void VisitDropTableStatement(DropTableStatement statement, VisitorState state)
        {
            state.Write(Symbols.DROP);
            state.Write(Symbols.TABLE);
            if (statement.CheckExists)
            {
                state.Write(Symbols.IF);
                state.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name, state);
        }

        protected override void VisitCreateIndexStatement(CreateIndexStatement statement, VisitorState state)
        {
            state.Write(Symbols.CREATE);

            if (statement.Unique)
            {
                state.Write(Symbols.UNIQUE);
            }

            state.Write(Symbols.INDEX);

            if (statement.CheckIfNotExists)
            {
                state.Write(Symbols.IF);
                state.Write(Symbols.NOT);
                state.Write(Symbols.EXISTS);
            }

            VisitToken(statement.Name, state);

            state.Write(Symbols.ON);

            VisitToken(statement.On, state);

            // columns
            VisitTokenSet(statement.Columns, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
            VisitWhereToken(statement.Where, state);
        }

        protected override void VisitAlterIndexStatement(AlterIndexStatement statement, VisitorState state)
        {
            if (statement.Rebuild)
            {
                state.Write(Symbols.REINDEX);

                if (statement.Name == null)
                {
                    VisitToken(statement.On, state);
                }
                else
                {
                    VisitToken(Sql.Name(statement.On.FirstPart, statement.Name.LastPart), state);
                }
            }
        }

        protected override void VisitDropIndexStatement(DropIndexStatement statement, VisitorState state)
        {
            state.Write(Symbols.DROP);
            state.Write(Symbols.INDEX);

            if (statement.CheckExists)
            {
                state.Write(Symbols.IF);
                state.Write(Symbols.EXISTS);
            }

            if (statement.On != null)
            {
                var name = Sql.Name(statement.On.FirstPart, statement.Name.LastPart);
                VisitToken(name, state);
            }
            else
            {
                VisitToken(statement.Name, state);
            }
        }

        protected override void VisitCreateViewStatement(CreateViewStatement statement, VisitorState state)
        {
            state.Write(Symbols.CREATE);
            if (statement.IsTemporary)
            {
                state.Write(Symbols.TEMPORARY);
            }
            state.Write(Symbols.VIEW);

            if (statement.CheckIfNotExists)
            {
                state.Write(Symbols.IF);
                state.Write(Symbols.NOT);
                state.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name, state);
            state.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }

        protected override void VisitAlterViewStatement(AlterViewStatement statement, VisitorState state)
        {
            state.Write(Symbols.DROP);
            state.Write(Symbols.VIEW);
            state.Write(Symbols.IF);
            state.Write(Symbols.EXISTS);
            VisitNameToken(statement.Name, state);
            state.WriteStatementTerminator();

            state.Write(Symbols.CREATE);
            if (statement.IsTemporary)
            {
                state.Write(Symbols.TEMPORARY);
            }
            state.Write(Symbols.VIEW);

            VisitNameToken(statement.Name, state);
            state.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }

        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement, VisitorState state)
        {
            state.Write(Symbols.DROP);
            state.Write(Symbols.VIEW);
            state.Write(Symbols.IF);
            state.Write(Symbols.EXISTS);
            VisitNameToken(statement.Name, state);
            state.WriteStatementTerminator();

            state.Write(Symbols.CREATE);
            if (statement.IsTemporary)
            {
                state.Write(Symbols.TEMPORARY);
            }
            state.Write(Symbols.VIEW);

            VisitNameToken(statement.Name, state);
            state.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }
        protected override void VisitDropViewStatement(DropViewStatement statement, VisitorState state)
        {
            state.Write(Symbols.DROP);
            state.Write(Symbols.VIEW);

            if (statement.CheckExists)
            {
                state.Write(Symbols.IF);
                state.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name, state);
        }

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