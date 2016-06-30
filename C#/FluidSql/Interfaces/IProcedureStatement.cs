namespace TTRider.FluidSql
{
    public interface IProcedureStatement
    {
        Name Name { get; set; }

        ParameterSet Parameters { get; }

        IStatement Body { get; set; }

        bool Recompile { get; set; }
    }
}