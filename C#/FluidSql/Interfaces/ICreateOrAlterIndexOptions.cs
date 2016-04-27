// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public interface ICreateOrAlterIndexOptions
    {
        bool? PadIndex { get; set; }
        int? Fillfactor { get; set; }
        bool? SortInTempdb { get; set; }
        bool? IgnoreDupKey { get; set; }
        bool? StatisticsNorecompute { get; set; }
        bool? DropExisting { get; set; }
        bool? AllowRowLocks { get; set; }
        bool? AllowPageLocks { get; set; }
        bool? Online { get; set; }
        int? MaxDegreeOfParallelism { get; set; }

        bool IsDefined { get; }
    }
}