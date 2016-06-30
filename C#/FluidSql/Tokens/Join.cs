// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class Join : Token
    {
        public Joins Type { get; set; }

        public RecordsetSourceToken Source { get; set; }

        public ExpressionToken On { get; set; }
    }
}