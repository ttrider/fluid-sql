// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TTRider.Data.RequestResponse;
using TTRider.FluidSql.Providers;

namespace TTRider.FluidSql.RequestResponse
{
    public class DataRequest : IDbRequest
    {
        static readonly ConcurrentDictionary<Type, IProvider> Providers = new ConcurrentDictionary<Type, IProvider>();
        private IDbCommand command;
        private IEnumerable<IDbCommand> prerequisiteCommands;

        private DataRequestProperties properties;


        public IEnumerable<IDbCommand> PrerequisiteCommands => this.prerequisiteCommands ??
                                                               (this.prerequisiteCommands = this.properties.GetPrerequiseteCommands());

        public IDbCommand Command => this.command ?? (this.command = this.properties.GetCommand());

        /// <summary>
        ///     reuse buffer for each record in recordset
        /// </summary>
        public DbRequestMode Mode => this.properties.Mode;

        public IDbResponse GetResponse()
        {
            // ensure that all commands will share a connection 
            foreach (var dbCommand in this.PrerequisiteCommands)
            {
                dbCommand.Connection = this.Command.Connection;
            }

            return DbResponse.GetResponse(this);
        }

        public async Task<IDbResponse> GetResponseAsync()
        {
            if (this.command == null)
            {
                this.command = await this.properties.GetCommandAsync();
            }

            foreach (var dbCommand in this.PrerequisiteCommands)
            {
                dbCommand.Connection = this.command.Connection;
            }

            return await DbResponse.GetResponseAsync(this);
        }


        public static IDbRequest Create(DataRequestProperties properties)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (properties.QueryProvider == null) throw new ArgumentException("properties.QueryProvider");
            if (properties.Statement == null) throw new ArgumentException("properties.Statement");

            return new DataRequest
            {
                properties = properties.Clone()
            };
        }

        public static IDbRequest Create<T>(DataRequestProperties properties)
            where T : IProvider
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (properties.Statement == null) throw new ArgumentException("properties.Statement");

            var prop = properties.Clone();
            prop.QueryProvider = Providers.GetOrAdd(typeof (T), t => Activator.CreateInstance<T>());
            return new DataRequest
            {
                properties = prop
            };
        }


        public static IDbRequest Create<T>(IStatement statement, DbRequestMode mode, string connectionString = null,
            IEnumerable<IStatement> prerequisiteStatements = null)
            where T : IProvider
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return Create(Providers.GetOrAdd(typeof (T), t => Activator.CreateInstance<T>()), statement, mode,
                connectionString, prerequisiteStatements);
        }

        public static IDbRequest Create<T>(IStatement statement, string connectionString = null,
            IEnumerable<IStatement> prerequisiteStatements = null)
            where T : IProvider
        {
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return Create(Providers.GetOrAdd(typeof (T), t => Activator.CreateInstance<T>()), statement,
                DbRequestMode.NoBufferReuseMemory, connectionString, prerequisiteStatements);
        }

        public static IDbRequest Create(IProvider queryProvider, IStatement statement, string connectionString = null,
            IEnumerable<IStatement> prerequisiteStatements = null)
        {
            if (queryProvider == null) throw new ArgumentNullException(nameof(queryProvider));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            return Create(queryProvider, statement,
                DbRequestMode.NoBufferReuseMemory, connectionString, prerequisiteStatements);
        }

        public static IDbRequest Create(IProvider queryProvider, IStatement statement, DbRequestMode mode,
            string connectionString = null, IEnumerable<IStatement> prerequisiteStatements = null)
        {
            if (queryProvider == null) throw new ArgumentNullException(nameof(queryProvider));
            if (statement == null) throw new ArgumentNullException(nameof(statement));
            var properties = new DataRequestProperties
            {
                Statement = statement,
                Mode = mode,
                ConnectionString = connectionString,
                QueryProvider = queryProvider
            };
            if (prerequisiteStatements != null)
            {
                foreach (var ps in prerequisiteStatements)
                {
                    properties.PrerequisiteStatements.Add(ps);
                }
            }
            return new DataRequest
            {
                properties = properties
            };
        }


        public IDbRequest CreateNew(string connectionString = null, DbRequestMode? mode = null)
        {
            var dr = new DataRequest
            {
                properties = this.properties.Clone()
            };
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                dr.properties.ConnectionString = connectionString;
            }
            if (mode.HasValue)
            {
                dr.properties.Mode = mode.Value;
            }
            return dr;
        }
    }
}