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
        , ISelectStatement
    {
        public DeleteStatement()
        {
            this.Joins = new List<Join>();
            this.OrderBy = new List<Order>();
            this.CommonTableExpressions = new List<CTEDefinition>();
            this.UsingList = new List<Name>();
            this.Returning = new List<ExpressionToken>();
        }   

        public RecordsetSourceToken RecordsetSource { get; set; }
        public List<Join> Joins { get; }

        public Top Top { get; set; }
        public Token Where { get; set; }

        // supported on Sqlite
        public Token Offset { get; set; }
        public List<Order> OrderBy { get; private set; }
        public List<CTEDefinition> CommonTableExpressions { get; set; }

        //supported on Postgresql
        public bool Only { get; set; }
        public Name CursorName { get; set; }
        public List<Name> UsingList { get; set; }
        public List<ExpressionToken> Returning { get; private set; }
    }
}