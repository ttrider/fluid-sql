// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class ThrowStatement : Token, IStatement
    {
        public Token ErrorNumber { get; set; }
        public Token Message { get; set; }
        public Token State { get; set; }
    }
}