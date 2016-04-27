// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class MakeTimeFunctionToken : DateFunctionExpressionToken
    {
        public ExpressionToken Hour { get; set; }
        public ExpressionToken Minute { get; set; }
        public ExpressionToken Second { get; set; }
    }
}