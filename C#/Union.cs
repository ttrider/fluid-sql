// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class CorrelationStatement : SelectStatement
    {
        public SelectStatement First { get; set; }
        public SelectStatement Second  { get; set; } 

        protected CorrelationStatement(SelectStatement source1, SelectStatement source2)
        {
            this.First = source1;
            this.Second = source2;
        }
    }

    public class Union : CorrelationStatement
    {
        public bool All { get; set; }

        public Union(SelectStatement source1, SelectStatement source2, bool all=false)
            :base(source1, source2)
        {
            this.All = all;
        }

    }
    public class Intersect : CorrelationStatement
    {
        public Intersect(SelectStatement source1, SelectStatement source2)
            : base(source1, source2)
        {
        }
    }
    public class Except : CorrelationStatement
    {
        public Except(SelectStatement source1, SelectStatement source2)
            : base(source1, source2)
        {
        }
}
}
