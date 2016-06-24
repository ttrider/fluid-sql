using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.PostgreSQL
{
    internal partial class PostgreSQLVisitor
    {
        protected override void VisitSnippetToken(Snippet token) { throw new NotImplementedException(); }
        protected override void VisitBitwiseNotToken(BitwiseNotToken token) { throw new NotImplementedException(); }
        protected override void VisitUnaryMinusToken(UnaryMinusToken token) { throw new NotImplementedException(); }
        protected override void VisitNotToken(NotToken token) { throw new NotImplementedException(); }
        //protected override void VisitIsNullToken(IsNullToken token) { throw new NotImplementedException(); }
        //protected override void VisitIsNotNullToken(IsNotNullToken token) { throw new NotImplementedException(); }
        protected override void VisitExistsToken(ExistsToken token) { throw new NotImplementedException(); }
        protected override void VisitAllToken(AllToken token) { throw new NotImplementedException(); }
        protected override void VisitAnyToken(AnyToken token) { throw new NotImplementedException(); }
        protected override void VisitBetweenToken(BetweenToken token) { throw new NotImplementedException(); }
        protected override void VisitNotInToken(NotInToken token) { throw new NotImplementedException(); }
        protected override void VisitCommentToken(CommentToken token) { throw new NotImplementedException(); }
        protected override void VisitStringifyToken(StringifyToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenMatchedThenDelete(WhenMatchedTokenThenDeleteToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenMatchedThenUpdateSet(WhenMatchedTokenThenUpdateSetToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenNotMatchedThenInsert(WhenNotMatchedTokenThenInsertToken token) { throw new NotImplementedException(); }
        //protected override void VisitCommonTableExpression(CTEDefinition token) { throw new NotImplementedException(); }
        protected override void VisitCaseToken(CaseToken token) { throw new NotImplementedException(); }

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
                foreach(Name usingItem in usingList)
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
                        if (String.IsNullOrEmpty(returningItem.Alias))
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
                if(outputInto != null)
                {
                    State.Write(Symbols.INTO);
                    VisitToken(Sql.Name(outputInto.FirstPart.Replace(Symbols.At, String.Empty)), false);
                }
            }
        }
        protected void VisitTopToken(RecordsetSourceToken recordsetSource, Top top, bool ifWhereExist = false)
        {
            Name techId = new Name();
            if(!ifWhereExist)
            {
                State.Write(Symbols.WHERE);
            }
            else
            {
                State.Write(Symbols.AND);
            }
            if(!String.IsNullOrEmpty(recordsetSource.Alias))
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
            if(intoToken != null)
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

        private readonly string[] PredefinedTemporaryTables =
        {
            "DELETED",       
            "INSERTED",     
            "UPDATED"      
        };
    }
}
