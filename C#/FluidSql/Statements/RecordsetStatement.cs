// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public abstract class RecordsetStatement : Token, IStatement, IAliasToken
    {
        protected RecordsetStatement()
        {
            this.Output = new List<ExpressionToken>();
        }

        public List<ExpressionToken> Output { get; private set; }
        public Name OutputInto { get; set; }
        public string Alias { get; set; }
    }
}