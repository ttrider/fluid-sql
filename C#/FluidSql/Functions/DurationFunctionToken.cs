namespace TTRider.FluidSql
{
    public class DurationFunctionToken : FunctionExpressionToken
    {
        public DatePart DatePart { get; set; }
        public ExpressionToken Start { get; set; }
        public ExpressionToken End { get; set; }
    }
}