// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class DropFunctionStatement : IStatement
    {
        public Name Name { get; set; }
        public bool CheckExists { get; set; }
         //for pgrestr
        public Parameter ReturnValue { get; set; }
        public bool? IsCascade { get; set; }
    }
}