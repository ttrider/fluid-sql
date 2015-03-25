using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public interface IOrderByStatement
    {
        List<Order> OrderBy { get; }
    }
}