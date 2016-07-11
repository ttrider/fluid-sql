// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public interface IIndex
    {
        Name Name { get; set; }

        Name On { get; set; }

        bool Unique { get; set; }

        List<Order> Columns { get; }

        bool CheckIfNotExists { get; set; }

        Token Where { get; set; }
    }
}
