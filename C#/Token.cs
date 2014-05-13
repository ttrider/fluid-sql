// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>


using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class Token
    {
        public Token()
        {
            this.Parameters = new ParameterSet();
            this.ParameterValues = new List<ParameterValue>();
        }
        public ParameterSet Parameters { get; private set; }

        public IList<ParameterValue> ParameterValues { get; private set; }

        public string Alias { get; set; }
    }
}
