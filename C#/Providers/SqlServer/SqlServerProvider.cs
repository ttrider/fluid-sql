using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.SqlServer
{
    public class SqlServerProvider : Provider
    {
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

        public override IDbCommand GetCommand(string connectionString, IStatement statement)
        {
            var state = SqlServerVisitor.Compile(statement);

            var csb = new SqlConnectionStringBuilder(connectionString) { AsynchronousProcessing = true };

            var connection = new SqlConnection(csb.ConnectionString);
            connection.Open();

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

        public async override Task<IDbCommand> GetCommandAsync(string connectionString, IStatement statement, CancellationToken token)
        {
            var state = SqlServerVisitor.Compile(statement);

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

        public int CommandTimeout { get; set; }
    }
}
