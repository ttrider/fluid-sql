// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
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