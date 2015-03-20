namespace TTRider.FluidSql
{
    public class StringifyStatement : Token, IStatement
    {
        public IStatement Content { get; set; }
    }
}