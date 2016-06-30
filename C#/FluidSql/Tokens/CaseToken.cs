// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class CaseToken : ExpressionToken
    {
        public CaseToken()
        {
            this.WhenConditions = new List<CaseWhenToken>();
        }

        public ExpressionToken CaseValueToken { get; set; }
        public List<CaseWhenToken> WhenConditions { get; }
        public ExpressionToken ElseToken { get; set; }
    }
}