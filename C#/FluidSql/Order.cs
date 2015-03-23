// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class Order : Token
    {
        public Name Column { get; set; }
        public Direction Direction { get; set; }
    }
}