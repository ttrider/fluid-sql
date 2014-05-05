// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System.Linq;

namespace TTRider.FluidSql
{
    public class CorrelationStatement : RecordsetStatement
    {
        private RecordsetStatement first;

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

        protected CorrelationStatement(RecordsetStatement source1, RecordsetStatement source2)
        {
            this.First = source1;
            this.Second = source2;
        }
    }

    public class Union : CorrelationStatement
    {
        public bool All { get; set; }

        public Union(RecordsetStatement source1, RecordsetStatement source2, bool all = false)
            : base(source1, source2)
        {
            this.All = all;
        }

    }
    public class Intersect : CorrelationStatement
    {
        public Intersect(RecordsetStatement source1, RecordsetStatement source2)
            : base(source1, source2)
        {
        }
    }
    public class Except : CorrelationStatement
    {
        public Except(RecordsetStatement source1, RecordsetStatement source2)
            : base(source1, source2)
        {
        }
    }
}
