// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public abstract class RecordsetStatement : Token, IStatement
    {
        protected RecordsetStatement()
        {
            this.Output = new List<Token>();
        }

        public List<Token> Output { get; private set; }
        public Name OutputInto { get; set; }
    }
}
