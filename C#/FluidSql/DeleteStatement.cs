// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
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
    {
        public DeleteStatement()
        {
            this.Joins = new List<Join>();
            this.OrderBy = new List<Order>();
        }

        public Token From { get; set; }
        public List<Join> Joins { get; private set; }
        public Top Top { get; set; }
        public Token Where { get; set; }

        // supported on Sqlite
        public Token Offset { get; set; }
        public List<Order> OrderBy { get; private set; }

    }
}