using System;
using System.Collections.Generic;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {

        protected abstract void VisitStringifyToken(StringifyToken token, VisitorState state);
        protected abstract void VisitWhenMatchedThenDelete(WhenMatchedThenDelete token, VisitorState state);
        protected abstract void VisitWhenMatchedThenUpdateSet(WhenMatchedThenUpdateSet token, VisitorState state);
        protected abstract void VisitWhenNotMatchedThenInsert(WhenNotMatchedThenInsert token, VisitorState state);





        protected virtual void VisitScalarToken(Scalar token, VisitorState state)
        {
            var value = token.Value;
            if (value == null) return;

            if (value is DBNull)
            {
                state.Write(Sym.NULL);
            }
            else if ((value is Boolean)
                || (value is SByte)
                || (value is Byte)
                || (value is Int16)
                || (value is UInt16)
                || (value is Int32)
                || (value is UInt32)
                || (value is Int64)
                || (value is UInt64)
                || (value is Single)
                || (value is Double)
                || (value is Decimal))
            {
                state.Write(value.ToString());
            }
            else if (value is TimeSpan)
            {
                state.Write(this.LiteralOpenQuote, ((TimeSpan)value).ToString("HH:mm:ss"), this.LiteralCloseQuote);
            }
            else if (value is DateTime)
            {
                state.Write(this.LiteralOpenQuote, ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss"), this.LiteralCloseQuote);
            }
            else if (value is DateTimeOffset)
            {
                state.Write(this.LiteralOpenQuote, ((DateTimeOffset)value).ToString("yyyy-MM-ddTHH:mm:ss"), this.LiteralCloseQuote);
            }
            else
            {
                state.Write(this.LiteralOpenQuote, value.ToString(), this.LiteralCloseQuote);
            }
        }

        protected virtual void VisitStatementToken(IStatement token, VisitorState state)
        {
            state.Write(Sym.op);
            VisitStatement(token, state);
            state.Write(Sym.cp);
        }

        protected virtual void VisitNameToken(Name token, VisitorState state)
        {
            state.Write(ResolveName(token));
        }

        protected virtual void VisitWhereToken(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Write(Sym.WHERE);
                VisitToken(whereToken, state);
            }
        }



        protected virtual void VisitParameterToken(Parameter token, VisitorState state)
        {
            state.Write(token.Name);
        }

        protected virtual void VisitSnippetToken(Snippet token, VisitorState state)
        {
            state.Write(token.Value);
        }

        protected virtual void VisitFunctionToken(Function token, VisitorState state)
        {
            state.Write(token.Name, Sym.op);
            VisitTokenSet(token.Arguments, state, null, Sym.COMMA, null);
            state.Write(Sym.cp);
        }

        protected virtual void VisitBinaryToken(BinaryToken token, VisitorState state, string operation)
        {
            VisitToken(token.First, state);
            state.Write(operation);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitIsEqualsToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.EqualsVal);
        }

        protected virtual void VisitNotEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.NotEqualVal);
        }

        protected virtual void VisitLessToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.LessVal);
        }

        protected virtual void VisitNotLessToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.NotLessVal);
        }

        protected virtual void VisitLessOrEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.LessOrEqualVal);
        }

        protected virtual void VisitGreaterToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.GreaterVal);
        }

        protected virtual void VisitNotGreaterToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.NotGreaterVal);
        }

        protected virtual void VisitGreaterOrEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.GreaterOrEqualVal);
        }

        protected virtual void VisitAllToken(AllToken token, VisitorState state)
        {
            state.Write(Sym.ALL);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitAnyToken(AnyToken token, VisitorState state)
        {
            state.Write(Sym.ANY);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitAndToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.AndVal);
        }

        protected virtual void VisitOrToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.OrVal);
        }

        protected virtual void VisitPlusToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.PlusEqVal : Sym.PlusVal);
        }

        protected virtual void VisitMinusToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.MinusEqVal : Sym.MinusVal);
        }

        protected virtual void VisitDivideToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.DivideEqVal : Sym.DivideVal);
        }

        protected virtual void VisitModuloToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.ModuloEqVal : Sym.ModuloVal);
        }

        protected virtual void VisitMultiplyToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.MultiplyEqVal : Sym.MultiplyVal);
        }

        protected virtual void VisitBitwiseAndToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.BitwiseAndEqVal : Sym.BitwiseAndVal);
        }

        protected virtual void VisitBitwiseOrToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.BitwiseOrEqVal : Sym.BitwiseOrVal);
        }

        protected virtual void VisitBitwiseXorToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.BitwiseXorEqVal : Sym.BitwiseXorVal);
        }

        protected virtual void VisitBitwiseNotToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.BitwiseNotEqVal : Sym.BitwiseNotVal);
        }

        protected virtual void VisitAssignToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.AssignVal);
        }

        protected virtual void VisitLikeToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Write(Sym.LIKE);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitExistsToken(ExistsToken token, VisitorState state)
        {
            state.Write(Sym.EXISTS);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitNotToken(NotToken token, VisitorState state)
        {
            state.Write(Sym.NOT_op);
            VisitToken(token.Token, state);
            state.Write(Sym.cp);
        }

        protected virtual void VisitIsNullToken(IsNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Sym.IS_NULL);
        }

        protected virtual void VisitIsNotNullToken(IsNotNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Sym.IS_NOT_NULL);
        }

        protected virtual void VisitBetweenToken(BetweenToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Sym.BETWEEN);
            VisitToken(token.First, state);
            state.Write(Sym.AND);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitInToken(InToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            VisitTokenSet(token.Set, state, Sym.IN_op, Sym.COMMA, Sym.cp);
        }

        protected virtual void VisitNotInToken(NotInToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            VisitTokenSet(token.Set, state, Sym.NOT_IN_op, Sym.COMMA, Sym.cp);
        }
        protected virtual void VisitCommentToken(CommentToken token, VisitorState state)
        {
            state.Write(this.CommentOpenQuote);
            VisitToken(token.Content, state);
            state.Write(this.CommentCloseQuote);
        }
        protected virtual void VisitFromToken(IEnumerable<Token> recordsets, VisitorState state)
        {
            VisitTokenSet(recordsets, state, Sym.FROM, Sym.COMMA, null, true);
        }

        protected virtual void VisitFromToken(Token recordset, VisitorState state)
        {
            if (recordset != null)
            {
                state.Write(Sym.FROM);
                VisitToken(recordset, state, true);
            }
        }

        protected virtual void VisitGroupByToken(ICollection<Name> groupBy, VisitorState state)
        {
            VisitTokenSet(groupBy, state, Sym.GROUP_BY, Sym.COMMA, null);
        }

        protected virtual void VisitHavingToken(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Write(Sym.HAVING);
                VisitToken(whereToken, state);
            }
        }

        protected virtual void VisitOrderToken(Order orderToken, VisitorState state)
        {
            VisitNameToken(orderToken.Column, state);
            state.Write(orderToken.Direction == Direction.Asc ? Sym.ASC : Sym.DESC);
        }


        protected virtual void VisitOrderByToken(ICollection<Order> orderBy, VisitorState state)
        {
            VisitTokenSet(orderBy, state, Sym.ORDER_BY, Sym.COMMA, null);
        }


        protected virtual void VisitContainsToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Write(Sym.LIKE);
            state.Write(this.LiteralOpenQuote, Sym.ModuloVal, this.LiteralCloseQuote);
            state.Write(Sym.PlusVal);
            VisitToken(token.Second, state);
            state.Write(Sym.PlusVal);
            state.Write(this.LiteralOpenQuote, Sym.ModuloVal, this.LiteralCloseQuote);
        }

        protected virtual void VisitStartsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Write(Sym.LIKE);
            VisitToken(token.Second, state);
            state.Write(Sym.PlusVal);
            state.Write(this.LiteralOpenQuote, Sym.ModuloVal, this.LiteralCloseQuote);
        }

        protected virtual void VisitEndsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Write(Sym.LIKE);
            state.Write(this.LiteralOpenQuote, Sym.ModuloVal, this.LiteralCloseQuote);
            state.Write(Sym.PlusVal);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitGroupToken(GroupToken token, VisitorState state)
        {
            state.Write(Sym.op);
            VisitToken(token.Token, state);
            state.Write(Sym.cp);
        }

        protected virtual void VisitJoin(ICollection<Join> list, VisitorState state)
        {
            if (list.Count > 0)
            {
                foreach (var join in list)
                {
                    VisitJoinType(join.Type, state);
                    VisitToken(@join.Source, state, true);

                    if (join.On != null)
                    {
                        state.Write(Sym.ON);
                        VisitToken(@join.On, state);
                    }
                }
            }
        }
    }
}