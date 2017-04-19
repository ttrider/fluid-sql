// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
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