using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.PostgreBased
{
    public partial class PostgreBasedSQLVisitor
    {
        //protected override void VisitSnippetToken(Snippet token) { throw new NotImplementedException(); }
        //protected override void VisitBitwiseNotToken(BitwiseNotToken token) { throw new NotImplementedException(); }
        //protected override void VisitUnaryMinusToken(UnaryMinusToken token) { throw new NotImplementedException(); }
        //protected override void VisitNotToken(NotToken token) { throw new NotImplementedException(); }
        //protected override void VisitIsNullToken(IsNullToken token) { throw new NotImplementedException(); }
        //protected override void VisitIsNotNullToken(IsNotNullToken token) { throw new NotImplementedException(); }
        //protected override void VisitExistsToken(ExistsToken token) { throw new NotImplementedException(); }
        //protected override void VisitAllToken(AllToken token) { throw new NotImplementedException(); }
        //protected override void VisitAnyToken(AnyToken token) { throw new NotImplementedException(); }
        //protected override void VisitBetweenToken(BetweenToken token) { throw new NotImplementedException(); }
        //protected override void VisitNotInToken(NotInToken token) { throw new NotImplementedException(); }
        //protected override void VisitCommentToken(CommentToken token) { throw new NotImplementedException(); }
        //protected override void VisitStringifyToken(StringifyToken token) { throw new NotImplementedException(); }

        protected override void VisitWhenMatchedThenUpdateSet(WhenMatchedTokenThenUpdateSetToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenNotMatchedThenInsert(WhenNotMatchedTokenThenInsertToken token) { throw new NotImplementedException(); }
        //protected override void VisitCommonTableExpression(CTEDefinition token) { throw new NotImplementedException(); }
        //protected override void VisitCaseToken(CaseToken token) { throw new NotImplementedException(); }

        protected void VisitWhereCurrentOfToken(Name cursorName)
        {
            if (cursorName != null)
            {
                State.Write(Symbols.WHERE);
                State.Write(Symbols.CURRENT);
                State.Write(Symbols.OF);

                VisitNameToken(cursorName);
            }
        }

        protected void VisitUsingToken(List<Name> usingList)
        {
            if (usingList != null)
            {
                var separator = Symbols.USING;
                foreach (Name usingItem in usingList)
                {
                    State.Write(separator);
                    VisitToken(usingItem, true);
                    separator = Symbols.Comma;
                }
            }
        }

        protected void VisitCRUDJoinOnToken(List<Join> joins, bool ifWhereExist = false)
        {
            string separator = string.Empty;

            if (!ifWhereExist)
            {
                separator = Symbols.WHERE;
            }
            else
            {
                separator = Symbols.AND;
            }

            foreach (Join join in joins)
            {
                if (join.On != null)
                {
                    State.Write(separator);
                    VisitToken(@join.On);
                    separator = Symbols.AND;
                }
            }
        }

        protected void VisitCRUDJoinToken(List<Join> joins, bool isUpdate = false)
        {
            if (joins.Count > 0)
            {
                var separator = (isUpdate) ? Symbols.FROM : Symbols.USING;
                foreach (Join join in joins)
                {
                    State.Write(separator);
                    VisitToken(join.Source);
                    separator = Symbols.Comma;
                }
            }
        }

        protected void VisitReturningToken(List<ExpressionToken> returningList, Name outputInto = null)
        {
            if (returningList != null)
            {
                var separator = Symbols.RETURNING;
                foreach (Name returningItem in returningList)
                {
                    State.Write(separator);

                    if (PredefinedTemporaryTables.Contains(returningItem.FirstPart.ToUpper()))
                    {
                        if (String.IsNullOrWhiteSpace(returningItem.Alias))
                        {
                            VisitToken(Sql.Name(returningItem.LastPart));
                        }
                        else
                        {
                            VisitToken(Sql.NameAs(returningItem.LastPart, returningItem.Alias), true);
                        }
                    }
                    else
                    {
                        VisitToken(returningItem, true);
                    }
                    separator = Symbols.Comma;
                }
                if (outputInto != null)
                {
                    State.Write(Symbols.INTO);
                    VisitToken(Sql.Name(outputInto.FirstPart.Replace(Symbols.At, String.Empty)), false);
                }
            }
        }

        protected void VisitTopToken(RecordsetSourceToken recordsetSource, Top top, bool ifWhereExist = false)
        {
            Name techId = new Name();
            if (!ifWhereExist)
            {
                State.Write(Symbols.WHERE);
            }
            else
            {
                State.Write(Symbols.AND);
            }
            if (!String.IsNullOrWhiteSpace(recordsetSource.Alias))
            {
                VisitToken(Sql.Name(recordsetSource.Alias, Symbols.TechId));
            }
            else
            {
                VisitToken(Sql.Name(Symbols.TechId));
            }
            State.Write(Symbols.EqualsVal);
            State.Write(Symbols.ANY);
            State.Write(Symbols.OpenParenthesis);
            State.Write(Symbols.ARRAY);

            if (top.IntValue != -1)
            {
                VisitToken(Sql.Select.Output(Sql.Name(Symbols.TechId)).From(recordsetSource).Limit(top.IntValue, top.Percent));
            }

            State.Write(Symbols.CloseParenthesis);

        }

        protected void VisitIntoToken(Name intoToken)
        {
            if (intoToken != null)
            {
                State.Write(Symbols.INTO);
                VisitNameToken(intoToken);
            }
        }

        protected void VisitFetchNextToken(Top fetchToken)
        {
            if (fetchToken != null)
            {
                State.Write(Symbols.FETCH);
                State.Write(Symbols.NEXT);
                VisitToken(fetchToken);
                State.Write(Symbols.ROWS);
                State.Write(Symbols.ONLY);
            }
        }

        protected void VisitTopToken(SelectStatement statement)
        {
            if (statement.Top != null)
            {
                State.Write(Symbols.LIMIT);

                if (statement.Top.Value != null)
                {
                    if (statement.Top.Percent)
                    {
                        State.Write(Symbols.OpenParenthesis);
                        State.Write(Symbols.SELECT);
                        VisitToken(Sql.Select.Output(Sql.Function("COUNT", Sql.Star())).From(statement.From));
                        //calculate percent value - * percent / 100
                        State.Write(Symbols.MultiplyVal);
                        VisitToken(Sql.Scalar(statement.Top.Value));
                        State.Write(Symbols.DivideVal);
                        State.Write("100");

                        State.Write(Symbols.CloseParenthesis);
                    }
                    else
                    {
                        VisitToken(statement.Top.Value);
                    }
                }
            }
        }

        protected void VisitWhenMatchedUpdateToken(MergeStatement statement
            , IList<string> tempUpdateAliases
            , string targetAlias
            , string targetTable
            , string targetColumnOn
            , string sourceAlias
            , string sourceTable
            , string sourceColumnOn
            , bool isTop)
        {
            int counter = 0;
            ExpressionToken tempExpression = new ExpressionToken();

            if (statement.WhenMatched != null)
            {
                foreach (WhenMatchedToken item in statement.WhenMatched)
                {
                    if (!(item is WhenMatchedTokenThenDeleteToken))
                    {
                        IList<CTEDefinition> updatedList = new List<CTEDefinition>();
                        string tempAlias = "tmp_" + counter;

                        tempExpression = (IsEqualsToken)(statement.On);

                        if (((WhenMatchedTokenThenUpdateSetToken)item).Set.Count != 0)
                        {
                            foreach (BinaryEqualToken setItem in ((WhenMatchedTokenThenUpdateSetToken)item).Set)
                            {
                                if (setItem.First is Name)
                                {
                                    setItem.First = Sql.Name(((Name)setItem.First).LastPart);
                                }

                                if (setItem.Second is Name)
                                {
                                    setItem.Second = Sql.Name(sourceAlias, ((Name)setItem.Second).LastPart);
                                }

                            }
                        }

                        if (item.AndCondition != null)
                        {
                            if (item.AndCondition is BinaryToken)
                            {
                                ((BinaryToken)item.AndCondition).First = Sql.Name(targetAlias, ((Name)((BinaryToken)item.AndCondition).First).LastPart);
                            }
                            else if (item.AndCondition is UnaryToken)
                            {
                                ((UnaryToken)item.AndCondition).Token = Sql.Name(targetAlias, ((Name)((UnaryToken)item.AndCondition).Token).LastPart);
                            }
                            tempExpression = tempExpression.And((ExpressionToken)item.AndCondition);
                        }

                        if (isTop)
                        {
                            tempExpression = tempExpression.And(Sql.Name(targetAlias, targetColumnOn)
                                .In(Sql.Select.Output(sourceColumnOn).From(TopAlias)));
                        }

                        CreateTableStatement createTempTable =
                            Sql.CreateTemporaryTable(tempAlias)
                            .As(Sql.With("updated").As(
                            Sql.Update(Sql.NameAs(targetTable, targetAlias))
                            .Set(((WhenMatchedTokenThenUpdateSetToken)item).Set)
                            .From(Sql.NameAs(sourceTable, sourceAlias))
                            .Where(tempExpression)
                            .Output(Sql.Name(targetAlias, targetColumnOn)))
                            .Select().From("updated").Output(targetColumnOn));

                        VisitStatement(createTempTable);
                        State.WriteStatementTerminator();

                        tempUpdateAliases.Add(tempAlias);

                        counter++;
                        State.WriteStatementTerminator();
                    }
                }
            }
        }

        protected void VisitWhenMatchedDeleteToken(MergeStatement statement
            , string targetAlias
            , string targetTable
            , string targetColumnOn
            , string sourceAlias
            , string sourceTable
            , string sourceColumnOn
            , bool isTop)
        {
            ExpressionToken tempExpression = new ExpressionToken();
            if (statement.WhenMatched != null)
            {
                foreach (WhenMatchedToken item in statement.WhenMatched)
                {
                    bool isTargetCondition = true;
                    SelectStatement sourceSelect = Sql.Select.Output(sourceColumnOn).From(sourceTable);

                    if (item.AndCondition != null)
                    {
                        if ((item.AndCondition is BinaryToken)
                            && (
                                (((Name)((BinaryToken)item.AndCondition).First).FirstPart == sourceAlias)
                                || ((Name)((BinaryToken)item.AndCondition).First).FirstPart == sourceTable)
                            )
                        {
                            ((BinaryToken)item.AndCondition).First = Sql.Name(sourceTable, ((Name)((BinaryToken)item.AndCondition).First).LastPart);
                            sourceSelect.Where((BinaryToken)item.AndCondition);
                            isTargetCondition = false;
                        }

                        else if ((item.AndCondition is UnaryToken)
                             && (
                                (((Name)((UnaryToken)item.AndCondition).Token).FirstPart == sourceAlias)
                                || (((Name)((UnaryToken)item.AndCondition).Token).FirstPart == sourceTable)
                                )
                             )
                        {
                            ((UnaryToken)item.AndCondition).Token = Sql.Name(sourceTable, ((Name)((UnaryToken)item.AndCondition).Token).LastPart);
                            sourceSelect.Where((UnaryToken)item.AndCondition);
                            isTargetCondition = false;
                        }
                    }
                    tempExpression = Sql.Name(targetAlias, targetColumnOn)
                            .In(sourceSelect);

                    if ((item.AndCondition != null) && isTargetCondition)
                    {
                        if (item.AndCondition is BinaryToken)
                        {
                            ((BinaryToken)item.AndCondition).First = Sql.Name(targetAlias, ((Name)((BinaryToken)item.AndCondition).First).LastPart);
                        }
                        else if (item.AndCondition is UnaryToken)
                        {
                            ((UnaryToken)item.AndCondition).Token = Sql.Name(targetAlias, ((Name)((UnaryToken)item.AndCondition).Token).LastPart);
                        }
                        tempExpression = tempExpression.And((ExpressionToken)item.AndCondition);
                    }

                    if (isTop)
                    {
                        tempExpression = tempExpression.And(Sql.Name(targetAlias, targetColumnOn)
                            .In(Sql.Select.Output(sourceColumnOn).From(TopAlias)));
                    }

                    if ((item is WhenMatchedTokenThenDeleteToken))
                    {
                        DeleteStatement deleteStatement = Sql.Delete.From(Sql.NameAs(targetTable, targetAlias))
                                        .Where(tempExpression);
                        VisitStatement(deleteStatement);
                        State.WriteStatementTerminator();
                    }
                }
            }
        }

        protected void VisitWhenNotMatchedBySourceToken(MergeStatement statement
            , string targetAlias
            , string targetTable
            , string targetColumnOn
            , string sourceAlias
            , string sourceTable
            , string sourceColumnOn
            , bool isTop
            )
        {
            ExpressionToken tempExpression = new ExpressionToken();

            if (statement.WhenNotMatchedBySource.Count != 0)
            {
                foreach (WhenMatchedToken item in statement.WhenNotMatchedBySource)
                {
                    tempExpression = Sql.Name(targetAlias, targetColumnOn)
                            .NotIn(Sql.Select.Output(sourceColumnOn).From(sourceTable));

                    if (item.AndCondition != null)
                    {
                        if (item.AndCondition is BinaryToken)
                        {
                            ((BinaryToken)item.AndCondition).First = Sql.Name(targetAlias, ((Name)((BinaryToken)item.AndCondition).First).LastPart);
                        }
                        else if (item.AndCondition is UnaryToken)
                        {
                            ((UnaryToken)item.AndCondition).Token = Sql.Name(targetAlias, ((Name)((UnaryToken)item.AndCondition).Token).LastPart);
                        }
                        tempExpression = tempExpression.And((ExpressionToken)item.AndCondition);
                    }

                    if (isTop)
                    {
                        tempExpression = tempExpression.And(Sql.Name(targetAlias, targetColumnOn)
                            .In(Sql.Select.Output(sourceColumnOn).From(TopAlias)));
                    }

                    DeleteStatement whenNotMatchedBySourceDelete =
                    Sql.Delete.From(Sql.NameAs(targetTable, targetAlias))
                    .Where(tempExpression);

                    VisitStatement(whenNotMatchedBySourceDelete);
                    State.WriteStatementTerminator();
                }
            }
        }

        protected void VisitWhenNotMatchedThenInsertToken(MergeStatement statement
            , IList<string> tempUpdateAliases
            , string targetAlias
            , string targetColumnOn
            , string targetTable
            , string sourceAlias
            , string sourceTable
            , string sourceColumnOn)
        {
            ExpressionToken tempExpression = new ExpressionToken();
            if (statement.WhenNotMatched.Count != 0)
            {
                int counter = 0;

                foreach (WhenMatchedToken item in statement.WhenNotMatched)
                {
                    IList<CTEDefinition> tmpSourceList = new List<CTEDefinition>();

                    tempExpression = Sql.Name(sourceAlias, sourceColumnOn)
                    .NotIn(Sql.Select.Output(targetColumnOn).From(targetTable));

                    if (item.AndCondition != null)
                    {
                        if (item.AndCondition is BinaryToken)
                        {
                            ((BinaryToken)item.AndCondition).First = Sql.Name(sourceAlias, ((Name)((BinaryToken)item.AndCondition).First).LastPart);
                        }
                        else if (item.AndCondition is UnaryToken)
                        {
                            ((UnaryToken)item.AndCondition).Token = Sql.Name(sourceAlias, ((Name)((UnaryToken)item.AndCondition).Token).LastPart);
                        }
                        tempExpression = tempExpression.And((ExpressionToken)item.AndCondition);
                    }

                    CTEDefinition source = Sql.With(sourceAlias).As(
                            Sql.Select.From(Sql.NameAs(sourceTable, sourceAlias)).Where(tempExpression));
                    tmpSourceList.Add(source);


                    InsertStatement insertStatement = Sql.Insert.Into(targetTable);

                    if ((((WhenNotMatchedTokenThenInsertToken)item).Values.Count == 0)
                        || (((WhenNotMatchedTokenThenInsertToken)item).Columns.Count == 0)
                        || (((WhenNotMatchedTokenThenInsertToken)item).Columns.Count != ((WhenNotMatchedTokenThenInsertToken)item).Values.Count))
                    {
                        insertStatement.Columns.Add(targetColumnOn);
                        insertStatement.From(Sql.Select.From(sourceAlias).Output(sourceColumnOn));
                    }
                    else
                    {
                        foreach (Name columnName in ((WhenNotMatchedTokenThenInsertToken)item).Columns)
                        {
                            insertStatement.Columns.Add(Sql.Name(columnName.LastPart));
                        }
                        SelectStatement fromSelect = Sql.Select.From(sourceAlias);
                        foreach (Token outputColumn in ((WhenNotMatchedTokenThenInsertToken)item).Values)
                        {
                            fromSelect.Output.Add(Sql.Name(((Name)outputColumn).LastPart));
                        }
                        insertStatement.From(fromSelect);
                    }

                    VisitCommonTableExpressions(tmpSourceList);
                    VisitStatement(insertStatement);

                    State.WriteStatementTerminator();

                    InsertStatement whenNotMatchedInsert =
                    Sql.Insert.Into(Sql.Name(targetTable))
                    .From(Sql.Select.From(Sql.Name(sourceAlias))
                    .Where(tempExpression));
                    
                    counter++;
                }
            }
        }

        private readonly string[] PredefinedTemporaryTables =
        {
            "DELETED",
            "INSERTED",
            "UPDATED"
        };
    }
}
