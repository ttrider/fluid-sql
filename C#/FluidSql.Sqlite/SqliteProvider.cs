using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.Sqlite
{
    public class SqliteProvider : Provider
    {
        VisitorState Compile(IStatement statement)
        {
            return new SqliteVisitor().Compile(statement, new VisitorState());
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
            return new SQLiteConnection(connectionString);
        }


        public override IDbCommand GetCommand(IStatement statement, string connectionString = null)
        {
            var state = this.Compile(statement);

            SQLiteCommand command;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var connection = new SQLiteConnection(connectionString);
                connection.Open();
                command = connection.CreateCommand();
                command.Disposed += (s, o) => connection.Close();
            }
            else
            {
                command = new SQLiteCommand();
            }

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
            return Sql.Name("temp", (name ?? Sql.Name(Guid.NewGuid().ToString("N"))).LastPart);
        }

        public override async Task<IDbCommand> GetCommandAsync(IStatement statement,
            string connectionString, CancellationToken token)
        {
            var state = this.Compile(statement);

            var csb = new SQLiteConnectionStringBuilder(connectionString);

            var connection = new SQLiteConnection(csb.ConnectionString);

            await connection.OpenAsync(token);

            var command = connection.CreateCommand();

            command.Disposed += (s, o) => connection.Close();

            command.CommandType = CommandType.Text;
            command.CommandText = state.Value;
            foreach (var parameter in state.GetDbParameters())
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }
    }
}
