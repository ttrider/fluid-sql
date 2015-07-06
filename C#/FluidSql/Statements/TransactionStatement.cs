// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public abstract class TransactionStatement : IStatement
    {
        public Name Name { get; set; }

        public Parameter Parameter { get; set; }
    }
}