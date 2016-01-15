namespace TTRider.FluidSql
{
    public class DateAddFunctionToken : DateFunctionExpressionToken
    {
        public DatePart DatePart { get; set; }

        public Token Number { get; set; }

        public Token Token { get; set; }

        public bool Subtract { get; set;}
    }
}