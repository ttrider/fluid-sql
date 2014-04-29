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

        IDbCommand GetCommand(string connectionString, IStatement statement);

        Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement, CancellationToken token);
        Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement);
    }
}
