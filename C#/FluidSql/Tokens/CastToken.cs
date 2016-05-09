// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class CastToken : ExpressionToken, ITyped
    {
        public CommonDbType? DbType { get; set; }
        public byte? Precision { get; set; }
        public byte? Scale { get; set; }
        public int? Length { get; set; }
        public ExpressionToken Token { get; set; }
    }
}