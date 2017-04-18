// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class ExecuteStatement : Token, IStatement
    {
        public ExecuteStatement()
        {
            Target = new ExecParameter();
        }
        public Name Name { get; set; }
        public ExecParameter Target { get; set; }
    }
}