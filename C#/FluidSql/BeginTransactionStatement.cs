﻿// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class BeginTransactionStatement : TransactionStatement
    {
        public string Description { get; set; }

        public TransactionType? Type { get; set; }
    }

    public enum TransactionType
    {
        Deferred, 
        Immediate, 
        Exclusive
    }
}