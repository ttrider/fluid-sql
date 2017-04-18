// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class CreateIndexStatement : IIndex, IStatement
        , IWhereStatement
    {
        public CreateIndexStatement()
        {
            this.Columns = new List<Order>();
            this.Include = new List<Name>();
            this.With = new IndexOptions();
        }

        public Name Name { get; set; }

        public Name On { get; set; }

        public bool Unique { get; set; }

        public bool? Clustered { get; set; }

        public List<Order> Columns { get; private set; }

        public List<Name> Include { get; private set; }

        public bool? Nonclustered
        {
            get
            {
                if (this.Clustered.HasValue)
                {
                    return !this.Clustered.Value;
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    this.Clustered = !value.Value;
                }
                this.Clustered = null;
            }
        }

        public ICreateOrAlterIndexOptions With { get; private set; }
        public bool CheckIfNotExists { get; set; }
        public Token Where { get; set; }

        //TODO [ FILESTREAM_ON { filestream_filegroup_name | partition_scheme_name | "NULL" } ]

        //TODO:[ ON { partition_scheme_name ( column_name ) | filegroup_name | default }
    }
}