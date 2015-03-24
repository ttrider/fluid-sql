// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System;

namespace TTRider.FluidSql
{
    public class CorrelationStatement : RecordsetStatement
    {
        private RecordsetStatement first;

        protected CorrelationStatement(RecordsetStatement source1, RecordsetStatement source2)
        {
            this.First = source1;
            this.Second = source2;
        }

        public RecordsetStatement First
        {
            get { return this.first; }
            set
            {
                if (this.first != value)
                {
                    this.first = value;
                    this.Output.Clear();

                    this.Output.AddRange(
                        this.first
                            .Output);
                }
            }
        }

        public RecordsetStatement Second { get; set; }
    }

    [Obsolete("Please use UnionStatement", true)]
    public class Union : UnionStatement
    {
        public Union(RecordsetStatement source1, RecordsetStatement source2, bool all = false)
            : base(source1, source2, all)
        {
        }
    }
    public class UnionStatement : CorrelationStatement
    {
        public UnionStatement(RecordsetStatement source1, RecordsetStatement source2, bool all = false)
            : base(source1, source2)
        {
            this.All = all;
        }

        public bool All { get; set; }
    }



    public class IntersectStatement : CorrelationStatement
    {
        public IntersectStatement(RecordsetStatement source1, RecordsetStatement source2)
            : base(source1, source2)
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

    public class ExceptStatement : CorrelationStatement
    {
        public ExceptStatement(RecordsetStatement source1, RecordsetStatement source2)
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