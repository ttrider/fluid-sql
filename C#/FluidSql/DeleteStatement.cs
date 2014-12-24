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
    {
        public DeleteStatement()
        {
            this.Joins = new List<Join>();
        }

        public Token From { get; set; }
        public List<Join> Joins { get; private set; }
        public Top Top { get; set; }
        public Token Where { get; set; }
    }
}