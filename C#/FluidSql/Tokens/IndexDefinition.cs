﻿// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class ConstrainDefinition :Token
    {
        public ConstrainDefinition()
        {
            this.Columns = new List<Order>();
            this.ColumnsName = new List<Name>();
        }

        public Name Name { get; set; }

        public List<Order> Columns { get; private set; }

        public bool Unique { get; set; }

        public bool? Clustered { get; set; }
        public OnConflict? Conflict { get; set; }

        public List<Name> ColumnsName { get; private set; }
    }
}