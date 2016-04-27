namespace TTRider.FluidSql
{
    public class DropProcedureStatement : IStatement
    {
        public Name Name { get; set; }
        public bool CheckExists { get; set; }
    }
}