namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {
        protected override void VisitCreateTableStatement(CreateTableStatement statement, VisitorState state)
        {
            state.Write(statement.IsTableVariable || statement.IsTemporary ? Sym.CREATE_TEMP_TABLE : Sym.CREATE_TABLE);

            if (statement.CheckIfNotExists)
            {
                state.Write(Sym.IF_NOT_EXISTS);
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
                    state.Write(Sym.PRIMARY_KEY);
                    state.Write(column.PrimaryKeyDirection.Value == Direction.Asc ? Sym.ASC : Sym.DESC);
                    VisitConflict(column.PrimaryKeyConflict, state);

                    if (column.Identity.On)
                    {
                        state.Write(Sym.AUTOINCREMENT);
                    }
                }

                if (column.Null.HasValue)
                {
                    state.Write(column.Null.Value ? Sym.NULL : Sym.NOT_NULL);
                    VisitConflict(column.NullConflict, state);
                }

                if (column.DefaultValue != null)
                {
                    state.Write(Sym.DEFAULT_op);
                    VisitToken(column.DefaultValue, state);
                    state.Write(Sym.cp);
                }
            }

            if (statement.PrimaryKey != null)
            {
                if (statement.PrimaryKey.Name != null)
                {
                    state.Write(Sym.COMMA);
                    state.Write(Sym.CONSTRAINT);
                    VisitNameToken(statement.PrimaryKey.Name, state);
                }
                state.Write(Sym.PRIMARY_KEY);
                VisitTokenSet(statement.PrimaryKey.Columns, state, Sym.op, Sym.COMMA,Sym.cp );
                VisitConflict(statement.PrimaryKey.Conflict, state);
            }

            foreach (var unique in statement.UniqueConstrains)
            {
                state.Write(Sym.COMMA);
                state.Write(Sym.CONSTRAINT);
                VisitNameToken(unique.Name, state);
                state.Write(Sym.UNIQUE);
                VisitTokenSet(unique.Columns, state, Sym.op, Sym.COMMA, Sym.cp);
                VisitConflict(unique.Conflict, state);
            }

            state.Write(Sym.cp);

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
            state.Write(Sym.DROP_TABLE);
            if (statement.CheckExists)
            {
                state.Write(Sym.IF_EXISTS);
            }
            VisitNameToken(statement.Name, state);
        }

        protected override void VisitCreateIndexStatement(CreateIndexStatement statement, VisitorState state)
        {
            state.Write(Sym.CREATE);

            if (statement.Unique)
            {
                state.Write(Sym.UNIQUE);
            }

            state.Write(Sym.INDEX);

            if (statement.CheckIfNotExists)
            {
                state.Write(Sym.IF_NOT_EXISTS);
            }

            VisitToken(statement.Name, state);

            state.Write(Sym.ON);

            VisitToken(statement.On, state);

            // columns
            VisitTokenSet(statement.Columns, state, Sym.op, Sym.COMMA, Sym.cp);
            VisitWhereToken(statement.Where, state);
        }

        protected override void VisitAlterIndexStatement(AlterIndexStatement statement, VisitorState state)
        {
            if (statement.Rebuild)
            {
                state.Write(Sym.REINDEX);

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
            state.Write(Sym.DROP_INDEX);

            if (statement.CheckExists)
            {
                state.Write(Sym.IF_EXISTS);
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
            state.Write(statement.IsTemporary ? Sym.CREATE_TEMPORARY_VIEW : Sym.CREATE_VIEW);

            if (statement.CheckIfNotExists)
            {
                state.Write(Sym.IF_NOT_EXISTS);
            }
            VisitNameToken(statement.Name, state);
            state.Write(Sym.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }

        protected override void VisitAlterViewStatement(AlterViewStatement statement, VisitorState state)
        {
            state.Write(Sym.DROP_VIEW);
            state.Write(Sym.IF_EXISTS);
            VisitNameToken(statement.Name, state);
            state.WriteStatementTerminator();

            state.Write(statement.IsTemporary ? Sym.CREATE_TEMPORARY_VIEW : Sym.CREATE_VIEW);

            VisitNameToken(statement.Name, state);
            state.Write(Sym.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }

        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement, VisitorState state)
        {
            state.Write(Sym.DROP_VIEW);
            state.Write(Sym.IF_EXISTS);
            VisitNameToken(statement.Name, state);
            state.WriteStatementTerminator();

            state.Write(statement.IsTemporary ? Sym.CREATE_TEMPORARY_VIEW : Sym.CREATE_VIEW);

            VisitNameToken(statement.Name, state);
            state.Write(Sym.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }
        protected override void VisitDropViewStatement(DropViewStatement statement, VisitorState state)
        {
            state.Write(Sym.DROP_VIEW);
            if (statement.CheckExists)
            {
                state.Write(Sym.IF_EXISTS);
            }
            VisitNameToken(statement.Name, state);
        }

        protected override void VisitBeginTransaction(BeginTransactionStatement statement, VisitorState state)
        {
            if (statement.Type.HasValue)
            {
                switch (statement.Type.Value)
                {
                    case TransactionType.Deferred:
                        state.Write(Sym.BEGIN_TRANSACTION);
                        break;
                    case TransactionType.Immediate:
                        state.Write(Sym.BEGIN_TRANSACTION);
                        break;
                    case TransactionType.Exclusive:
                        state.Write(Sym.BEGIN_TRANSACTION);
                        break;
                }
            }
            else
            {
                state.Write(Sym.BEGIN_TRANSACTION);
            }
        }

        protected override void VisitSaveTransaction(SaveTransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.SAVEPOINT_);
            VisitTransactionName(statement, state);
        }

        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement, VisitorState state)
        {
            state.Write("ROLLBACK TRANSACTION");

            if (statement.Name != null || statement.Parameter != null)
            {
                state.Write(" TO SAVEPOINT");
                VisitTransactionName(statement, state);
            }
        }

        protected override void VisitCommitTransaction(CommitTransactionStatement statement, VisitorState state)
        {
            if (statement.Name != null || statement.Parameter != null)
            {
                state.Write("RELEASE SAVEPOINT ");
                VisitTransactionName(statement, state);
            }
            else
            {
                state.Write("COMMIT TRANSACTION");
            }
        }

    }
}