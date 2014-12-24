// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class InsertStatement : RecordsetStatement
        , ITopStatement
    {
        public InsertStatement()
        {
            this.Columns = new List<Name>();
            this.Values = new List<Token[]>();
        }

        public Name Into { get; set; }
        public RecordsetStatement From { get; set; }
        public bool DefaultValues { get; set; }
        public List<Name> Columns { get; private set; }
        public List<Token[]> Values { get; private set; }
        public Top Top { get; set; }
    }
}