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

            var separator = " (";
            foreach (var column in statement.Columns)
            {
                state.Write(separator);
                separator = ", ";

                state.Write("\"", column.Name, "\"");

                VisitType(column, state);

                if (column.PrimaryKeyDirection.HasValue)
                {
                    state.Write(Sym._PRIMARY_KEY);
                    state.Write(column.PrimaryKeyDirection.Value == Direction.Asc ? Sym.ASC : Sym.DESC);
                    VisitConflict(column.PrimaryKeyConflict, state);

                    if (column.Identity.On)
                    {
                        state.Write(Sym.AUTOINCREMENT);
                    }
                }

                if (column.Null.HasValue)
                {
                    state.Write(column.Null.Value ? Sym._NULL : Sym._NOT_NULL);
                    VisitConflict(column.NullConflict, state);
                }

                if (column.DefaultValue != null)
                {
                    state.Write(Sym._DEFAULT_op);
                    VisitToken(column.DefaultValue, state);
                    state.Write(Sym._cp);
                }
            }

            if (statement.PrimaryKey != null)
            {
                if (statement.PrimaryKey.Name != null)
                {
                    state.Write(Sym.COMMA);
                    state.Write(Sym.CONSTRAINT_);
                    state.Write(ResolveName(statement.PrimaryKey.Name));
                }
                state.Write(Sym._PRIMARY_KEY);
                state.Write(Sym._op);
                state.Write(string.Join(Sym.COMMA,
                    statement.PrimaryKey.Columns.Select(
                        n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym.ASC : Sym.DESC))));
                state.Write(Sym._cp);

                VisitConflict(statement.PrimaryKey.Conflict, state);
            }

            foreach (var unique in statement.UniqueConstrains)
            {
                state.Write(Sym.COMMA);
                state.Write(Sym.CONSTRAINT_);
                state.Write(ResolveName(unique.Name));
                state.Write(Sym._UNIQUE);
                state.Write(string.Join(Sym.COMMA,
                    unique.Columns.Select(n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym.ASC : Sym.DESC))));
                state.Write(Sym._cp);
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
                state.Write(Sym._UNIQUE);
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
            state.Write(Sym._op);
            state.Write(string.Join(Sym.COMMA,
                statement.Columns.Select(
                    n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym.ASC : Sym.DESC))));
            state.Write(Sym.cp);

            VisitWhere(statement.Where, state);

            state.Write(Sym.sc);
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
            state.Write(Sym.sc);
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
            state.Write(Sym.sc);

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