// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {
        protected override void VisitCreateTableStatement(CreateTableStatement statement)
        {
            State.Write(Symbols.CREATE);
            if (statement.IsTableVariable || statement.IsTemporary)
            {
                State.Write(Symbols.TEMPORARY);
            }
            State.Write(Symbols.TABLE);

            if (statement.CheckIfNotExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.NOT);
                State.Write(Symbols.EXISTS);
            }

            VisitNameToken(statement.Name);

            var separator = "(";
            foreach (var column in statement.Columns)
            {
                State.Write(separator);
                separator = ",";

                State.Write("\"", column.Name, "\"");

                VisitType(column);

                if (column.PrimaryKeyDirection.HasValue)
                {
                    State.Write(Symbols.PRIMARY);
                    State.Write(Symbols.KEY);
                    State.Write(column.PrimaryKeyDirection.Value == Direction.Asc ? Symbols.ASC : Symbols.DESC);
                    VisitConflict(column.PrimaryKeyConflict);

                    if (column.Identity.On)
                    {
                        State.Write(Symbols.AUTOINCREMENT);
                    }
                }

                if (column.Null.HasValue)
                {
                    if (!column.Null.Value)
                    {
                        State.Write(Symbols.NOT);
                    }
                    State.Write(Symbols.NULL);
                    VisitConflict(column.NullConflict);
                }

                if (column.DefaultValue != null)
                {
                    State.Write(Symbols.DEFAULT);
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(column.DefaultValue);
                    State.Write(Symbols.CloseParenthesis);
                }
            }

            if (statement.PrimaryKey != null)
            {
                if (statement.PrimaryKey.Name != null)
                {
                    State.Write(Symbols.Comma);
                    State.Write(Symbols.CONSTRAINT);
                    VisitNameToken(statement.PrimaryKey.Name);
                }
                State.Write(Symbols.PRIMARY);
                State.Write(Symbols.KEY);
                VisitTokenSetInParenthesis(statement.PrimaryKey.Columns);
                VisitConflict(statement.PrimaryKey.Conflict);
            }

            foreach (var unique in statement.UniqueConstrains)
            {
                State.Write(Symbols.Comma);
                State.Write(Symbols.CONSTRAINT);
                VisitNameToken(unique.Name);
                State.Write(Symbols.UNIQUE);
                VisitTokenSetInParenthesis(unique.Columns);
                VisitConflict(unique.Conflict);
            }

            State.Write(Symbols.CloseParenthesis);

            // if indecies are set, create them
            if (statement.Indicies.Count > 0)
            {
                foreach (var createIndexStatement in statement.Indicies)
                {
                    State.WriteStatementTerminator();
                    createIndexStatement.CheckIfNotExists |= statement.CheckIfNotExists;
                    VisitCreateIndexStatement(createIndexStatement);
                }
            }
        }

        protected override void VisitDropTableStatement(DropTableStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.TABLE);
            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name);
        }

        protected override void VisitCreateIndexStatement(CreateIndexStatement statement)
        {
            State.Write(Symbols.CREATE);

            if (statement.Unique)
            {
                State.Write(Symbols.UNIQUE);
            }

            State.Write(Symbols.INDEX);

            if (statement.CheckIfNotExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.NOT);
                State.Write(Symbols.EXISTS);
            }

            VisitToken(statement.Name);

            State.Write(Symbols.ON);

            VisitToken(statement.On);

            // columns
            VisitTokenSetInParenthesis(statement.Columns);
            VisitWhereToken(statement.Where);
        }

        protected override void VisitAlterIndexStatement(AlterIndexStatement statement)
        {
            if (statement.Rebuild)
            {
                State.Write(Symbols.REINDEX);

                if (statement.Name == null)
                {
                    VisitToken(statement.On);
                }
                else
                {
                    VisitToken(Sql.Name(statement.On.FirstPart, statement.Name.LastPart));
                }
            }
        }

        protected override void VisitDropIndexStatement(DropIndexStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.INDEX);

            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }

            if (statement.On != null)
            {
                var name = Sql.Name(statement.On.FirstPart, statement.Name.LastPart);
                VisitToken(name);
            }
            else
            {
                VisitToken(statement.Name);
            }
        }

        protected override void VisitCreateViewStatement(CreateViewStatement statement)
        {
            State.Write(Symbols.CREATE);
            if (statement.IsTemporary)
            {
                State.Write(Symbols.TEMPORARY);
            }
            State.Write(Symbols.VIEW);

            if (statement.CheckIfNotExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.NOT);
                State.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name);
            State.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement);
        }

        protected override void VisitAlterViewStatement(AlterViewStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.VIEW);
            State.Write(Symbols.IF);
            State.Write(Symbols.EXISTS);
            VisitNameToken(statement.Name);
            State.WriteStatementTerminator();

            State.Write(Symbols.CREATE);
            if (statement.IsTemporary)
            {
                State.Write(Symbols.TEMPORARY);
            }
            State.Write(Symbols.VIEW);

            VisitNameToken(statement.Name);
            State.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement);
        }

        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.VIEW);
            State.Write(Symbols.IF);
            State.Write(Symbols.EXISTS);
            VisitNameToken(statement.Name);
            State.WriteStatementTerminator();

            State.Write(Symbols.CREATE);
            if (statement.IsTemporary)
            {
                State.Write(Symbols.TEMPORARY);
            }
            State.Write(Symbols.VIEW);

            VisitNameToken(statement.Name);
            State.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement);
        }

        protected override void VisitDropViewStatement(DropViewStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.VIEW);

            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name);
        }
    }
}