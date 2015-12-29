// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class DurationFunctionToken : FunctionExpressionToken
    {
        public DatePart DatePart { get; set; }
        public ExpressionToken Start { get; set; }
        public ExpressionToken End { get; set; }
    }


    public class CaseToken : ExpressionToken
    {
        public CaseToken()
        {
            this.WhenConditions = new List<CaseWhenToken>();
        }

        public List<CaseWhenToken> WhenConditions { get; }
        public Token ElseToken { get; set; }
    }

    public class CaseWhenToken : Token
    {
        public ExpressionToken WhenToken { get; set; }
        public ExpressionToken ThenToken { get; set; }
    }
}