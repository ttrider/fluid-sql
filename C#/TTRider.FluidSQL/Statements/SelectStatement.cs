// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class SelectStatement : RecordsetStatement
        , IJoinStatement
        , ITopStatement
        , ISetStatement
        , IWhereStatement
        , IIntoStatement
        , IOrderByStatement
        , ICTEStatement
        , IOffsetStatement
        , ISelectStatement
    {
        public SelectStatement()
        {
            this.GroupBy = new List<Name>();
            this.OrderBy = new List<Order>();
            this.From = new List<RecordsetSourceToken>();
            this.Joins = new List<Join>();
            this.Set = new List<BinaryEqualToken>();
            this.CommonTableExpressions = new List<CTEDefinition>();
        }

        public List<RecordsetSourceToken> From { get; set; }
        public bool Distinct { get; set; }
        public Token Having { get; set; }
        public List<Name> GroupBy { get; private set; }
        public List<CTEDefinition> CommonTableExpressions { get; set; }
        public Name Into { get; set; }
        public List<Join> Joins { get; }

        public Token Offset { get; set; }
        public List<Order> OrderBy { get; }
        public IList<BinaryEqualToken> Set { get; }
        public Top Top { get; set; }
        public Token Where { get; set; }
    }
}