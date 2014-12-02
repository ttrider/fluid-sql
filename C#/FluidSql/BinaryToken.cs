// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class BinaryToken : Token
    {
        public Token First { get; set; }
        public Token Second { get; set; }
    }

    public class IsEqualsToken : BinaryToken { }
    public class NotEqualToken : BinaryToken { }
    public class LessToken : BinaryToken { }
    public class LessOrEqualToken : BinaryToken { }
    public class GreaterToken : BinaryToken { }
    public class GreaterOrEqualToken : BinaryToken { }
    public class AndToken : BinaryToken { }
    public class OrToken : BinaryToken { }
    public class PlusToken : BinaryToken { }
    public class MinusToken : BinaryToken { }
    public class DivideToken : BinaryToken { }
    public class ModuleToken : BinaryToken { }
    public class MultiplyToken : BinaryToken { }
    public class ContainsToken : BinaryToken { }
    public class StartsWithToken : BinaryToken { }
    public class EndsWithToken : BinaryToken { }


    public class UnaryToken : Token
    {
        public Token Token { get; set; }
    }
    public class GroupToken : UnaryToken { }
    public class NotToken : UnaryToken { }
    public class IsNullToken : UnaryToken { }
    public class IsNotNullToken : UnaryToken { }
    public class ExistsToken : UnaryToken { }

    public class BetweenToken : Token
    {
        public Token Token { get; set; }
        public Token First { get; set; }
        public Token Second { get; set; }
    };

    public class InToken : Token
    {
        public InToken()
        {
            this.Set = new List<Token>();
        }
        public Token Token { get; set; }
        public List<Token> Set { get; private set; } 
    };

    public class NotInToken : Token
    {
        public NotInToken()
        {
            this.Set = new List<Token>();
        }

        public Token Token { get; set; }
        public List<Token> Set { get; private set; }
    }
}
