// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public abstract class RecordsetStatement : AliasedToken, IStatement
    {
        protected RecordsetStatement()
        {
            this.Output = new List<AliasedToken>();
        }

        public List<AliasedToken> Output { get; private set; }
        public Name OutputInto { get; set; }
    }
}