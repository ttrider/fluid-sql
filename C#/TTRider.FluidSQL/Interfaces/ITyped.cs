// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public interface ITyped
    {
        CommonDbType? DbType { get; set; }
        byte? Precision { get; set; }
        byte? Scale { get; set; }
        int? Length { get; set; }
    }
}