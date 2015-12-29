using System.Collections.Generic;

namespace TTRider.FluidSql
{
    public class FunctionExpressionToken : SequenceToken
    {
        public List<Token> Arguments { get { return base.Set; } }
    }
}