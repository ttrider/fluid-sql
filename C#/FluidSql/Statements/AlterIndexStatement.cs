// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class AlterIndexStatement : Token, IStatement
    {
        public AlterIndexStatement()
        {
            this.Set = new IndexOptions();
            this.RebuildWith = new IndexOptions();
        }

        public Name Name { get; set; }

        public Name On { get; set; }

        public bool Rebuild { get; set; }

        public ICreateOrAlterIndexOptions RebuildWith { get; set; }

        public bool Disable { get; set; }

        public bool Reorganize { get; set; }
        //TODO:[ PARTITION = partition_number ][ WITH ( LOB_COMPACTION = { ON | OFF } ) ]

        public ISetIndexOptions Set { get; private set; }
    }
}