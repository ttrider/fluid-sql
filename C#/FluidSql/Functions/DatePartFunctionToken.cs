namespace TTRider.FluidSql
{
    public class DatePartFunctionToken : FunctionExpressionToken
    {
        public DatePart DatePart { get; set; }

        public Token Token { get; set; }
    }
}