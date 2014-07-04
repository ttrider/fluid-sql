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
        
        [Obsolete()]
        IDbCommand GetCommand(string connectionString, IStatement statement);

        IDbCommand GetCommand(IStatement statement, string connectionString = null);

        IDbConnection GetConnection(string connectionString);

        [Obsolete]
        Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement, CancellationToken token);
        [Obsolete]
        Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement);

        Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString, CancellationToken token);
        Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString);
    }
}
