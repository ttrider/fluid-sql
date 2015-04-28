// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class SnippetStatement : RecordsetStatement
    {
        public SnippetStatement()
        {
            this.Arguments = new List<Token>();
        }

        public string Value { get; set; }

        public IList<Token> Arguments { get; private set; }

    }
}