// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {
        private static readonly Regex SnippetArgumentRegex = new Regex(@"{(?<index>\d+)([,:][^}]*)?}");

        protected virtual void VisitStringifyToken(StringifyToken token)
        {
            throw new NotImplementedException();
        }

        protected virtual void VisitWhenMatchedThenDelete(WhenMatchedTokenThenDeleteToken token)
        {
            throw new NotImplementedException();
        }

        protected virtual void VisitWhenMatchedThenUpdateSet(WhenMatchedTokenThenUpdateSetToken token)
        {
            throw new NotImplementedException();
        }

        protected virtual void VisitWhenNotMatchedThenInsert(WhenNotMatchedTokenThenInsertToken token)
        {
            throw new NotImplementedException();
        }


        protected virtual void VisitScalarToken(Scalar token)
        {
            var value = token.Value;
            if (value == null) return;


            if (value is DBNull) { FormatScalarValue((DBNull)value); return; }
            if (value is Boolean) { FormatScalarValue((bool)value); return; }
            if (value is SByte) { FormatScalarValue((SByte)value); return; }
            if (value is Byte) { FormatScalarValue((Byte)value); return; }
            if (value is Int16) { FormatScalarValue((Int16)value); return; }
            if (value is UInt16) { FormatScalarValue((UInt16)value); return; }
            if (value is Int32) { FormatScalarValue((Int32)value); return; }
            if (value is UInt32) { FormatScalarValue((UInt32)value); return; }
            if (value is Int64) { FormatScalarValue((Int64)value); return; }
            if (value is UInt64) { FormatScalarValue((UInt64)value); return; }
            if (value is Single) { FormatScalarValue((Single)value); return; }
            if (value is Double) { FormatScalarValue((Double)value); return; }
            if (value is Decimal) { FormatScalarValue((Decimal)value); return; }

            if (value is TimeSpan) { FormatScalarValue((TimeSpan)value); return; }
            if (value is DateTime) { FormatScalarValue((DateTime)value); return; }
            if (value is DateTimeOffset) { FormatScalarValue((DateTimeOffset)value); return; }

            FormatScalarValue(value.ToString());



        }



        protected virtual void VisitStatementToken(IStatement token)
        {
            State.Write(Symbols.OpenParenthesis);
            VisitStatement(token);
            State.Write(Symbols.CloseParenthesis);
        }

        protected virtual void VisitNameToken(Name token)
        {
            State.Write(ResolveName(token));
        }

        protected virtual void VisitWhereToken(Token whereToken)
        {
            if (whereToken != null)
            {
                State.Write(Symbols.WHERE);
                VisitToken(whereToken);
            }
        }


        protected virtual void VisitParameterToken(Parameter token)
        {
            State.Write(token.Name);
        }


        protected virtual void VisitSnippetToken(Snippet token)
        {
            var value = token.GetValue(this.SupportedDialects);

            if (token.Arguments.Count > 0)
            {
                var index = 0;
                SnippetArgumentRegex.Replace(value, match =>
                {
                    if (match.Index > index)
                    {
                        State.Write(value.Substring(index, match.Index - index));
                    }
                    var argIndex = int.Parse(match.Groups["index"].Value);
                    VisitToken(token.Arguments[argIndex]);
                    index = match.Index + match.Length;
                    return "";
                });
                State.Write(value.Substring(index));
            }
            else
            {
                State.Write(value);
            }
        }

        protected virtual void VisitFunctionToken(Function token)
        {
            State.Write(token.Name, Symbols.OpenParenthesis);
            VisitTokenSet(token.Arguments);
            State.Write(Symbols.CloseParenthesis);
        }

        protected virtual void VisitBinaryToken(BinaryToken token, string operation)
        {
            VisitToken(token.First);
            State.Write(operation);
            VisitToken(token.Second);
        }

        protected virtual void VisitIsEqualsToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.EqualsVal);
        }

        protected virtual void VisitNotEqualToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.NotEqualVal);
        }

        protected virtual void VisitLessToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.LessVal);
        }

        protected virtual void VisitNotLessToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.NotLessVal);
        }

        protected virtual void VisitLessOrEqualToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.LessOrEqualVal);
        }

        protected virtual void VisitGreaterToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.GreaterVal);
        }

        protected virtual void VisitNotGreaterToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.NotGreaterVal);
        }

        protected virtual void VisitGreaterOrEqualToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.GreaterOrEqualVal);
        }

        protected virtual void VisitAllToken(AllToken token)
        {
            State.Write(Symbols.ALL);
            VisitToken(token.Token);
        }

        protected virtual void VisitAnyToken(AnyToken token)
        {
            State.Write(Symbols.ANY);
            VisitToken(token.Token);
        }

        protected virtual void VisitAndToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.AndVal);
        }

        protected virtual void VisitOrToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.OrVal);
        }

        protected virtual void VisitPlusToken(BinaryEqualToken token)
        {
            VisitBinaryToken(token, token.Equal ? Symbols.PlusEqVal : Symbols.PlusVal);
        }

        protected virtual void VisitMinusToken(BinaryEqualToken token)
        {
            VisitBinaryToken(token, token.Equal ? Symbols.MinusEqVal : Symbols.MinusVal);
        }

        protected virtual void VisitDivideToken(BinaryEqualToken token)
        {
            VisitBinaryToken(token, token.Equal ? Symbols.DivideEqVal : Symbols.DivideVal);
        }

        protected virtual void VisitModuloToken(BinaryEqualToken token)
        {
            VisitBinaryToken(token, token.Equal ? Symbols.ModuloEqVal : Symbols.ModuloVal);
        }

        protected virtual void VisitMultiplyToken(BinaryEqualToken token)
        {
            VisitBinaryToken(token, token.Equal ? Symbols.MultiplyEqVal : Symbols.MultiplyVal);
        }

        protected virtual void VisitBitwiseAndToken(BinaryEqualToken token)
        {
            VisitBinaryToken(token, token.Equal ? Symbols.BitwiseAndEqVal : Symbols.BitwiseAndVal);
        }

        protected virtual void VisitBitwiseOrToken(BinaryEqualToken token)
        {
            VisitBinaryToken(token, token.Equal ? Symbols.BitwiseOrEqVal : Symbols.BitwiseOrVal);
        }

        protected virtual void VisitBitwiseXorToken(BinaryEqualToken token)
        {
            VisitBinaryToken(token, token.Equal ? Symbols.BitwiseXorEqVal : Symbols.BitwiseXorVal);
        }

        protected virtual void VisitBitwiseNotToken(BitwiseNotToken token)
        {
            State.Write(Symbols.BitwiseNotVal);
            VisitToken(token.Token);
        }

        protected virtual void VisitAssignToken(BinaryToken token)
        {
            VisitBinaryToken(token, Symbols.AssignVal);
        }

        protected virtual void VisitLikeToken(BinaryToken token)
        {
            VisitToken(token.First);
            State.Write(Symbols.LIKE);
            VisitToken(token.Second);
        }

        protected virtual void VisitExistsToken(ExistsToken token)
        {
            State.Write(Symbols.EXISTS);
            VisitToken(token.Token);
        }

        protected virtual void VisitNotToken(NotToken token)
        {
            State.Write(Symbols.NOT);
            State.Write(Symbols.OpenParenthesis);
            VisitToken(token.Token);
            State.Write(Symbols.CloseParenthesis);
        }

        protected virtual void VisitIsNullToken(IsNullToken token)
        {
            VisitToken(token.Token);
            State.Write(Symbols.IS);
            State.Write(Symbols.NULL);
        }

        protected virtual void VisitIsNotNullToken(IsNotNullToken token)
        {
            VisitToken(token.Token);
            State.Write(Symbols.IS);
            State.Write(Symbols.NOT);
            State.Write(Symbols.NULL);
        }

        protected virtual void VisitBetweenToken(BetweenToken token)
        {
            VisitToken(token.Token);
            State.Write(Symbols.BETWEEN);
            VisitToken(token.First);
            State.Write(Symbols.AND);
            VisitToken(token.Second);
        }

        protected virtual void VisitInToken(InToken token)
        {
            VisitToken(token.Token);
            State.Write(Symbols.IN);
            VisitTokenSetInParenthesis(token.Set);
        }

        protected virtual void VisitNotInToken(NotInToken token)
        {
            VisitToken(token.Token);
            State.Write(Symbols.NOT);
            State.Write(Symbols.IN);
            VisitTokenSetInParenthesis(token.Set);
        }

        protected virtual void VisitCommentToken(CommentToken token)
        {
            State.Write(this.CommentOpenQuote);
            VisitToken(token.Content);
            State.Write(this.CommentCloseQuote);
        }

        protected virtual void VisitFromToken(IEnumerable<Token> recordsets)
        {
            VisitTokenSet(recordsets);
        }

        protected virtual void VisitFromToken(RecordsetSourceToken recordset)
        {
            if (recordset != null)
            {
                VisitToken(recordset.Source);
                VisitAlias(recordset.Alias);
            }
        }

        protected virtual void VisitGroupByToken(ICollection<Name> groupBy)
        {
            VisitTokenSet(groupBy, () => State.Write(Symbols.GROUP_BY));
        }

        protected virtual void VisitHavingToken(Token whereToken)
        {
            if (whereToken != null)
            {
                State.Write(Symbols.HAVING);
                VisitToken(whereToken);
            }
        }

        protected virtual void VisitOrderToken(Order orderToken)
        {
            VisitNameToken(orderToken.Column);
            State.Write(orderToken.Direction == Direction.Asc ? Symbols.ASC : Symbols.DESC);
        }


        protected virtual void VisitOrderByToken(ICollection<Order> orderBy)
        {
            if (orderBy != null)
            {
                VisitTokenSet(orderBy, () => State.Write(Symbols.ORDER_BY));
            }
        }


        protected virtual void VisitContainsToken(BinaryToken token)
        {
            VisitToken(token.First);
            State.Write(Symbols.LIKE);
            State.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
            State.Write(Symbols.PlusVal);
            VisitToken(token.Second);
            State.Write(Symbols.PlusVal);
            State.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
        }

        protected virtual void VisitStartsWithToken(BinaryToken token)
        {
            VisitToken(token.First);
            State.Write(Symbols.LIKE);
            VisitToken(token.Second);
            State.Write(Symbols.PlusVal);
            State.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
        }

        protected virtual void VisitEndsWithToken(BinaryToken token)
        {
            VisitToken(token.First);
            State.Write(Symbols.LIKE);
            State.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
            State.Write(Symbols.PlusVal);
            VisitToken(token.Second);
        }

        protected virtual void VisitGroupToken(GroupToken token)
        {
            State.Write(Symbols.OpenParenthesis);
            VisitToken(token.Token);
            State.Write(Symbols.CloseParenthesis);
        }

        protected virtual void VisitUnaryMinusToken(UnaryMinusToken token)
        {
            State.Write(Symbols.MinusVal);
            VisitToken(token.Token);
        }

        protected virtual void VisitJoin(ICollection<Join> list)
        {
            if (list.Count > 0)
            {
                foreach (var join in list)
                {
                    VisitJoinType(join.Type);

                    VisitFromToken(join.Source);

                    if (join.On != null)
                    {
                        State.Write(Symbols.ON);
                        VisitToken(@join.On);
                    }
                }
            }
        }
    }
}