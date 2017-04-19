// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class ExceptStatement : CorrelationStatement
    {
        public ExceptStatement(RecordsetStatement source1, RecordsetStatement source2, bool all)
            : base(source1, source2)
        {
            this.All = all;
        }

        public bool All { get; set; }
    }
}