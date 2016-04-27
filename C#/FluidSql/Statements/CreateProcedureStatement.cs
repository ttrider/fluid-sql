namespace TTRider.FluidSql
{
    public class CreateProcedureStatement : IProcedureStatement, IStatement
    {
        public Name Name { get; set; }
        public bool CheckIfNotExists { get; set; }
        public ParameterSet Parameters { get; } = new ParameterSet();
        public IStatement Body { get; set; }
        public bool Recompile { get; set; }
    }
}