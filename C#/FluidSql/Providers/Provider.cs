// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers
{
    public abstract class Provider : IProvider
    {
        public virtual string GenerateStatement(IStatement statement)
        {
            return this.Compile(statement).Value;
        }

        public virtual IEnumerable<DbParameter> GetParameters(IStatement statement)
        {
            return GetDbParameters(this.Compile(statement));
        }

        public abstract IDbCommand GetCommand(IStatement statement, string connectionString = null);

        public abstract IDbConnection GetConnection(string connectionString);

        public abstract Name GetTemporaryTableName(Name name = null);
        protected abstract VisitorState Compile(IStatement statement);
        protected abstract IEnumerable<DbParameter> GetDbParameters(VisitorState state);

        public abstract Task<IDbCommand> GetCommandAsync(IStatement statement,
            string connectionString, CancellationToken token);

        public Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString)
        {
            return this.GetCommandAsync(statement, connectionString, CancellationToken.None);
        }
    }
}