// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class ForeignDefinition : Token
    {
        public ForeignDefinition()
        {
            this.Columns = new List<Order>();
            this.ReferencesColumns = new List<Name>();
        }

        public Name Name { get; set; }
        public Name References { get; set; }

        public List<Order> Columns { get; private set; }

        public List<Name> ReferencesColumns { get; private set; }

        public bool Unique { get; set; }

        public bool? Clustered { get; set; }

        public ForeignKeyOption? OnDelete { get; set; }

        public ForeignKeyOption? OnUpdate { get; set; }
    }
}