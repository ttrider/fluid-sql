using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql
{
    public class ExecuteStatement : IStatement
    {
        public IStatement Target { get; set; }
    }
}
