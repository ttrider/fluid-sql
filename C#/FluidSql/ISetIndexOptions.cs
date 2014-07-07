// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public interface ISetIndexOptions
    {
        bool? IgnoreDupKey { get; set; }
        bool? StatisticsNorecompute { get; set; }
        bool? AllowRowLocks { get; set; }
        bool? AllowPageLocks { get; set; }

        bool IsDefined { get; }

    }
}