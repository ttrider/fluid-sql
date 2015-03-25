// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {

        protected virtual void VisitStringifyToken(StringifyToken token, VisitorState state) { throw new NotImplementedException(); }
        protected virtual void VisitWhenMatchedThenDelete(WhenMatchedTokenThenDeleteToken token, VisitorState state) { throw new NotImplementedException(); }
        protected virtual void VisitWhenMatchedThenUpdateSet(WhenMatchedTokenThenUpdateSetToken token, VisitorState state) { throw new NotImplementedException(); }
        protected virtual void VisitWhenNotMatchedThenInsert(WhenNotMatchedTokenThenInsertToken token, VisitorState state) { throw new NotImplementedException(); }





        protected virtual void VisitScalarToken(Scalar token, VisitorState state)
        {
            var value = token.Value;
            if (value == null) return;

            if (value is DBNull)
            {
                state.Write(Symbols.NULL);
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
            state.Write(Symbols.OpenParenthesis);
            VisitStatement(token, state);
            state.Write(Symbols.CloseParenthesis);
        }

        protected virtual void VisitNameToken(Name token, VisitorState state)
        {
            state.Write(ResolveName(token));
        }

        protected virtual void VisitWhereToken(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Write(Symbols.WHERE);
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
            state.Write(token.Name, Symbols.OpenParenthesis);
            VisitTokenSet(token.Arguments, state, null, Symbols.Comma, null);
            state.Write(Symbols.CloseParenthesis);
        }

        protected virtual void VisitBinaryToken(BinaryToken token, VisitorState state, string operation)
        {
            VisitToken(token.First, state);
            state.Write(operation);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitIsEqualsToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.EqualsVal);
        }

        protected virtual void VisitNotEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.NotEqualVal);
        }

        protected virtual void VisitLessToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.LessVal);
        }

        protected virtual void VisitNotLessToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.NotLessVal);
        }

        protected virtual void VisitLessOrEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.LessOrEqualVal);
        }

        protected virtual void VisitGreaterToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.GreaterVal);
        }

        protected virtual void VisitNotGreaterToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.NotGreaterVal);
        }

        protected virtual void VisitGreaterOrEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.GreaterOrEqualVal);
        }

        protected virtual void VisitAllToken(AllToken token, VisitorState state)
        {
            state.Write(Symbols.ALL);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitAnyToken(AnyToken token, VisitorState state)
        {
            state.Write(Symbols.ANY);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitAndToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.AndVal);
        }

        protected virtual void VisitOrToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.OrVal);
        }

        protected virtual void VisitPlusToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Symbols.PlusEqVal : Symbols.PlusVal);
        }

        protected virtual void VisitMinusToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Symbols.MinusEqVal : Symbols.MinusVal);
        }

        protected virtual void VisitDivideToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Symbols.DivideEqVal : Symbols.DivideVal);
        }

        protected virtual void VisitModuloToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Symbols.ModuloEqVal : Symbols.ModuloVal);
        }

        protected virtual void VisitMultiplyToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Symbols.MultiplyEqVal : Symbols.MultiplyVal);
        }

        protected virtual void VisitBitwiseAndToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Symbols.BitwiseAndEqVal : Symbols.BitwiseAndVal);
        }

        protected virtual void VisitBitwiseOrToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Symbols.BitwiseOrEqVal : Symbols.BitwiseOrVal);
        }

        protected virtual void VisitBitwiseXorToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Symbols.BitwiseXorEqVal : Symbols.BitwiseXorVal);
        }

        protected virtual void VisitBitwiseNotToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Symbols.BitwiseNotEqVal : Symbols.BitwiseNotVal);
        }

        protected virtual void VisitAssignToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Symbols.AssignVal);
        }

        protected virtual void VisitLikeToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Write(Symbols.LIKE);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitExistsToken(ExistsToken token, VisitorState state)
        {
            state.Write(Symbols.EXISTS);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitNotToken(NotToken token, VisitorState state)
        {
            state.Write(Symbols.NOT);
            state.Write(Symbols.OpenParenthesis);
            VisitToken(token.Token, state);
            state.Write(Symbols.CloseParenthesis);
        }

        protected virtual void VisitIsNullToken(IsNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Symbols.IS);
            state.Write(Symbols.NULL);
        }

        protected virtual void VisitIsNotNullToken(IsNotNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Symbols.IS);
            state.Write(Symbols.NOT);
            state.Write(Symbols.NULL);
        }

        protected virtual void VisitBetweenToken(BetweenToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Symbols.BETWEEN);
            VisitToken(token.First, state);
            state.Write(Symbols.AND);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitInToken(InToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Symbols.IN);
            VisitTokenSet(token.Set, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
        }

        protected virtual void VisitNotInToken(NotInToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Symbols.NOT);
            state.Write(Symbols.IN);
            VisitTokenSet(token.Set, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
        }
        protected virtual void VisitCommentToken(CommentToken token, VisitorState state)
        {
            state.Write(this.CommentOpenQuote);
            VisitToken(token.Content, state);
            state.Write(this.CommentCloseQuote);
        }
        protected virtual void VisitFromToken(IEnumerable<Token> recordsets, VisitorState state)
        {
            VisitTokenSet(recordsets, state, Symbols.FROM, Symbols.Comma, null, true);
        }

        protected virtual void VisitFromToken(Token recordset, VisitorState state)
        {
            if (recordset != null)
            {
                state.Write(Symbols.FROM);
                VisitToken(recordset, state, true);
            }
        }

        protected virtual void VisitGroupByToken(ICollection<Name> groupBy, VisitorState state)
        {
            VisitTokenSet(groupBy, state, Symbols.GROUP_BY, Symbols.Comma, null);
        }

        protected virtual void VisitHavingToken(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Write(Symbols.HAVING);
                VisitToken(whereToken, state);
            }
        }

        protected virtual void VisitOrderToken(Order orderToken, VisitorState state)
        {
            VisitNameToken(orderToken.Column, state);
            state.Write(orderToken.Direction == Direction.Asc ? Symbols.ASC : Symbols.DESC);
        }


        protected virtual void VisitOrderByToken(ICollection<Order> orderBy, VisitorState state)
        {
            if (orderBy != null)
            {
                VisitTokenSet(orderBy, state, Symbols.ORDER_BY, Symbols.Comma, null);
            }
        }


        protected virtual void VisitContainsToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Write(Symbols.LIKE);
            state.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
            state.Write(Symbols.PlusVal);
            VisitToken(token.Second, state);
            state.Write(Symbols.PlusVal);
            state.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
        }

        protected virtual void VisitStartsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Write(Symbols.LIKE);
            VisitToken(token.Second, state);
            state.Write(Symbols.PlusVal);
            state.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
        }

        protected virtual void VisitEndsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Write(Symbols.LIKE);
            state.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
            state.Write(Symbols.PlusVal);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitGroupToken(GroupToken token, VisitorState state)
        {
            state.Write(Symbols.OpenParenthesis);
            VisitToken(token.Token, state);
            state.Write(Symbols.CloseParenthesis);
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
                        state.Write(Symbols.ON);
                        VisitToken(@join.On, state);
                    }
                }
            }
        }
    }
}