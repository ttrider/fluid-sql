namespace TTRider.FluidSql
{
    public class ExecParameter
    {
        public ExecParameter()
        {
        }

        public ExecParameter(Name name)
        {
            Name = name;
        }

        public ExecParameter(IStatement target)
        {
            Target = target;
        }

        public Name Name { get; set; }
        public IStatement Target { get; set; }
    }
}
