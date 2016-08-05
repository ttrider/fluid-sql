// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class ForeignDefinitionBase : Token
    {
        public Name Name { get; set; }
        public Name References { get; set; }
        public ForeignKeyOption? OnDelete { get; set; }

        public ForeignKeyOption? OnUpdate { get; set; }


    }


    public class ForeignDefinition : ForeignDefinitionBase
    {
        public List<Order> Columns { get; private set; } = new List<Order>();

        public List<Name> ReferencesColumns { get; private set; }=new List<Name>();

        public bool Unique { get; set; }

        public bool? Clustered { get; set; }
    }

    
}