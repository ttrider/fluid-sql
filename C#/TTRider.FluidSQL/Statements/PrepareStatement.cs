using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql
{
    public class PrepareStatement : Token, IExecutableStatement, IStatement
    {
        public PrepareStatement()
        {
            Target = new ExecParameter();
        }
        public Name Name { get; set; }
        public ExecParameter Target { get; set; }
     }
}
