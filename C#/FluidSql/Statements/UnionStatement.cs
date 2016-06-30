// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class UnionStatement : CorrelationStatement
    {
        public UnionStatement(RecordsetStatement source1, RecordsetStatement source2, bool all = false)
            : base(source1, source2)
        {
            this.All = all;
        }

        public bool All { get; set; }
    }
}