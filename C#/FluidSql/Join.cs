// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public enum Joins
    {
        Inner = 0,
        LeftOuter = 1,
        RightOuter = 2,
        FullOuter = 3,
        Cross = 3,
    }


    public class Join
    {
        public Joins Type { get; set; }

        public Token Source { get; set; }

        public Token On { get; set; }
    }
}
