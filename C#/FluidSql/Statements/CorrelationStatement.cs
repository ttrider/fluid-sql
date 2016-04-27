// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class CorrelationStatement : RecordsetStatement
        , ISelectStatement
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
}