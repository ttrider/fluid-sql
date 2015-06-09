using System.Collections.Generic;
using System.Data;

namespace TTRider.FluidSql.DataProvider
{
    public interface IDataRequest
    {

        IList<IDbCommand> PrerequisiteCommands { get; }

        /// <summary>
        /// buffering mode 
        /// </summary>
        DataRequestMode Mode { get; }

        /// <summary>
        /// main statement
        /// </summary>
        IDbCommand Command { get; }

        IDataResponse GetResponse();
    }
}
