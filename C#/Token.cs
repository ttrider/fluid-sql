// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class Token
    {
        public Token()
        {
            this.Parameters = new ParameterSet();
        }
        public ParameterSet Parameters { get; private set; }

        public string Alias { get; set; }
    }
}
