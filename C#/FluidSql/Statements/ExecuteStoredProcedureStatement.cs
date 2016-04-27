// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class ExecuteProcedureStatement : IStatement
    {
        public Name Name { get; set; }

        public bool Recompile { get; set; }

        public List<Parameter> Parameters { get; } = new List<Parameter>();
    }
}