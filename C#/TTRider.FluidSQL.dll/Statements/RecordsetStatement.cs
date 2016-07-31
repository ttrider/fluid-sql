// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public abstract class RecordsetStatement : ExpressionToken, IStatement
    {
        protected RecordsetStatement()
        {
            this.Output = new List<ExpressionToken>();
        }

        public List<ExpressionToken> Output { get; private set; }
        public Name OutputInto { get; set; }
    }
}