// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
//     Copyright (c) 2014-2017 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers
{
    public interface IProvider
    {
        string GenerateStatement(IStatement statement);
        IEnumerable<DbParameter> GetParameters(IStatement statement);

        IDbCommand GetCommand(IStatement statement, string connectionString = null);

        IDbConnection GetConnection(string connectionString);

        Name GetTemporaryTableName(Name name = null);


        Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString,
            CancellationToken token);

        Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString);
    }
}