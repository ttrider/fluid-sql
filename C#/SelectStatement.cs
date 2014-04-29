// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>
using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class SelectStatement : RecordsetStatement
    {
        public SelectStatement()
        {
            this.Output = new List<Token>();
            this.GroupBy = new List<Name>();
            this.OrderBy = new List<Order>();
            this.From = new List<Token>();
            this.Joins = new List<Join>();
        }

        public List<Token> From { get; set; }
        public Top Top { get; set; }
        public bool Distinct { get; set; }
        public Token Where { get; set; }
        public Name Into { get; set; }
        public List<Token> Output { get; private set; }
        public List<Name> GroupBy { get; private set; }
        public List<Order> OrderBy { get; private set; }
        public List<Join> Joins { get; private set; }
    }
}
