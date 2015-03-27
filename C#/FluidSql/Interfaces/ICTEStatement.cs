using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql
{
    interface ICTEStatement
    {
        List<CTEDefinition> CommonTableExpressions { get; set; }
    }
}
