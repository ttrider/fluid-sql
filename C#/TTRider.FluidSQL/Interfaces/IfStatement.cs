// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class IfStatement : IStatement
    {
        public Token Condition { get; set; }
        public StatementsStatement Then { get; set; }
        public StatementsStatement Else { get; set; }
    }
}