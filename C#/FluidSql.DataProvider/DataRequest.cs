using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TTRider.FluidSql.Providers;

namespace TTRider.FluidSql.DataProvider
{
    public class DataRequest : IDataRequest
    {
        static readonly ConcurrentDictionary<Type, IProvider> Providers = new ConcurrentDictionary<Type, IProvider>();


        public static IDataRequest Create<T>(string connectionString, IStatement statement, IEnumerable<IStatement> prerequisiteStatements = null)
            where T : IProvider
        {
            return Create<T>(connectionString, DataRequestMode.NoBufferReuseMemory, statement, prerequisiteStatements);
        }

        public static IDataRequest Create<T>(string connectionString, DataRequestMode mode, IStatement statement, IEnumerable<IStatement> prerequisiteStatements = null)
            where T : IProvider
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");
            if (statement == null) throw new ArgumentNullException("statement");

            var queryProvider = Providers.GetOrAdd(typeof(T), t => Activator.CreateInstance<T>());

            var connection = queryProvider.GetConnection(connectionString);

            connection.Open();

            var command = queryProvider.GetCommand(statement);
            command.Connection = connection;

            var request = new DataRequest
            {
                Command = command,
                Mode = mode
            };

            if (prerequisiteStatements != null)
            {
                foreach (var cmd in prerequisiteStatements
                    .Select(ps => queryProvider.GetCommand(ps)))
                {
                    cmd.Connection = connection;
                    request.PrerequisiteCommands.Add(cmd);
                }
            }

            return request;
        }

        public static IDataRequest Create(IDbCommand command, IEnumerable<IDbCommand> prerequisiteCommands = null)
        {
            return Create(command, DataRequestMode.NoBufferReuseMemory, prerequisiteCommands);
        }

        public static IDataRequest Create(IDbCommand command, DataRequestMode mode, IEnumerable<IDbCommand> prerequisiteCommands = null)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (command.Connection == null) throw new ArgumentException("command.Connection");

            if (command.Connection.State == ConnectionState.Closed)
            {
                command.Connection.Open();
            }

            var request = new DataRequest
            {
                Command = command,
                Mode = mode
            };

            if (prerequisiteCommands != null)
            {
                foreach (var cmd in prerequisiteCommands)
                {
                    cmd.Connection = command.Connection;
                    request.PrerequisiteCommands.Add(cmd);
                }
            }

            return request;
        }



        DataRequest()
        {
            this.PrerequisiteCommands = new List<IDbCommand>();
        }


        public IList<IDbCommand> PrerequisiteCommands { get; private set; }


        public IDbCommand Command { get; private set; }


        /// <summary>
        /// reuse buffer for each record in recordset
        /// </summary>
        public DataRequestMode Mode { get; set; }

        public IDataResponse GetResponse()
        {
            return DataResponse.GetResponse(this);
        }
    }
}
