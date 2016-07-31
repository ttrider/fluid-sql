// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
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