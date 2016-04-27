// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public interface IDropIndexOptions
    {
        bool? Online { get; set; }
        int? MaxDegreeOfParallelism { get; set; }

        bool IsDefined { get; }
    }
}