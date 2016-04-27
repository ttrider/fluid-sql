// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class ParameterValue : Token
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }
}