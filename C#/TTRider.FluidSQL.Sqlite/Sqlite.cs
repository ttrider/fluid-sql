// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using TTRider.FluidSql.Statements;

namespace TTRider.FluidSql
{
    public class Sqlite : Sql
    {
        public static VacuumStatement Vacuum => new VacuumStatement();
    }
}