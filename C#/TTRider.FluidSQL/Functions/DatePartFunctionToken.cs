// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class DatePartFunctionToken : FunctionExpressionToken
    {
        public DatePart DatePart { get; set; }

        public Token Token { get; set; }
    }
}