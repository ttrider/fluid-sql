namespace TTRider.FluidSql
{
    public class CaseWhenToken : Token
    {
        public ExpressionToken WhenToken { get; set; }
        public ExpressionToken ThenToken { get; set; }
    }
}