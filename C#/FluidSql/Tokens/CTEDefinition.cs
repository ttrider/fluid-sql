using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql
{
    public class CTEDeclaration
    {
        public CTEDeclaration()
        {
            this.Columns = new List<Name>();
        }
        public string Name { get; set; }
        public List<Name> Columns { get; set; }

        public bool Recursive { get; set; }
        public CTEDefinition PreviousCommonTableExpression { get; set; }
    }
    public class CTEDefinition : Token
    {
        public CTEDeclaration Declaration { get; set; }
        public SelectStatement Definition { get; set; }
    }
}
