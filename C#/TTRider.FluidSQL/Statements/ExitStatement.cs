// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class ExitStatement : Token, IStatement
    {
        public string Label { get; set; }
        public ExpressionToken When { get; set; }
    }
}
