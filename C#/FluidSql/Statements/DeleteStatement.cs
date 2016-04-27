// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class DeleteStatement : RecordsetStatement
        , IJoinStatement
        , ITopStatement
        , IFromStatement
        , IWhereStatement
        , IOffsetStatement
        , IOrderByStatement
        , ICTEStatement
    {
        public DeleteStatement()
        {
            this.Joins = new List<Join>();
            this.OrderBy = new List<Order>();
            this.CommonTableExpressions = new List<CTEDefinition>();
        }

        public List<CTEDefinition> CommonTableExpressions { get; set; }

        public RecordsetSourceToken RecordsetSource { get; set; }
        public List<Join> Joins { get; }

        public Token Offset { get; set; }
        public List<Order> OrderBy { get; }
        public Top Top { get; set; }
        public Token Where { get; set; }
    }
}