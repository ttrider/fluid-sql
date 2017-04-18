// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public enum TransactionType
    {
        Deferred,
        Immediate,
        Exclusive
    }

    public enum IsolationLevelType
    {
        Serializable,
        RepeatableRead,
        ReadCommited,
        ReadUnCommited
    }

    public enum TransactionAccessType
    {
        None,
        ReadWrite,
        ReadOnly       
    }
}