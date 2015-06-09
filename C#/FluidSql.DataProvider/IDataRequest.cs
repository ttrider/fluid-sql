using System.Collections.Generic;

namespace TTRider.FluidSql.DataProvider
{
    public interface IDataRequest
    {
        /// <summary>
        /// connection string
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// buffering mode 
        /// </summary>
        DataRequestMode Mode { get; set; }

        /// <summary>
        /// main statement
        /// </summary>
        IStatement Statement { get; set; }
    }
}
