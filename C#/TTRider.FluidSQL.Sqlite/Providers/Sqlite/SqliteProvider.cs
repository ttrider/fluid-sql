// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.Sqlite
{
    public class SqliteProvider : Provider
    {
        internal static readonly Dictionary<CommonDbType, DbType> CommonDbTypeToDbType = new Dictionary
            <CommonDbType, DbType>
        {
            { CommonDbType.BigInt, DbType.Int64 },
            { CommonDbType.Binary, DbType.Binary },
            { CommonDbType.Bit, DbType.Boolean },
            { CommonDbType.Char, DbType.AnsiStringFixedLength },
            { CommonDbType.DateTime, DbType.DateTime },
            { CommonDbType.Decimal, DbType.Decimal },
            { CommonDbType.Float, DbType.Single },
            { CommonDbType.Image, DbType.Binary },
            { CommonDbType.Int, DbType.Int32 },
            { CommonDbType.Money, DbType.Currency },
            { CommonDbType.NChar, DbType.StringFixedLength },
            { CommonDbType.NText, DbType.String },
            { CommonDbType.NVarChar, DbType.String },
            { CommonDbType.Real, DbType.Double },
            { CommonDbType.UniqueIdentifier, DbType.Guid },
            { CommonDbType.SmallDateTime, DbType.DateTime },
            { CommonDbType.SmallInt, DbType.Int16 },
            { CommonDbType.SmallMoney, DbType.Single },
            { CommonDbType.Text, DbType.AnsiString },
            { CommonDbType.Timestamp, DbType.Decimal },
            { CommonDbType.TinyInt, DbType.Byte },
            { CommonDbType.VarBinary, DbType.Binary },
            { CommonDbType.VarChar, DbType.AnsiString },
            { CommonDbType.Variant, DbType.Object },
            { CommonDbType.Xml, DbType.Xml },
            { CommonDbType.Date, DbType.Date },
            { CommonDbType.Time, DbType.Time },
            { CommonDbType.DateTimeOffset, DbType.DateTimeOffset }
        };

        protected override VisitorState Compile(IStatement statement)
        {
            return new SqliteVisitor().Compile(statement);
        }

        public override IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }
            return new SqliteConnection(connectionString);
        }


        public override IDbCommand GetCommand(IStatement statement, string connectionString = null)
        {
            var state = this.Compile(statement);

            SqliteCommand command;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var connection = new SqliteConnection(connectionString);
                connection.Open();
                command = connection.CreateCommand();
                //command.Disposed += (s, o) => connection.Close();
            }
            else
            {
                command = new SqliteCommand();
            }

            command.CommandType = CommandType.Text;
            command.CommandText = state.Value;
            foreach (var parameter in GetDbParameters(state))
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

            var csb = new SqliteConnectionStringBuilder(connectionString);

            var connection = new SqliteConnection(csb.ConnectionString);

            await connection.OpenAsync(token);

            var command = connection.CreateCommand();

            //command.Disposed += (s, o) => connection.Close();

            command.CommandType = CommandType.Text;
            command.CommandText = state.Value;
            foreach (var parameter in GetDbParameters(state))
            {
                command.Parameters.Add(parameter);
            }

            return command;
        }

        protected override IEnumerable<DbParameter> GetDbParameters(VisitorState state)
        {
            // we have a list of parameters, some of them are duplicates
            // some contain type, some not
            // we need to get a final list, preferable with types

            return state.Parameters
                .GroupBy(p => p.Name)
                .Select(pg => pg.FirstOrDefault(p => p.DbType.HasValue) ?? pg.First())
                .Except(state.Variables, ParameterEqualityComparer.Default)
                .Select(p =>
                {
                    var sp = new SqliteParameter
                    {
                        ParameterName = p.Name,
                        Direction = p.Direction
                    };
                    if (p.DbType.HasValue)
                    {
                        sp.DbType = CommonDbTypeToDbType[p.DbType.Value];
                    }
                    if (p.Length.HasValue)
                    {
                        sp.Size = p.Length.Value;
                    }

                    var value = state.ParameterValues.FirstOrDefault(pp => string.Equals(pp.Name, p.Name));
                    if (value != null && value.Value != null)
                    {
                        sp.Value = value.Value;
                    }
                    else if (p.DefaultValue != null)
                    {
                        sp.Value = p.DefaultValue;
                    }
                    return sp;
                });
        }
    }
}