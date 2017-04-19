// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class StatementsStatement : IStatement
    {
        public StatementsStatement()
        {
            this.Statements = new List<IStatement>();
        }

        public List<IStatement> Statements { get; private set; }
    }
}