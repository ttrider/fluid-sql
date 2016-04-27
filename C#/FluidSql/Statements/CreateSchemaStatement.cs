// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class CreateSchemaStatement : IStatement
    {
        public Name Name { get; set; }

        public bool CheckIfNotExists { get; set; }

        public string Owner { get; set; }
    }
}