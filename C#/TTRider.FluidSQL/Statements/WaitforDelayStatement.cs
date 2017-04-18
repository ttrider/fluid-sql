// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System;

namespace TTRider.FluidSql
{
    public class WaitforDelayStatement : WaitforStatement
    {
        public TimeSpan Delay { get; set; }
    }
}