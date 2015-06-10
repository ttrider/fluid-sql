// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace TTRider.FluidSql.RequestResponse
{
    public interface IDataRequest
    {
        IEnumerable<IDbCommand> PrerequisiteCommands { get; }

        /// <summary>
        /// buffering mode 
        /// </summary>
        DataRequestMode Mode { get; }

        /// <summary>
        /// main statement
        /// </summary>
        IDbCommand Command { get; }

        IDataResponse GetResponse(string connectionString = null, DataRequestMode? mode = null);
        Task<IDataResponse> GetResponseAsync(string connectionString = null, DataRequestMode? mode = null);
    }
}
