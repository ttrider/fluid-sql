// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
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

        public Token From { get; set; }
        public List<Join> Joins { get; private set; }
        public Top Top { get; set; }
        public Token Where { get; set; }

        // supported on Sqlite
        public Token Offset { get; set; }
        public List<Order> OrderBy { get; private set; }
        public List<CTEDefinition> CommonTableExpressions { get; set; }
    }
}