// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014, 2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class WhileStatement : Token, IStatement
    {
        public Token Condition { get; set; }
        public StatementsStatement Do { get; set; }
    }
}
