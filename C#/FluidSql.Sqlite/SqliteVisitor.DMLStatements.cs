using System;
using System.Globalization;

namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {
        protected override void VisitSelect(SelectStatement statement, VisitorState state)
        {
            if (statement.Into != null)
            {
                state.Write(Symbols.CREATE);
                state.Write(Symbols.TABLE);
                VisitNameToken(statement.Into, state);
                state.Write(Symbols.AS);
            }

            state.Write(Symbols.SELECT);

            if (statement.Distinct)
            {
                state.Write(Symbols.DISTINCT);
            }

            // output columns
            if (statement.Output.Count == 0)
            {
                state.Write(Symbols.Asterisk);
            }
            else
            {
                VisitTokenSet(statement.Output, state, null, Symbols.Comma, null, true);
            }


            VisitFromToken(statement.From, state);

            VisitJoin(statement.Joins, state);

            VisitWhereToken(statement.Where, state);

            VisitGroupByToken(statement.GroupBy, state);

            VisitHavingToken(statement.Having, state);

            VisitOrderByToken(statement.OrderBy, state);

            if (statement.Top != null)
            {
                state.Write(Symbols.LIMIT);
                if (statement.Top.Value.HasValue)
                {
                    state.Write(statement.Top.Value.Value.ToString(CultureInfo.InvariantCulture));
                }
                else if (statement.Top.Parameters.Count > 0)
                {
                    foreach (var parameter in statement.Top.Parameters)
                    {
                        state.Parameters.Add(parameter);
                    }
                    state.Write(statement.Top.Parameters[0].Name);
                }
            }

            if (statement.Offset != null)
            {
                state.Write(Symbols.OFFSET);
                VisitToken(statement.Offset, state);
            }
        }

        protected override void VisitDelete(DeleteStatement statement, VisitorState state)
        {
            state.Write(Symbols.DELETE);

            VisitFromToken(statement.From, state);

            VisitWhereToken(statement.Where, state);

            VisitOrderByToken(statement.OrderBy, state);

            if (statement.Top != null)
            {
                state.Write(Symbols.LIMIT);
                if (statement.Top.Value.HasValue)
                {
                    state.Write(statement.Top.Value.Value.ToString(CultureInfo.InvariantCulture));
                }
                else if (statement.Top.Parameters.Count > 0)
                {
                    foreach (var parameter in statement.Top.Parameters)
                    {
                        state.Parameters.Add(parameter);
                    }
                    state.Write(statement.Top.Parameters[0].Name);
                }
            }

            if (statement.Offset != null)
            {
                state.Write(Symbols.OFFSET);
                VisitToken(statement.Offset, state);
            }
        }

        protected override void VisitUpdate(UpdateStatement statement, VisitorState state)
        {
            state.Write(Symbols.UPDATE);

            if (statement.Conflict.HasValue)
            {
                state.Write(Symbols.OR);
                switch (statement.Conflict.Value)
                {
                    case OnConflict.Abort:
                        state.Write(Symbols.ABORT);
                        break;
                    case OnConflict.Fail:
                        state.Write(Symbols.FAIL);
                        break;
                    case OnConflict.Ignore:
                        state.Write(Symbols.IGNORE);
                        break;
                    case OnConflict.Replace:
                        state.Write(Symbols.REPLACE);
                        break;
                    case OnConflict.Rollback:
                        state.Write(Symbols.ROLLBACK);
                        break;
                }
            }

            VisitNameToken(statement.Target, state);

            state.Write(Symbols.SET);

            VisitTokenSet(statement.Set, state, null, Symbols.Comma, null);

            VisitWhereToken(statement.Where, state);
        }

        protected override void VisitInsert(InsertStatement statement, VisitorState state)
        {
            state.Write(Symbols.INSERT);

            if (statement.Conflict.HasValue)
            {
                state.Write(Symbols.OR);
                switch (statement.Conflict.Value)
                {
                    case OnConflict.Abort:
                        state.Write(Symbols.ABORT);
                        break;
                    case OnConflict.Fail:
                        state.Write(Symbols.FAIL);
                        break;
                    case OnConflict.Ignore:
                        state.Write(Symbols.IGNORE);
                        break;
                    case OnConflict.Replace:
                        state.Write(Symbols.REPLACE);
                        break;
                    case OnConflict.Rollback:
                        state.Write(Symbols.ROLLBACK);
                        break;
                }
            }
            state.Write(Symbols.INTO);

            VisitNameToken(statement.Into, state);

            VisitTokenSet(statement.Columns, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis, true);

            if (statement.DefaultValues)
            {
                state.Write(Symbols.DEFAULT);
                state.Write(Symbols.VALUES);

            }
            else if (statement.Values.Count > 0)
            {
                var separator = Symbols.VALUES;
                foreach (var valuesSet in statement.Values)
                {
                    state.Write(separator);
                    separator = Symbols.Comma;

                    VisitTokenSet(valuesSet, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
                }
            }
            else if (statement.From != null)
            {
                VisitStatement(statement.From, state);
            }
        }

        protected override void VisitMerge(MergeStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }
    }
}