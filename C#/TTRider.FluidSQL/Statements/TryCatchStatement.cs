// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class TryCatchStatement : Token, IStatement
    {
        public IStatement TryStatement { get; set; }
        public IStatement CatchStatement { get; set; }
    }
}