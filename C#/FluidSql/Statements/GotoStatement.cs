// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class GotoStatement : Token, IStatement
    {
        public string Label { get; set; }
    }
}