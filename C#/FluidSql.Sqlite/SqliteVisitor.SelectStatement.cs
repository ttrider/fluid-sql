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
        private void VisitSelect(SelectStatement statement, VisitorState state)
        {
            if (statement.Into != null)
            {
                state.Append(Sym.CREATE_TABLE_);
                VisitNameToken(statement.Into, state);
                state.Append(Sym._AS_);
            }

            state.Append(Sym.SELECT);

            if (statement.Distinct)
            {
                state.Append(Sym._DISTINCT);
            }

            // output columns
            if (statement.Output.Count == 0)
            {
                state.Append(" *");
            }
            else
            {
                VisitTokenSet(statement.Output, state, Sym.SPACE, Sym.COMMA_, String.Empty, true);
            }


            VisitFrom(statement.From, state);

            VisitJoin(statement.Joins, state);

            VisitWhere(statement.Where, state);

            VisitGroupBy(statement.GroupBy, state);

            VisitHaving(statement.Having, state);

            VisitOrderBy(statement.OrderBy, state);

            if (statement.Top != null)
            {
                state.Append(Sym._LIMIT_);
                if (statement.Top.Value.HasValue)
                {
                    state.Append(statement.Top.Value.Value);
                }
                else if (statement.Top.Parameters.Count > 0)
                {
                    foreach (var parameter in statement.Top.Parameters)
                    {
                        state.Parameters.Add(parameter);
                    }
                    state.Append(statement.Top.Parameters[0].Name);
                }
            }

            if (statement.Offset != null)
            {
                state.Append(Sym._OFFSET_);
                VisitToken(statement.Offset, state);
            }
        }
    }
}