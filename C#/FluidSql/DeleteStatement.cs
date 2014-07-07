using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class DeleteStatement : RecordsetStatement
    {
        public Token From { get; set; }
        public Top Top { get; set; }
        public Token Where { get; set; }
    }
}