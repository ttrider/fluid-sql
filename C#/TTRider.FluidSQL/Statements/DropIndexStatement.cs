// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql
{
    public class DropIndexStatement : IStatement
    {
        public DropIndexStatement()
        {
            this.With = new IndexOptions();
        }

        public Name Name { get; set; }

        public Name On { get; set; }

        public IDropIndexOptions With { get; private set; }
        public bool CheckExists { get; set; }
    }
}