namespace TTRider.FluidSql.DataProvider
{
    public class DataRequest : IDataRequest
    {
        public DataRequest(IStatement statement = null)
        {
            this.Statement = statement;
        }

        public string ConnectionString { get; set; }

        /// <summary>
        /// reuse buffer for each record in recordset
        /// </summary>
        public DataRequestMode Mode { get; set; }

        /// <summary>
        /// main statement
        /// </summary>
        public IStatement Statement { get; set; }
    }
}
