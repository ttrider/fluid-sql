namespace TTRider.FluidSql
{
    public class DropTableStatement : IStatement
    {
        public Name Name { get; set; }
        public bool CheckExists { get; set; }
        public bool IsTemporary { get; set; }
    }
}