using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;


namespace TTRider.FluidSql.Providers.SqlServer
{
    public class SqlServerProvider : Provider
    {
        public int CommandTimeout { get; set; }

        public override string GenerateStatement(IStatement statement)
        {
            var state = SqlServerVisitor.Compile(statement);
            return state.Value;
        }

        public override IEnumerable<DbParameter> GetParameters(IStatement statement)
        {
            var state = SqlServerVisitor.Compile(statement);

            return state.GetDbParameters();
        }

        [Obsolete]
        public override IDbCommand GetCommand(string connectionString, IStatement statement)
        {
            return this.GetCommand(statement, connectionString);
        }

        public override IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            var csb = new SqlConnectionStringBuilder(connectionString) { AsynchronousProcessing = true };

            return new SqlConnection(csb.ConnectionString);
        }


        public override IDbCommand GetCommand(IStatement statement, string connectionString = null)
        {

            var state = SqlServerVisitor.Compile(statement);

            SqlCommand command;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var csb = new SqlConnectionStringBuilder(connectionString) { AsynchronousProcessing = true };

                var connection = new SqlConnection(csb.ConnectionString);
                connection.Open();
                command = connection.CreateCommand();
                command.Disposed += (s, o) => connection.Close();
            }
            else
            {
                command = new SqlCommand();
            }

            command.CommandTimeout = this.CommandTimeout;
            command.CommandType = CommandType.Text;
            command.CommandText = state.Value;
            foreach (var parameter in state.GetDbParameters())
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        public override Name GetTemporaryTableName(Name name = null)
        {
            return SqlServerVisitor.GetTempTableName(name ?? Sql.Name(Guid.NewGuid().ToString("N")));
        }
        
#if _ASYNC_
        public async override System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(IStatement statement, string connectionString, CancellationToken token)
        {
            var state = SqlServerVisitor.Compile(statement);

            var csb = new SqlConnectionStringBuilder(connectionString) { AsynchronousProcessing = true };

            var connection = new SqlConnection(csb.ConnectionString);

            await connection.OpenAsync(token);

            var command = connection.CreateCommand();

            command.Disposed += (s, o) => connection.Close();

            command.CommandTimeout = this.CommandTimeout;
            command.CommandType = CommandType.Text;
            command.CommandText = state.Value;
            foreach (var parameter in state.GetDbParameters())
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }
#endif
        
    }
}
