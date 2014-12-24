// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class DeclareStatement : IStatement
    {
        public Parameter Variable { get; set; }
        public Token Initializer { get; set; }
    }
}