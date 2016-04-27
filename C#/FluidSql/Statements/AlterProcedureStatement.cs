namespace TTRider.FluidSql
{
    public class AlterProcedureStatement : IProcedureStatement, IStatement
    {
        public Name Name { get; set; }
        public bool CreateIfNotExists { get; set; }
        public ParameterSet Parameters { get; } = new ParameterSet();
        public IStatement Body { get; set; }
        public bool Recompile { get; set; }
    }
}