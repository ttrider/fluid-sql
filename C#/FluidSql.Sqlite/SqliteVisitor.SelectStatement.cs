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
    }
}