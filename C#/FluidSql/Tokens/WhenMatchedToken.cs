// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class WhenMatchedToken : Token
    {
        public Token AndCondition { get; set; }

        public bool IsDeleteMatched { get; set; }
    }
}