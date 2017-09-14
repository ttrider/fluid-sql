// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTRider.FluidSql.Providers.Redshift
{
    public partial class RedshiftVisitor
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

            //VisitIntoToken(statement.Into);

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

            VisitTopToken(statement);

            if (statement.Offset != null)
            {
                State.Write(Symbols.OFFSET);
                VisitToken(statement.Offset);
            }
        }

        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement)
        {
            this.AlterView(statement.Name, statement.DefinitionStatement);
        }

        protected override void VisitAlterViewStatement(AlterViewStatement statement)
        {
            this.AlterView(statement.Name, statement.DefinitionStatement);
        }

        void AlterView(Name tokenName, IStatement definitionStatement)
        {
            DropViewStatement dropViewStatement = Sql.DropView(tokenName, true);
            VisitStatement(dropViewStatement);
            State.WriteStatementTerminator();

            CreateViewStatement createViewStatement = Sql.CreateView(tokenName, definitionStatement);
            VisitStatement(createViewStatement);
            State.WriteStatementTerminator();
        }
    }
}