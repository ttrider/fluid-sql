// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using TTRider.FluidSql.Providers.SqlServer;

namespace TTRider.FluidSql
{
    [DebuggerDisplay("T-SQL: {TSql}")]
    public class Token
    {
        public Token()
        {
            this.Parameters = new ParameterSet();
            this.ParameterValues = new List<ParameterValue>();
        }

        public ParameterSet Parameters { get; private set; }

        public IList<ParameterValue> ParameterValues { get; private set; }

        private string TSql
        {
            get
            {
                var state = SqlServerVisitor.Compile(this);
                return state.Value;
            }
        }
    }
}