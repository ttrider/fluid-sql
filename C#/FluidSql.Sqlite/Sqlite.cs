// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql.Statements;

namespace TTRider.FluidSql
{
    public class Sqlite : Sql
    {
        public static VacuumStatement Vacuum => new VacuumStatement();
    }
}