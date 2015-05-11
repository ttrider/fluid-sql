// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class SnippetStatement : RecordsetStatement
    {
        public SnippetStatement()
        {
            this.Dialects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Value { get; set; }
    
        public IDictionary<string, string> Dialects { get; private set; }

        public string GetValue(params string[] compatibleDialects)
        {
            // try to find a compatible dialect. if not - return default
            foreach (var dialect in compatibleDialects)
            {
                string val;
                if (this.Dialects.TryGetValue(dialect, out val))
                {
                    return val;
                }
            }
            return this.Value;
        }
    }
}