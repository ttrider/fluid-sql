// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class PerformStatement : Token, IExecutableStatement, IStatement
    {
        public string Name { get; set; }
        public string Query { get; set; }
        public IStatement Target { get; set; }
    }
}
