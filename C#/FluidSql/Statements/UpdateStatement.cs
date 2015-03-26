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
    {
        public UpdateStatement()
        {
            this.Set = new List<BinaryEqualToken>();
            this.Joins = new List<Join>();
        }

        public Name Target { get; set; }
        public Token From { get; set; }
        public List<Join> Joins { get; private set; }
        public IList<BinaryEqualToken> Set { get; private set; }
        public Top Top { get; set; }
        public Token Where { get; set; }
    }
}