﻿// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class AlterFunctionStatement : IProcedureStatement, IStatement
    {
        public bool CheckIfNotExists { get; set; }
        public Name Name { get; set; }
        public ParameterSet Parameters { get; } = new ParameterSet();
        public ParameterSet Declarations { get; } = new ParameterSet();
        public IStatement Body { get; set; }
        public bool Recompile { get; set; }
    }
}
