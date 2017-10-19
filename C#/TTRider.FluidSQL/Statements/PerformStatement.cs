// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class PerformStatement : Token, IExecutableStatement, IStatement
    {
        public Name Name { get; set; }
        public string Query { get; set; }
        public ExecParameter Target { get; set; }
    }
}
