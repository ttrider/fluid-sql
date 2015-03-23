// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
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
    {
        public SelectStatement()
        {
            this.GroupBy = new List<Name>();
            this.OrderBy = new List<Order>();
            this.From = new List<Token>();
            this.Joins = new List<Join>();
            this.Set = new List<BinaryEqualToken>();
        }

        public List<Token> From { get; set; }
        public bool Distinct { get; set; }
        public Token Having { get; set; }
        public Name Into { get; set; }
        public List<Name> GroupBy { get; private set; }
        public List<Order> OrderBy { get; private set; }
        public List<Join> Joins { get; private set; }
        public IList<BinaryEqualToken> Set { get; private set; }
        public Top Top { get; set; }
        public Token Where { get; set; }

        public Token Offset { get; set; }
    }
}