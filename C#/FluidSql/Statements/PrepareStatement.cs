using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql
{
    public class PrepareStatement : Token, IExecutableStatement, IStatement
    {
        public string Name { get; set; }
        public IStatement Target { get; set; }
     }
}
