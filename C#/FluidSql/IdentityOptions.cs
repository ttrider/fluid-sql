// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class IdentityOptions
    {
        public bool On { get; set; }
        public bool NotForReplication { get; set; }
        public int Seed { get; set; }
        public int Increment { get; set; }
    }
}