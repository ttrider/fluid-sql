using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {
        private void VisitCreateTableStatement(CreateTableStatement statement, VisitorState state)
        {
            state.Write(statement.IsTableVariable || statement.IsTemporary ? Sym.CREATE_TEMP_TABLE : Sym.CREATE_TABLE);

            if (statement.CheckIfNotExists)
            {
                state.Write(Sym.IF_NOT_EXISTS);
            }

            state.Write(ResolveName(statement.Name));

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
                    state.Write(ResolveName(statement.PrimaryKey.Name));
                }
                state.Write(Sym.PRIMARY_KEY);
                VisitTokenSet(statement.PrimaryKey.Columns, state, Sym.op, Sym.COMMA,Sym.cp );
                VisitConflict(statement.PrimaryKey.Conflict, state);
            }

            foreach (var unique in statement.UniqueConstrains)
            {
                state.Write(Sym.COMMA);
                state.Write(Sym.CONSTRAINT);
                state.Write(ResolveName(unique.Name));
                state.Write(Sym.UNIQUE);
                VisitTokenSet(statement.PrimaryKey.Columns, state, Sym.op, Sym.COMMA, Sym.cp);
                state.Write(string.Join(Sym.COMMA,
                    unique.Columns.Select(n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym.ASC : Sym.DESC))));
                VisitConflict(unique.Conflict, state);
            }


            state.Write(Sym.cpsc);

            // if indecies are set, create them
            if (statement.Indicies.Count > 0)
            {
                foreach (var createIndexStatement in statement.Indicies)
                {
                    createIndexStatement.CheckIfNotExists |= statement.CheckIfNotExists;
                    VisitCreateIndexStatement(createIndexStatement, state);
                }
            }
        }

        private void VisitDropTableStatement(DropTableStatement statement, VisitorState state)
        {
            state.Write(Sym.DROP_TABLE);
            if (statement.CheckExists)
            {
                state.Write(Sym.IF_EXISTS);
            }
            state.Write(ResolveName(statement.Name));
        }

        private void VisitCreateIndexStatement(CreateIndexStatement statement, VisitorState state)
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
            VisitWhere(statement.Where, state);
        }

        private void VisitAlterIndexStatement(AlterIndexStatement statement, VisitorState state)
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


        private void VisitDropIndexStatement(DropIndexStatement statement, VisitorState state)
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


        private void VisitCreateViewStatement(CreateViewStatement statement, VisitorState state)
        {
            state.Write(statement.IsTemporary ? Sym.CREATE_TEMPORARY_VIEW : Sym.CREATE_VIEW);

            if (statement.CheckIfNotExists)
            {
                state.Write(Sym.IF_NOT_EXISTS);
            }
            state.Write(ResolveName(statement.Name));
            state.Write(Sym.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }

        private void VisitAlterViewStatement(AlterViewStatement statement, VisitorState state)
        {
            state.Write(Sym.DROP_VIEW);
            state.Write(Sym.IF_EXISTS);
            state.Write(ResolveName(statement.Name));
            state.WriteStatementTerminator();

            state.Write(statement.IsTemporary ? Sym.CREATE_TEMPORARY_VIEW : Sym.CREATE_VIEW);

            state.Write(ResolveName(statement.Name));
            state.Write(Sym.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }

        private void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement, VisitorState state)
        {
            state.Write(Sym.DROP_VIEW);
            state.Write(Sym.IF_EXISTS);
            state.Write(ResolveName(statement.Name));
            state.WriteStatementTerminator();

            state.Write(statement.IsTemporary ? Sym.CREATE_TEMPORARY_VIEW : Sym.CREATE_VIEW);

            state.Write(ResolveName(statement.Name));
            state.Write(Sym.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }
        private void VisitDropViewStatement(DropViewStatement statement, VisitorState state)
        {
            state.Write(Sym.DROP_VIEW);
            if (statement.CheckExists)
            {
                state.Write(Sym.IF_EXISTS);
            }
            state.Write(ResolveName(statement.Name));
        }


        private void VisitBeginTransaction(BeginTransactionStatement statement, VisitorState state)
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

        private void VisitSaveTransaction(TransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.SAVEPOINT_);
            VisitTransactionName(statement, state);
        }

        private void VisitRollbackTransaction(TransactionStatement statement, VisitorState state)
        {
            state.Write("ROLLBACK TRANSACTION");

            if (statement.Name != null || statement.Parameter != null)
            {
                state.Write(" TO SAVEPOINT");
                VisitTransactionName(statement, state);
            }
        }

        private void VisitCommitTransaction(TransactionStatement statement, VisitorState state)
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