using System;
using System.Collections.Generic;

namespace TTRider.FluidSql.Providers.MySql
{
    internal partial class MySqlVisitor
    {
        //protected override void VisitScalarToken(Scalar token) { throw new NotImplementedException(); }
        //protected override void VisitNameToken(Name token) { throw new NotImplementedException(); }
        protected override void VisitParameterToken(Parameter token) { throw new NotImplementedException(); }
        protected override void VisitSnippetToken(Snippet token) { throw new NotImplementedException(); }
        //protected override void VisitFunctionToken(Function token) { throw new NotImplementedException(); }
        protected override void VisitBitwiseNotToken(BitwiseNotToken token) { throw new NotImplementedException(); }
        //protected override void VisitGroupToken(GroupToken token) { throw new NotImplementedException(); }
        protected override void VisitUnaryMinusToken(UnaryMinusToken token) { throw new NotImplementedException(); }
        protected override void VisitNotToken(NotToken token) { throw new NotImplementedException(); }
        //protected override void VisitIsNullToken(IsNullToken token) { throw new NotImplementedException(); }
        //protected override void VisitIsNotNullToken(IsNotNullToken token) { throw new NotImplementedException(); }
        protected override void VisitExistsToken(ExistsToken token) { throw new NotImplementedException(); }
        protected override void VisitAllToken(AllToken token) { throw new NotImplementedException(); }
        protected override void VisitAnyToken(AnyToken token) { throw new NotImplementedException(); }
        protected override void VisitBetweenToken(BetweenToken token) { throw new NotImplementedException(); }
        //protected override void VisitInToken(InToken token) { throw new NotImplementedException(); }
        protected override void VisitNotInToken(NotInToken token) { throw new NotImplementedException(); }
        protected override void VisitCommentToken(CommentToken token) { throw new NotImplementedException(); }
        protected override void VisitStringifyToken(StringifyToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenMatchedThenDelete(WhenMatchedTokenThenDeleteToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenMatchedThenUpdateSet(WhenMatchedTokenThenUpdateSetToken token) { throw new NotImplementedException(); }
        protected override void VisitWhenNotMatchedThenInsert(WhenNotMatchedTokenThenInsertToken token) { throw new NotImplementedException(); }
        //protected override void VisitOrderToken(Order token) { throw new NotImplementedException(); }
        //protected override void VisitCommonTableExpression(CTEDefinition token) { throw new NotImplementedException(); }

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
    }
}
