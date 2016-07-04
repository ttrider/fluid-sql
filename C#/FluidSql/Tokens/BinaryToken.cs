// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class BinaryToken : ExpressionToken
    {
        public ExpressionToken First { get; set; }
        public ExpressionToken Second { get; set; }
    }

    public class BinaryEqualToken : BinaryToken
    {
        public bool Equal { get; set; }
    }

    public class IsEqualsToken : BinaryToken
    {
    }

    public class NotEqualToken : BinaryToken
    {
    }

    public class LessToken : BinaryToken
    {
    }

    public class NotLessToken : BinaryToken
    {
    }

    public class LessOrEqualToken : BinaryToken
    {
    }

    public class GreaterToken : BinaryToken
    {
    }

    public class NotGreaterToken : BinaryToken
    {
    }

    public class GreaterOrEqualToken : BinaryToken
    {
    }

    public class AndToken : BinaryToken
    {
    }

    public class OrToken : BinaryToken
    {
    }

    public class PlusToken : BinaryEqualToken
    {
    }

    public class MinusToken : BinaryEqualToken
    {
    }

    public class DivideToken : BinaryEqualToken
    {
    }

    public class BitwiseAndToken : BinaryEqualToken
    {
    }

    public class BitwiseOrToken : BinaryEqualToken
    {
    }

    public class BitwiseXorToken : BinaryEqualToken
    {
    }

    public class BitwiseNotToken : UnaryToken
    {
    }

    public class ModuloToken : BinaryEqualToken
    {
    }

    public class MultiplyToken : BinaryEqualToken
    {
    }

    public class ContainsToken : BinaryToken
    {
    }

    public class StartsWithToken : BinaryToken
    {
    }

    public class EndsWithToken : BinaryToken
    {
    }

    public class LikeToken : BinaryToken
    {
    }

    public class AssignToken : BinaryEqualToken
    {
        public AssignToken()
        {
            this.Equal = true;
        }
    }

    public class UnaryToken : ExpressionToken
    {
        public Token Token { get; set; }
    }

    public class UnaryMinusToken : UnaryToken
    {
    }

    public class GroupToken : UnaryToken
    {
    }

    public class NotToken : UnaryToken
    {
    }

    public class IsNullToken : UnaryToken
    {
    }

    public class IsNotNullToken : UnaryToken
    {
    }

    public class ExistsToken : UnaryToken
    {
    }

    public class AllToken : UnaryToken
    {
    }

    public class AnyToken : UnaryToken
    {
    }

    public class BetweenToken : ExpressionToken
    {
        public ExpressionToken Token { get; set; }
        public ExpressionToken First { get; set; }
        public ExpressionToken Second { get; set; }
    }

    public class SequenceToken : ExpressionToken
    {
        public SequenceToken()
        {
            this.Set = new List<Token>();
        }

        public List<Token> Set { get; private set; }
    }


    public class InToken : SequenceToken
    {
        public Token Token { get; set; }
    }

    public class NotInToken : SequenceToken
    {
        public Token Token { get; set; }
    }
}