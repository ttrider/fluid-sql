using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using TTRider.FluidSql.Providers;

namespace TTRider.FluidSql.Cache
{
    public class CacheProvider : IProvider
    {
        private readonly IProvider dataProvider;
        private readonly CacheAdapter cache;

        public CacheProvider([NotNull] IProvider dataProvider, [NotNull] IMemoryCache memoryCache)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));
            if (memoryCache == null) throw new ArgumentNullException(nameof(memoryCache));
            this.dataProvider = dataProvider;
            this.cache = new MemoryCacheAdapter(memoryCache);
        }
        public CacheProvider([NotNull] IProvider dataProvider, [NotNull] IDistributedCache distributedCache)
        {
            if (dataProvider == null) throw new ArgumentNullException(nameof(dataProvider));
            if (distributedCache == null) throw new ArgumentNullException(nameof(distributedCache));
            this.dataProvider = dataProvider;
            this.cache = new DistributedCacheAdapter(distributedCache);
        }

        #region Overrides of Provider

        public string GenerateStatement(IStatement statement)
        {
            return this.dataProvider.GenerateStatement(statement);
        }

        public IEnumerable<DbParameter> GetParameters(IStatement statement)
        {
            return this.dataProvider.GetParameters(statement);
        }

        public IDbConnection GetConnection(string connectionString)
        {
            return this.dataProvider.GetConnection(connectionString);
        }

        public Name GetTemporaryTableName(Name name = null)
        {
            return this.dataProvider.GetTemporaryTableName(name);
        }


        public IDbCommand GetCommand(IStatement statement, string connectionString = null)
        {
            return this.GetCommandAsync(statement, connectionString, CancellationToken.None).Result;
        }


        public Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString)
        {
            return this.GetCommandAsync(statement, connectionString, CancellationToken.None);
        }

        public async Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString, CancellationToken token)
        {
            // we support only SELECT statement for now
            if (!(statement is SelectStatement))
            {
                return await this.dataProvider.GetCommandAsync(statement, connectionString, token);
            }

            // we need to make sure we have a cached data for this select
            // to do this, we are going to create a statement without fields and attach parameters to it
            // then, we will figure out what columns we have already and what columns we still need to get


            var dataCommand = await this.dataProvider.GetCommandAsync(statement, connectionString, token);


            return new CacheDbCommand(statement, connectionString, dataCommand);
        }

        #endregion
    }


    class CacheDbCommand : IDbCommand
    {
        private IStatement statement;
        private string connectionString;
        private IDbCommand dataCommand;

        public CacheDbCommand([NotNull] IStatement statement, [NotNull] string connectionString, [NotNull] IDbCommand dataCommand)
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            if (dataCommand == null) throw new ArgumentNullException(nameof(dataCommand));
            this.statement = statement;
            this.connectionString = connectionString;
            this.dataCommand = dataCommand;
        }

        #region Implementation of IDbCommand

        void IDbCommand.Cancel()
        {
            this.dataCommand.Cancel();
        }

        IDbDataParameter IDbCommand.CreateParameter()
        {
            return this.dataCommand.CreateParameter();
        }

        int IDbCommand.ExecuteNonQuery()
        {
            return this.dataCommand.ExecuteNonQuery();
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            return this.dataCommand.ExecuteReader();
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            return this.dataCommand.ExecuteReader(behavior);
        }

        object IDbCommand.ExecuteScalar()
        {
            return this.dataCommand.ExecuteScalar();
        }

        void IDbCommand.Prepare()
        {
            this.dataCommand.Prepare();
        }

        string IDbCommand.CommandText
        {
            get => this.dataCommand.CommandText;
            set => this.dataCommand.CommandText = value;
        }

        int IDbCommand.CommandTimeout
        {
            get => this.dataCommand.CommandTimeout;
            set => this.dataCommand.CommandTimeout = value;
        }

        CommandType IDbCommand.CommandType
        {
            get => this.dataCommand.CommandType;
            set => this.dataCommand.CommandType = value;
        }

        IDbConnection IDbCommand.Connection
        {
            get => this.dataCommand.Connection;
            set => this.dataCommand.Connection = value;
        }

        IDataParameterCollection IDbCommand.Parameters => this.dataCommand.Parameters;

        IDbTransaction IDbCommand.Transaction
        {
            get => this.dataCommand.Transaction;
            set => this.dataCommand.Transaction = value;
        }

        UpdateRowSource IDbCommand.UpdatedRowSource
        {
            get => this.dataCommand.UpdatedRowSource;
            set => this.dataCommand.UpdatedRowSource = value;
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            this.dataCommand.Dispose();
        }

        #endregion

    }


    class CacheAdapter
    {
        
    }

    class MemoryCacheAdapter : CacheAdapter
    {
        private readonly IMemoryCache cache;

        internal MemoryCacheAdapter(IMemoryCache cache)
        {
            this.cache = cache;
        }
    }

    class DistributedCacheAdapter : CacheAdapter
    {
        private readonly IDistributedCache cache;

        internal DistributedCacheAdapter(IDistributedCache cache)
        {
            this.cache = cache;
        }

    }
}
