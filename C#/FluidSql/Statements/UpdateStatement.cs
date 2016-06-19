﻿// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class UpdateStatement : RecordsetStatement
        , ITopStatement
        , IFromStatement
        , ISetStatement
        , IJoinStatement
        , IWhereStatement
        , ICTEStatement
        , IOnConflict

    {
        public UpdateStatement()
        {
            this.Set = new List<BinaryEqualToken>();
            this.Joins = new List<Join>();
            this.CommonTableExpressions = new List<CTEDefinition>();
        }

        public Name Target { get; set; }
        public RecordsetSourceToken RecordsetSource { get; set; }
        public List<Join> Joins { get; private set; }
        public IList<BinaryEqualToken> Set { get; private set; }
        public Top Top { get; set; }
        public Token Where { get; set; }
        public OnConflict? Conflict { get; set; }
        public List<CTEDefinition> CommonTableExpressions { get; set; }
        
        //supported on Postgresql
        public bool Only { get; set; }
        public Name CursorName { get; set; }
    }
}