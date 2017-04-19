// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class AlterSchemaStatement : IStatement
    {
        public Name Name { get; set; }

        public Name NewName { get; set; }

        public Name NewOwner { get; set; }
    }
}
