// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class DropTableStatement : IStatement
    {
        public Name Name { get; set; }
        public bool CheckExists { get; set; }
        public bool IsTemporary { get; set; }
    }
}