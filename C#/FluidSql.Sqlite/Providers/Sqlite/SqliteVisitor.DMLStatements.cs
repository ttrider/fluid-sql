// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System;

namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {
        protected override void VisitSelect(SelectStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions, true);

            if (statement.Into != null)
            {
                State.Write(Symbols.CREATE);
                State.Write(Symbols.TABLE);
                VisitNameToken(statement.Into);
                State.Write(Symbols.AS);
            }

            State.Write(Symbols.SELECT);

            if (statement.Distinct)
            {
                State.Write(Symbols.DISTINCT);
            }

            // output columns
            if (statement.Output.Count == 0)
            {
                State.Write(Symbols.Asterisk);
            }
            else
            {
                VisitAliasedTokenSet(statement.Output, null, Symbols.Comma, null);
            }


            if (statement.From.Count > 0)
            {
                State.Write(Symbols.FROM);
                VisitFromToken(statement.From);
            }

            VisitJoin(statement.Joins);

            VisitWhereToken(statement.Where);

            VisitGroupByToken(statement.GroupBy);

            VisitHavingToken(statement.Having);

            VisitOrderByToken(statement.OrderBy);

            if (statement.Top != null)
            {
                State.Write(Symbols.LIMIT);

                if (statement.Top.Value != null)
                {
                    VisitToken(statement.Top.Value);
                }
            }

            if (statement.Offset != null)
            {
                State.Write(Symbols.OFFSET);
                VisitToken(statement.Offset);
            }
        }

        protected override void VisitDelete(DeleteStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions, true);

            State.Write(Symbols.DELETE);
            State.Write(Symbols.FROM);
            VisitFromToken(statement.RecordsetSource);

            VisitWhereToken(statement.Where);

            VisitOrderByToken(statement.OrderBy);

            if (statement.Top != null)
            {
                State.Write(Symbols.LIMIT);
                if (statement.Top.Value != null)
                {
                    VisitToken(statement.Top.Value);
                }
            }

            if (statement.Offset != null)
            {
                State.Write(Symbols.OFFSET);
                VisitToken(statement.Offset);
            }
        }

        protected override void VisitUpdate(UpdateStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions, true);

            State.Write(Symbols.UPDATE);

            if (statement.Conflict.HasValue)
            {
                State.Write(Symbols.OR);
                switch (statement.Conflict.Value)
                {
                    case OnConflict.Abort:
                        State.Write(Symbols.ABORT);
                        break;
                    case OnConflict.Fail:
                        State.Write(Symbols.FAIL);
                        break;
                    case OnConflict.Ignore:
                        State.Write(Symbols.IGNORE);
                        break;
                    case OnConflict.Replace:
                        State.Write(Symbols.REPLACE);
                        break;
                    case OnConflict.Rollback:
                        State.Write(Symbols.ROLLBACK);
                        break;
                }
            }

            VisitNameToken(statement.Target);

            State.Write(Symbols.SET);

            VisitTokenSet(statement.Set);

            VisitWhereToken(statement.Where);
        }

        protected override void VisitInsert(InsertStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions, true);

            State.Write(Symbols.INSERT);

            if (statement.Conflict.HasValue)
            {
                State.Write(Symbols.OR);
                switch (statement.Conflict.Value)
                {
                    case OnConflict.Abort:
                        State.Write(Symbols.ABORT);
                        break;
                    case OnConflict.Fail:
                        State.Write(Symbols.FAIL);
                        break;
                    case OnConflict.Ignore:
                        State.Write(Symbols.IGNORE);
                        break;
                    case OnConflict.Replace:
                        State.Write(Symbols.REPLACE);
                        break;
                    case OnConflict.Rollback:
                        State.Write(Symbols.ROLLBACK);
                        break;
                }
            }
            State.Write(Symbols.INTO);

            VisitNameToken(statement.Into);

            VisitAliasedTokenSet(statement.Columns, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);

            if (statement.DefaultValues)
            {
                State.Write(Symbols.DEFAULT);
                State.Write(Symbols.VALUES);
            }
            else if (statement.Values.Count > 0)
            {
                var separator = Symbols.VALUES;
                foreach (var valuesSet in statement.Values)
                {
                    State.Write(separator);
                    separator = Symbols.Comma;

                    VisitTokenSetInParenthesis(valuesSet);
                }
            }
            else if (statement.From != null)
            {
                VisitStatement(statement.From);
            }
        }

        protected override void VisitMerge(MergeStatement statement)
        {
            throw new NotImplementedException();
        }
    }
}