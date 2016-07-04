// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class AlterIndexStatement : Token, IIndex, IStatement
    {
        public AlterIndexStatement()
        {
            this.Columns = new List<Order>();
            this.Set = new IndexOptions();
            this.RebuildWith = new IndexOptions();
        }

        public Name Name { get; set; }

        public Name On { get; set; }

        public bool Unique { get; set; } 

        public bool Rebuild { get; set; }

        public List<Order> Columns { get; private set; }

        public ICreateOrAlterIndexOptions RebuildWith { get; set; }

        public bool Disable { get; set; }

        public bool Reorganize { get; set; }
        //TODO:[ PARTITION = partition_number ][ WITH ( LOB_COMPACTION = { ON | OFF } ) ]

        public ISetIndexOptions Set { get; private set; }

        public bool CheckIfNotExists { get; set; }

        public Token Where { get; set; }
    }
}