// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{

    public class CreateViewStatement : IStatement
    {
        public Name Name { get; set; }

        public IStatement DefinitionQuery { get; set; }
    }
}
