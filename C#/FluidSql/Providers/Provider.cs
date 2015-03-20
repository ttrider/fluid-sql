using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace TTRider.FluidSql.Providers
{
    public abstract class Provider : IProvider
    {
        public abstract string GenerateStatement(IStatement statement);
        public abstract IEnumerable<DbParameter> GetParameters(IStatement statement);

        [Obsolete]
        public abstract IDbCommand GetCommand(string connectionString, IStatement statement);

        public abstract IDbCommand GetCommand(IStatement statement, string connectionString = null);

        public abstract IDbConnection GetConnection(string connectionString);

        public abstract Name GetTemporaryTableName(Name name = null);

#if _ASYNC_
        public abstract System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(IStatement statement,
            string connectionString, CancellationToken token);

        public System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString)
        {
            return this.GetCommandAsync(statement, connectionString, CancellationToken.None);
        }

        public System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement,
            CancellationToken token)
        {
            return this.GetCommandAsync(statement, connectionString, token);
        }

        public System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement)
        {
            return this.GetCommandAsync(statement, connectionString, CancellationToken.None);
        }
#endif
    }
}