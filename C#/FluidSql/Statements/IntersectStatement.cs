// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class IntersectStatement : CorrelationStatement
    {
        public IntersectStatement(RecordsetStatement source1, RecordsetStatement source2)
            : base(source1, source2)
        {
        }
    }
}