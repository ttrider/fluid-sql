// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class CaseWhenToken : Token
    {
        public ExpressionToken WhenToken { get; set; }
        public ExpressionToken ThenToken { get; set; }
    }
}