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

        VisitorState Compile(IStatement statement)
        {
            return new SqlServerVisitor().Compile(statement, new VisitorState());
        }


        public override string GenerateStatement(IStatement statement)
        {
            var state = this.Compile(statement);
            return state.Value;
        }

        public override IEnumerable<DbParameter> GetParameters(IStatement statement)
        {
            var state = this.Compile(statement);

            return state.GetDbParameters();
        }

        public override IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            var csb = new SqlConnectionStringBuilder(connectionString) {AsynchronousProcessing = true};

            return new SqlConnection(csb.ConnectionString);
        }


        public override IDbCommand GetCommand(IStatement statement, string connectionString = null)
        {
            var state = this.Compile(statement);

            SqlCommand command;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var csb = new SqlConnectionStringBuilder(connectionString) {AsynchronousProcessing = true};

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
        public override async System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(IStatement statement,
            string connectionString, CancellationToken token)
        {
            var state = this.Compile(statement);

            var csb = new SqlConnectionStringBuilder(connectionString) {AsynchronousProcessing = true};

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