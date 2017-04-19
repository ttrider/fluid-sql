// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class CreateViewStatement : IStatement
    {
        public Name Name { get; set; }

        public IStatement DefinitionStatement { get; set; }

        public bool CheckIfNotExists { get; set; }
        public bool IsTemporary { get; set; }
    }
}