using System;
using System.Collections.Generic;
using System.Linq;

namespace TTRider.FluidSql.Providers.MySql
{
    internal partial class MySqlVisitor
    {
        protected override void VisitParameterToken(Parameter token)
        {
            if (token.Value != null)
            {
                VisitValue(token.Value);
            }
            else
            {
                State.Write(token.Name);
            }
        }

        protected override void VisitExitStatement(ExitStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitPerformStatement(PerformStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitAlterSchemaStatement(AlterSchemaStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitAddForeignKeyStatement(AddForeignKeyStatement statement)
        {
            State.Write(Symbols.ALTER);
            State.Write(Symbols.TABLE);
            VisitNameToken(statement.TableName);
            State.Write(Symbols.ADD);
            State.Write(Symbols.CONSTRAINT);
            VisitNameToken(statement.Name);
            State.Write(Symbols.FOREIGN);
            State.Write(Symbols.KEY);
            VisitTokenSetInParenthesis(statement.Columns.Select(c => c.Name));
            State.Write(Symbols.REFERENCES);
            VisitNameToken(statement.References);
            VisitTokenSetInParenthesis(statement.Columns.Select(c => c.ReferencedName));

            if (statement.OnDelete != null)
            {
                State.Write(Symbols.ON);
                State.Write(Symbols.DELETE);
                State.Write(statement.OnDelete.ToString().ToUpper());
            }
            if (statement.OnUpdate != null)
            {
                State.Write(Symbols.ON);
                State.Write(Symbols.UPDATE);
                State.Write(statement.OnUpdate.ToString().ToUpper());
            }
        }

        protected override void VisitDropForeignKeyStatement(DropForeignKeyStatement statement)
        {
            State.Write(Symbols.ALTER);
            State.Write(Symbols.TABLE);
            VisitNameToken(statement.TableName);
            State.Write(Symbols.DROP);
            State.Write(Symbols.FOREIGN);
            State.Write(Symbols.KEY);
            VisitNameToken(statement.Name);
        }

        protected override void VisitWhenMatchedThenDelete(WhenMatchedTokenThenDeleteToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenMatchedThenUpdateSet(WhenMatchedTokenThenUpdateSetToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenNotMatchedThenInsert(WhenNotMatchedTokenThenInsertToken token) { throw new NotImplementedException(); }

        protected void VisitIntoToken(Name intoToken)
        {
            if (intoToken != null)
            {
                State.Write(Symbols.INTO);
                VisitNameToken(intoToken);
            }
        }

        protected void VisitFromToken(RecordsetSourceToken recordset, bool needAlias = true)
        {
            if (recordset != null)
            {
                VisitToken(recordset.Source);
                if (needAlias)
                {
                    VisitAlias(recordset.Alias);
                }
            }
        }

        protected override void VisitCaseToken(CaseToken token)
        {
            State.Write(Symbols.CASE);
            if (token.CaseValueToken != null)
            {
                VisitToken(token.CaseValueToken);
            }

            foreach (var whenCondition in token.WhenConditions)
            {
                State.Write(Symbols.WHEN);
                VisitToken(whenCondition.WhenToken);
                State.Write(Symbols.THEN);
                VisitToken(whenCondition.ThenToken);
            }

            if (token.ElseToken != null)
            {
                State.Write(Symbols.ELSE);
                VisitToken(token.ElseToken);
            }

            State.Write(Symbols.END);
        }

        protected void VisitTopToken(Top top)
        {
            if (top != null)
            {
                State.Write(Symbols.LIMIT);
                if (top.Value != null)
                {
                    VisitToken(top.Value);
                }
                if (top.Percent)
                {
                    State.Write(Symbols.PERCENT);
                }
                if (top.WithTies)
                {
                    State.Write(Symbols.WITH);
                    State.Write(Symbols.TIES);
                }
            }
        }

        protected void VisitUsingToken(List<Name> usingList)
        {
            if (usingList != null)
            {
                var separator = Symbols.USING;
                foreach (var usingItem in usingList)
                {
                    State.Write(separator);
                    VisitToken(usingItem, true);
                    separator = Symbols.Comma;
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
            var counter = 0;

            if (statement.WhenMatched != null)
            {
                foreach (var item in statement.WhenMatched)
                {
                    if (!(item is WhenMatchedTokenThenDeleteToken))
                    {

                        var tempTableSelectStatement = Sql.Select.Output(Sql.Name(targetAlias, targetColumnOn))
                                            .From(targetTable, targetAlias)
                                            .InnerJoin(Sql.Name(sourceTable).As(sourceAlias), Sql.Name(targetAlias, targetColumnOn).IsEqual(Sql.Name(sourceAlias, sourceColumnOn)));


                        var tempAlias = "tmp_" + counter;

                        ExpressionToken tempExpression = (IsEqualsToken)(statement.On);

                        if (item.AndCondition != null)
                        {
                            tempExpression = tempExpression.And(AddPrefixToExpressionToken((ExpressionToken)item.AndCondition, targetAlias));
                        }

                        if (isTop)
                        {
                            tempExpression = tempExpression.And(Sql.Name(targetAlias, targetColumnOn)
                                .In(Sql.Select.Output(sourceColumnOn).From(TopAlias)));
                        }
                        tempTableSelectStatement = tempTableSelectStatement.Where(tempExpression);

                        var createTempTable =
                            Sql.CreateTemporaryTable(tempAlias)
                            .As(tempTableSelectStatement);

                        VisitStatement(createTempTable);
                        State.WriteStatementTerminator();

                        tempUpdateAliases.Add(tempAlias);

                        if (((WhenMatchedTokenThenUpdateSetToken)item).Set.Count != 0)
                        {
                            foreach (var setItem in ((WhenMatchedTokenThenUpdateSetToken)item).Set)
                            {
                                if (setItem.First is Name)
                                {
                                    setItem.First = Sql.Name(targetAlias, ((Name)setItem.First).LastPart);
                                }

                                if (setItem.Second is Name)
                                {
                                    setItem.Second = Sql.Name(sourceAlias, ((Name)setItem.Second).LastPart);
                                }

                            }
                        }

                        var updateTable =
                            Sql.Update(Sql.NameAs(targetTable, targetAlias))
                            .Set(((WhenMatchedTokenThenUpdateSetToken)item).Set)
                            .From(Sql.NameAs(sourceTable, sourceAlias))
                            .Where(tempExpression);

                        counter++;
                        VisitStatement(updateTable);
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
            if (statement.WhenMatched != null)
            {
                foreach (var item in statement.WhenMatched)
                {
                    var isTargetCondition = true;
                    var sourceSelect = Sql.Select.Output(sourceColumnOn).From(sourceTable);

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
                    var tempExpression = Sql.Name(targetAlias, targetColumnOn)
                        .In(sourceSelect);

                    if ((item.AndCondition != null) && isTargetCondition)
                    {
                        tempExpression = tempExpression.And(AddPrefixToExpressionToken((ExpressionToken)item.AndCondition, targetAlias));
                    }

                    if (isTop)
                    {
                        tempExpression = tempExpression.And(Sql.Name(targetAlias, targetColumnOn)
                            .In(Sql.Select.Output(sourceColumnOn).From(TopAlias)));
                    }

                    if ((item is WhenMatchedTokenThenDeleteToken))
                    {
                        var deleteStatement = Sql.Delete.From(Sql.NameAs(targetTable, targetAlias))
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
            if (statement.WhenNotMatchedBySource.Count != 0)
            {
                foreach (var item in statement.WhenNotMatchedBySource)
                {
                    var tempExpression = Sql.Name(targetAlias, targetColumnOn)
                        .NotIn(Sql.Select.Output(sourceColumnOn).From(sourceTable));

                    if (item.AndCondition != null)
                    {
                        tempExpression = tempExpression.And(AddPrefixToExpressionToken((ExpressionToken)item.AndCondition, targetAlias));
                    }

                    if (isTop)
                    {
                        tempExpression = tempExpression.And(Sql.Name(targetAlias, targetColumnOn)
                            .In(Sql.Select.Output(sourceColumnOn).From(TopAlias)));
                    }

                    var whenNotMatchedBySourceDelete =
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
            var tempSourceAlias = "tmp_" + sourceAlias;
            if (statement.WhenNotMatched.Count != 0)
            {
               

                foreach (var item in statement.WhenNotMatched)
                {
                    var tempExpression = Sql.Name(sourceAlias, sourceColumnOn)
                        .NotIn(Sql.Select.Output(targetColumnOn).From(targetTable));

                    if (item.AndCondition != null)
                    {
                        tempExpression = tempExpression.And(AddPrefixToExpressionToken((ExpressionToken)item.AndCondition, sourceAlias));
                    }

                    var createTempTable =
                           Sql.CreateTemporaryTable(tempSourceAlias)
                           .As(Sql.Select.From(Sql.NameAs(sourceTable, sourceAlias)).Where(tempExpression));

                    VisitStatement(createTempTable);
                    State.WriteStatementTerminator();

                    var insertStatement = Sql.Insert.Into(targetTable);

                    if ((((WhenNotMatchedTokenThenInsertToken)item).Values.Count == 0)
                        || (((WhenNotMatchedTokenThenInsertToken)item).Columns.Count == 0)
                        || (((WhenNotMatchedTokenThenInsertToken)item).Columns.Count != ((WhenNotMatchedTokenThenInsertToken)item).Values.Count))
                    {
                        insertStatement.Columns.Add(targetColumnOn);
                        insertStatement.From(Sql.Select.From(tempSourceAlias).Output(sourceColumnOn));
                    }
                    else
                    {
                        foreach (var columnName in ((WhenNotMatchedTokenThenInsertToken)item).Columns)
                        {
                            insertStatement.Columns.Add(Sql.Name(columnName.LastPart));
                        }
                        var fromSelect = Sql.Select.From(tempSourceAlias);
                        foreach (var outputColumn in ((WhenNotMatchedTokenThenInsertToken)item).Values)
                        {
                            fromSelect.Output.Add(Sql.Name(((Name)outputColumn).LastPart));
                        }
                        insertStatement.From(fromSelect);
                    }

                    VisitStatement(insertStatement);
                    State.WriteStatementTerminator();
                    
                    VisitStatement(Sql.DropTable(tempSourceAlias, true));
                    State.WriteStatementTerminator();
                }
            }
        }

        protected void VisitCRUDJoinOnToken(List<Join> joins, bool ifWhereExist = false)
        {
           var separator = !ifWhereExist ? Symbols.WHERE : Symbols.AND;

            foreach (var join in joins)
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
                foreach (var join in joins)
                {
                    State.Write(separator);
                    VisitToken(join.Source);
                    separator = Symbols.Comma;
                }
            }
        }

        protected ExpressionToken AddPrefixToExpressionToken(ExpressionToken token, string prefix)
        {
            if (token is BinaryToken)
            {
                ((BinaryToken)token).First = Sql.Name(prefix, ((Name)((BinaryToken)token).First).LastPart);
            }
            else if (token is UnaryToken)
            {
                ((UnaryToken)token).Token = Sql.Name(prefix, ((Name)((UnaryToken)token).Token).LastPart);
            }
            return token;
        }
    }
}
