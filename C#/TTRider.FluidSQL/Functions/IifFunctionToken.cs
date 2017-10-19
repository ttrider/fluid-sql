// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class IifFunctionToken : FunctionExpressionToken
    {
        public ExpressionToken ConditionToken { get; set; }
        public ExpressionToken ThenToken { get; set; }
        public ExpressionToken ElseToken { get; set; }
    }
}