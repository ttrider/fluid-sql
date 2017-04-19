// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
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