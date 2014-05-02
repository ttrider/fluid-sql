using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql
{
    public class DropTableStatement : IStatement
    {
        public Name Name { get; set; }
        public bool CheckExists { get; set; }
    }
}
