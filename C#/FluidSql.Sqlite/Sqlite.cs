// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTRider.FluidSql.Statements;

namespace TTRider.FluidSql
{
    public class Sqlite : Sql
    {
        public static VacuumStatement Vacuum {
            get
            {
                return new VacuumStatement();
            } 
        }
    }
}
