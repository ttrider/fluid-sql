// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;

namespace TTRider.FluidSql
{
    [Obsolete("Please use UnionStatement", true)]
    public class Union : UnionStatement
    {
        public Union(RecordsetStatement source1, RecordsetStatement source2, bool all = false)
            : base(source1, source2, all)
        {
        }
    }


    [Obsolete("Please use IntersectStatement", true)]
    public class Intersect : IntersectStatement
    {
        public Intersect(RecordsetStatement source1, RecordsetStatement source2)
            : base(source1, source2)
        {
        }
    }

    [Obsolete("Please use ExceptStatement", true)]
    public class Except : ExceptStatement
    {
        public Except(RecordsetStatement source1, RecordsetStatement source2)
            : base(source1, source2)
        {
        }
    }
}