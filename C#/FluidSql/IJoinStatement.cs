using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public interface IJoinStatement
    {
        List<Join> Joins { get; }
    }
}