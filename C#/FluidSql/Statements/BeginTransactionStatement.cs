// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class BeginTransactionStatement : TransactionStatement
    {
        public string Description { get; set; }

        public IsolationLevelType? IsolationLevel { get; set; }

        public TransactionType? Type { get; set; }

        public TransactionAccessType? AccessType { get; set; }
    }
}