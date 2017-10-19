// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class ExecuteFunctionStatement: IStatement
    {
        public Name Name { get; set; }

        public List<Parameter> Parameters { get; } = new List<Parameter>();
    }
}
