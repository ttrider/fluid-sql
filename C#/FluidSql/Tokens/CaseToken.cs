using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class CaseToken : ExpressionToken
    {
        public CaseToken()
        {
            this.WhenConditions = new List<CaseWhenToken>();
        }
        public ExpressionToken CaseValueToken { get; set; }
        public List<CaseWhenToken> WhenConditions { get; }
        public ExpressionToken ElseToken { get; set; }
    }
}