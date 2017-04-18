// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTRider.FluidSql.Providers.Redshift
{
    public partial class RedshiftSQLVisitor
    {
        protected override void VisitCreateIndexStatement(CreateIndexStatement statement)
        {            
        }

        protected override void VisitAlterIndexStatement(AlterIndexStatement statement)
        {         
        }

        protected override void VisitDropIndexStatement(DropIndexStatement statement)
        {         
        }

        protected override void VisitSelect(SelectStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions, true);

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
                VisitAliasedTokenSet(statement.Output, (string)null, Symbols.Comma, null);
            }

            VisitIntoToken(statement.Into);

            if (statement.From.Count > 0)
            {
                State.Write(Symbols.FROM);
                VisitFromToken(statement.From);
            }

            VisitJoin(statement.Joins);

            VisitWhereToken(statement.Where);

            VisitTopToken(statement);

            if (statement.Offset != null)
            {
                State.Write(Symbols.OFFSET);
                VisitToken(statement.Offset);
            }
        }
    }
}