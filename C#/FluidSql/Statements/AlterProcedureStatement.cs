namespace TTRider.FluidSql
{
    public class AlterProcedureStatement : IProcedureStatement, IStatement
    {
        public bool CreateIfNotExists { get; set; }
        public Name Name { get; set; }
        public ParameterSet Parameters { get; } = new ParameterSet();
        public ParameterSet Declarations { get; } = new ParameterSet();
        public IStatement Body { get; set; }
        public bool Recompile { get; set; }
    }
}