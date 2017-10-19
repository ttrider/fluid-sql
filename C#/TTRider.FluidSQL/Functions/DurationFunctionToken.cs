// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class DurationFunctionToken : FunctionExpressionToken
    {
        public DatePart DatePart { get; set; }
        public ExpressionToken Start { get; set; }
        public ExpressionToken End { get; set; }
    }
}