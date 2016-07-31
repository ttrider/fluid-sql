// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class CTEDeclaration
    {
        public CTEDeclaration()
        {
            this.Columns = new List<Name>();
        }

        public string Name { get; set; }
        public List<Name> Columns { get; set; }

        public bool Recursive { get; set; }
        public CTEDefinition PreviousCommonTableExpression { get; set; }
    }
}