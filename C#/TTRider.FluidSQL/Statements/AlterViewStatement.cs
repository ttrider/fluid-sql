// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
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