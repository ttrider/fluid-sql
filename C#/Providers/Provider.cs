using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers
{
    public abstract class Provider : IProvider
    {
        public abstract string GenerateStatement(IStatement statement);
        public abstract IEnumerable<DbParameter> GetParameters(IStatement statement);
        public abstract IDbCommand GetCommand(string connectionString, IStatement statement);


        public abstract Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement, CancellationToken token);

        public Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement)
        {
            return this.GetCommandAsync(connectionString, statement, CancellationToken.None);
        }
    
    
    }
}
