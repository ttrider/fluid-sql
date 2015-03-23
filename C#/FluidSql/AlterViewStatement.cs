// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class AlterViewStatement : IStatement
    {
        public Name Name { get; set; }
        public IStatement DefinitionStatement { get; set; }

        public bool IsTemporary;
    }
}