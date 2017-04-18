// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace TTRider.FluidSql.Providers.PostgreBased
{
    public class PostgreBasedSQLProvider : Provider
    {
        protected override VisitorState Compile(IStatement statement)
        {
            return new PostgreBasedSQLVisitor().Compile(statement);
        }

        public override IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }
            return new NpgsqlConnection(connectionString);
        }


        public override IDbCommand GetCommand(IStatement statement, string connectionString = null)
        {
            var state = this.Compile(statement);

            NpgsqlCommand command;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                command = connection.CreateCommand();
            }
            else
            {
                command = new NpgsqlCommand();
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

            var csb = new NpgsqlConnectionStringBuilder(connectionString);

            var connection = new NpgsqlConnection(csb.ConnectionString);

            await connection.OpenAsync(token);

            var command = connection.CreateCommand();


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
                    var sp = new NpgsqlParameter
                    {
                        ParameterName = p.Name,
                        Direction = p.Direction,
                    };
                    if (p.DbType.HasValue)
                    {
                        sp.NpgsqlDbType = CommonDbTypeToDbType[p.DbType.Value];
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


        internal static readonly Dictionary<CommonDbType, NpgsqlDbType> CommonDbTypeToDbType = new Dictionary
            <CommonDbType, NpgsqlDbType>
        {
            {CommonDbType.BigInt, NpgsqlDbType.Bigint},
            {CommonDbType.Binary, NpgsqlDbType.Bytea},
            {CommonDbType.Bit, NpgsqlDbType.Boolean},
            {CommonDbType.Char, NpgsqlDbType.Char},
            {CommonDbType.DateTime, NpgsqlDbType.Timestamp},
            {CommonDbType.Decimal, NpgsqlDbType.Numeric},
            {CommonDbType.Float,NpgsqlDbType.Real},
            {CommonDbType.Image,NpgsqlDbType.Bytea},
            {CommonDbType.Int,NpgsqlDbType.Integer},
            {CommonDbType.Money,NpgsqlDbType.Money},
            {CommonDbType.NChar,NpgsqlDbType.Char},
            {CommonDbType.NText,NpgsqlDbType.Text},
            {CommonDbType.NVarChar,NpgsqlDbType.Text},
            {CommonDbType.Real,NpgsqlDbType.Double},
            {CommonDbType.UniqueIdentifier,NpgsqlDbType.Uuid},
            {CommonDbType.SmallDateTime,NpgsqlDbType.Timestamp},
            {CommonDbType.SmallInt,NpgsqlDbType.Smallint},
            {CommonDbType.SmallMoney,NpgsqlDbType.Real},
            {CommonDbType.Text,NpgsqlDbType.Text},
            {CommonDbType.Timestamp,NpgsqlDbType.Timestamp},
            {CommonDbType.TinyInt,NpgsqlDbType.Smallint},
            {CommonDbType.VarBinary,NpgsqlDbType.Bytea},
            {CommonDbType.VarChar,NpgsqlDbType.Varchar},
            {CommonDbType.Variant,NpgsqlDbType.Unknown},
            {CommonDbType.Xml,NpgsqlDbType.Xml},
            {CommonDbType.Date,NpgsqlDbType.Date},
            {CommonDbType.Time,NpgsqlDbType.Time},
            {CommonDbType.DateTimeOffset,NpgsqlDbType.TimestampTZ},
        };
    }
}