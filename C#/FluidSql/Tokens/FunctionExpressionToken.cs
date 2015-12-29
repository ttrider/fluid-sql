using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class FunctionExpressionToken : ExpressionToken
    {
        public FunctionExpressionToken()
        {
            this.Arguments = new List<Token>();
        }
        public List<Token> Arguments { get; private set; }

    }
}