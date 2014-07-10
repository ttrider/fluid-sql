using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class DeleteStatement : RecordsetStatement
                                 , IJoinStatement
    {
        public DeleteStatement()
        {
            this.Joins = new List<Join>();
        }

        public Token From { get; set; }
        public Top Top { get; set; }
        public Token Where { get; set; }
        public List<Join> Joins { get; private set; }
    }
}