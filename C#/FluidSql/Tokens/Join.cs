// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class Join : Token
    {
        public Joins Type { get; set; }

        public Token Source { get; set; }

        public Token On { get; set; }
    }
}