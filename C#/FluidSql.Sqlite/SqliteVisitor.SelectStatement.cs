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
                state.Write(Sym.CREATE_TABLE);
                VisitNameToken(statement.Into, state);
                state.Write(Sym.AS);
            }

            state.Write(Sym.SELECT);

            if (statement.Distinct)
            {
                state.Write(Sym.DISTINCT);
            }

            // output columns
            if (statement.Output.Count == 0)
            {
                state.Write(Sym.asterisk);
            }
            else
            {
                VisitTokenSet(statement.Output, state, null, Sym.COMMA, null, true);
            }


            VisitFrom(statement.From, state);

            VisitJoin(statement.Joins, state);

            VisitWhere(statement.Where, state);

            VisitGroupBy(statement.GroupBy, state);

            VisitHaving(statement.Having, state);

            VisitOrderBy(statement.OrderBy, state);

            if (statement.Top != null)
            {
                state.Write(Sym.LIMIT);
                if (statement.Top.Value.HasValue)
                {
                    state.Write(statement.Top.Value.Value.ToString());
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
                state.Write(Sym.OFFSET);
                VisitToken(statement.Offset, state);
            }
        }
    }
}