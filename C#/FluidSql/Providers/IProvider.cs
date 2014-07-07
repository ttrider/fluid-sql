using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;


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


#if _ASYNC_
        System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString, CancellationToken token);

        System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString);

        [Obsolete]
        System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement, CancellationToken token);
        [Obsolete]
        System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement);
#endif
    }
}
