// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class AlterViewStatement : IStatement
    {
        public bool IsTemporary;
        public Name Name { get; set; }
        public IStatement DefinitionStatement { get; set; }
    }
}