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
            state.Append(statement.IsTableVariable || statement.IsTemporary ? Sym.CREATE_TEMP_TABLE_ : Sym.CREATE_TABLE_);

            if (statement.CheckIfNotExists)
            {
                state.Append(Sym.IF_NOT_EXISTS_);
            }

            state.Append(ResolveName(statement.Name));

            var separator = " (";
            foreach (var column in statement.Columns)
            {
                state.Append(separator);
                separator = ", ";

                state.Append("\"");
                state.Append(column.Name);
                state.Append("\"");

                VisitType(column, state);

                if (column.PrimaryKeyDirection.HasValue)
                {
                    state.Append(Sym._PRIMARY_KEY);
                    state.Append(column.PrimaryKeyDirection.Value == Direction.Asc ? Sym._ASC : Sym._DESC);
                    VisitConflict(column.PrimaryKeyConflict, state);

                    if (column.Identity.On)
                    {
                        state.Append(Sym._AUTOINCREMENT);
                    }
                }

                if (column.Null.HasValue)
                {
                    state.Append(column.Null.Value ? Sym._NULL : Sym._NOT_NULL);
                    VisitConflict(column.NullConflict, state);
                }

                if (column.DefaultValue != null)
                {
                    state.Append(Sym._DEFAULT_op);
                    VisitToken(column.DefaultValue, state);
                    state.Append(Sym._cp);
                }
            }

            if (statement.PrimaryKey != null)
            {
                if (statement.PrimaryKey.Name != null)
                {
                    state.Append(Sym.COMMA_);
                    state.Append(Sym.CONSTRAINT_);
                    state.Append(ResolveName(statement.PrimaryKey.Name));
                }
                state.Append(Sym._PRIMARY_KEY);
                state.Append(Sym._op);
                state.Append(string.Join(Sym.COMMA_,
                    statement.PrimaryKey.Columns.Select(
                        n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym._ASC : Sym._DESC))));
                state.Append(Sym._cp);

                VisitConflict(statement.PrimaryKey.Conflict, state);
            }

            foreach (var unique in statement.UniqueConstrains)
            {
                state.Append(Sym.COMMA_);
                state.Append(Sym.CONSTRAINT_);
                state.Append(ResolveName(unique.Name));
                state.Append(Sym._UNIQUE);
                state.Append(string.Join(Sym.COMMA_,
                    unique.Columns.Select(n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym._ASC : Sym._DESC))));
                state.Append(Sym._cp);
                VisitConflict(unique.Conflict, state);
            }


            state.Append(Sym.cpsc);

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
            state.Append(Sym.DROP_TABLE_);
            if (statement.CheckExists)
            {
                state.Append(Sym.IF_EXISTS_);
            }
            state.Append(ResolveName(statement.Name));
        }

        private void VisitCreateIndexStatement(CreateIndexStatement statement, VisitorState state)
        {
            state.Append(Sym.CREATE);

            if (statement.Unique)
            {
                state.Append(Sym._UNIQUE);
            }

            state.Append(Sym._INDEX_);

            if (statement.CheckIfNotExists)
            {
                state.Append(Sym.IF_NOT_EXISTS_);
            }

            VisitToken(statement.Name, state);

            state.Append(Sym._ON_);

            VisitToken(statement.On, state);

            // columns
            state.Append(Sym._op);
            state.Append(string.Join(Sym.COMMA_,
                statement.Columns.Select(
                    n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym._ASC : Sym._DESC))));
            state.Append(Sym.cp);

            VisitWhere(statement.Where, state);

            state.Append(Sym.sc);
        }

        private void VisitAlterIndexStatement(AlterIndexStatement statement, VisitorState state)
        {
            if (statement.Rebuild)
            {
                state.Append(Sym.REINDEX_);

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
            state.Append(Sym.DROP_INDEX_);

            if (statement.CheckExists)
            {
                state.Append(Sym.IF_EXISTS_);
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
            state.Append(statement.IsTemporary ? Sym.CREATE_TEMPORARY_VIEW_ : Sym.CREATE_VIEW_);

            if (statement.CheckIfNotExists)
            {
                state.Append(Sym.IF_NOT_EXISTS_);
            }
            state.Append(ResolveName(statement.Name));
            state.Append(Sym._AS_);
            VisitStatement(statement.DefinitionStatement, state);
        }

        private void VisitAlterViewStatement(AlterViewStatement statement, VisitorState state)
        {
            state.Append(Sym.DROP_VIEW_);
            state.Append(Sym.IF_EXISTS_);
            state.Append(ResolveName(statement.Name));
            state.Append(Sym.sc);
            state.Append(statement.IsTemporary ? Sym.CREATE_TEMPORARY_VIEW_ : Sym.CREATE_VIEW_);

            state.Append(ResolveName(statement.Name));
            state.Append(Sym._AS_);
            VisitStatement(statement.DefinitionStatement, state);
        }

        private void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement, VisitorState state)
        {
            state.Append(Sym.DROP_VIEW_);
            state.Append(Sym.IF_EXISTS_);
            state.Append(ResolveName(statement.Name));
            state.Append(Sym.sc);

            state.Append(statement.IsTemporary ? Sym.CREATE_TEMPORARY_VIEW_ : Sym.CREATE_VIEW_);

            state.Append(ResolveName(statement.Name));
            state.Append(Sym._AS_);
            VisitStatement(statement.DefinitionStatement, state);
        }
        private void VisitDropViewStatement(DropViewStatement statement, VisitorState state)
        {
            state.Append(Sym.DROP_VIEW_);
            if (statement.CheckExists)
            {
                state.Append(Sym.IF_EXISTS_);
            }
            state.Append(ResolveName(statement.Name));
        }


        private void VisitBeginTransaction(BeginTransactionStatement statement, VisitorState state)
        {
            if (statement.Type.HasValue)
            {
                switch (statement.Type.Value)
                {
                    case TransactionType.Deferred:
                        state.Append(Sym.BEGIN_TRANSACTION);
                        break;
                    case TransactionType.Immediate:
                        state.Append(Sym.BEGIN_TRANSACTION);
                        break;
                    case TransactionType.Exclusive:
                        state.Append(Sym.BEGIN_TRANSACTION);
                        break;
                }
            }
            else
            {
                state.Append(Sym.BEGIN_TRANSACTION);
            }
        }

    }
}