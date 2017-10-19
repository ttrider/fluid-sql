// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using System;

namespace TTRider.FluidSql
{
    public class WaitforTimeStatement : WaitforStatement
    {
        public DateTime Time { get; set; }
    }
}