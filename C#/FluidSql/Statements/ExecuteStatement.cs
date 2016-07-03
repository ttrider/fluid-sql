// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class ExecuteStatement : Token, IExecutableStatement, IStatement
    {
        public string Name { get; set; }
        public IStatement Target { get; set; }
    }
}