// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class CreateSchemaStatement : IStatement
    {
        public Name Name { get; set; }

        public bool CheckIfNotExists { get; set; }

        public string Owner { get; set; }
    }
}
