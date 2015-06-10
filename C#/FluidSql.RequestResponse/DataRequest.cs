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
using TTRider.FluidSql.Providers;

namespace TTRider.FluidSql.RequestResponse
{
    public class DataRequest : IDataRequest
    {
        static readonly ConcurrentDictionary<Type, IProvider> Providers = new ConcurrentDictionary<Type, IProvider>();

        private DataRequestProperties properties;
        private IEnumerable<IDbCommand> prerequisiteCommands;
        private IDbCommand command;


        public static IDataRequest Create(DataRequestProperties properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            if (properties.QueryProvider == null) throw new ArgumentException("properties.QueryProvider");
            if (properties.Statement == null) throw new ArgumentException("properties.Statement");

            return new DataRequest()
            {
                properties = properties.Clone()
            };
        }

        public static IDataRequest Create<T>(DataRequestProperties properties)
            where T : IProvider
        {
            if (properties == null) throw new ArgumentNullException("properties");
            if (properties.Statement == null) throw new ArgumentException("properties.Statement");

            var prop = properties.Clone();
            prop.QueryProvider = Providers.GetOrAdd(typeof(T), t => Activator.CreateInstance<T>());
            return new DataRequest()
            {
                properties = prop
            };
        }


        public static IDataRequest Create<T>(IStatement statement, DataRequestMode mode, string connectionString = null, IEnumerable<IStatement> prerequisiteStatements = null)
            where T : IProvider
        {
            if (statement == null) throw new ArgumentNullException("statement");
            return Create(Providers.GetOrAdd(typeof (T), t => Activator.CreateInstance<T>()), statement, mode,
                connectionString, prerequisiteStatements);
        }

        public static IDataRequest Create<T>(IStatement statement, string connectionString = null, IEnumerable<IStatement> prerequisiteStatements = null)
            where T : IProvider
        {
            if (statement == null) throw new ArgumentNullException("statement");
            return Create(Providers.GetOrAdd(typeof (T), t => Activator.CreateInstance<T>()), statement,
                DataRequestMode.NoBufferReuseMemory, connectionString, prerequisiteStatements);
        }

        public static IDataRequest Create(IProvider queryProvider, IStatement statement, string connectionString = null, IEnumerable<IStatement> prerequisiteStatements = null)
        {
            if (queryProvider == null) throw new ArgumentNullException("queryProvider");
            if (statement == null) throw new ArgumentNullException("statement");
            return Create(queryProvider, statement,
                DataRequestMode.NoBufferReuseMemory, connectionString, prerequisiteStatements);
        }

        public static IDataRequest Create(IProvider queryProvider, IStatement statement, DataRequestMode mode, string connectionString = null, IEnumerable<IStatement> prerequisiteStatements = null)
        {
            if (queryProvider == null) throw new ArgumentNullException("queryProvider");
            if (statement == null) throw new ArgumentNullException("statement");
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
            return new DataRequest()
            {
                properties = properties
            };
        }

        public IEnumerable<IDbCommand> PrerequisiteCommands
        {
            get
            {
                return this.prerequisiteCommands ??
                       (this.prerequisiteCommands = this.properties.GetPrerequiseteCommands());
            }
        }

        public IDbCommand Command
        {
            get { return this.command ?? (this.command = this.properties.GetCommand()); }
        }

        /// <summary>
        /// reuse buffer for each record in recordset
        /// </summary>
        public DataRequestMode Mode 
        {
            get { return this.properties.Mode; } 
        }

        public IDataResponse GetResponse(string connectionString = null, DataRequestMode? mode = null)
        {
            if (connectionString != null || mode!=null)
            {
                //we need to clone this data request with new connection information

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
                return dr.GetResponse();
            }

            // ensure that all commands will share a connection 
            foreach (var dbCommand in this.PrerequisiteCommands)
            {
                dbCommand.Connection = this.Command.Connection;
            }

            return DataResponse.GetResponse(this);
        }

        public async Task<IDataResponse> GetResponseAsync(string connectionString = null, DataRequestMode? mode = null)
        {
            if (connectionString != null || mode != null)
            {
                //we need to clone this data request with new connection information

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
                return await dr.GetResponseAsync();
            }

            if (this.command == null)
            {
                this.command = await this.properties.GetCommandAsync();
            }

            foreach (var dbCommand in this.PrerequisiteCommands)
            {
                dbCommand.Connection = this.command.Connection;
            }

            return await DataResponse.GetResponseAsync(this);
        }
    }
}
