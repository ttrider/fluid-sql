// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class CaseWhenToken : Token
    {
        public ExpressionToken WhenToken { get; set; }
        public ExpressionToken ThenToken { get; set; }
    }
}