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
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace TTRider.FluidSql.Providers.SqlServer
{
    public class SqlServerProvider : Provider
    {
        internal static readonly Dictionary<SqlDbType, CommonDbType> SqlDbTypeToCommonDbType = new Dictionary
            <SqlDbType, CommonDbType>
        {
            { SqlDbType.BigInt, CommonDbType.BigInt },
            { SqlDbType.Binary, CommonDbType.Binary },
            { SqlDbType.Bit, CommonDbType.Bit },
            { SqlDbType.Char, CommonDbType.Char },
            { SqlDbType.DateTime, CommonDbType.DateTime },
            { SqlDbType.Decimal, CommonDbType.Decimal },
            { SqlDbType.Float, CommonDbType.Float },
            { SqlDbType.Image, CommonDbType.Image },
            { SqlDbType.Int, CommonDbType.Int },
            { SqlDbType.Money, CommonDbType.Money },
            { SqlDbType.NChar, CommonDbType.NChar },
            { SqlDbType.NText, CommonDbType.NText },
            { SqlDbType.NVarChar, CommonDbType.NVarChar },
            { SqlDbType.Real, CommonDbType.Real },
            { SqlDbType.UniqueIdentifier, CommonDbType.UniqueIdentifier },
            { SqlDbType.SmallDateTime, CommonDbType.SmallDateTime },
            { SqlDbType.SmallInt, CommonDbType.SmallInt },
            { SqlDbType.SmallMoney, CommonDbType.SmallMoney },
            { SqlDbType.Text, CommonDbType.Text },
            { SqlDbType.Timestamp, CommonDbType.Timestamp },
            { SqlDbType.TinyInt, CommonDbType.TinyInt },
            { SqlDbType.VarBinary, CommonDbType.VarBinary },
            { SqlDbType.VarChar, CommonDbType.VarChar },
            { SqlDbType.Variant, CommonDbType.Variant },
            { SqlDbType.Xml, CommonDbType.Xml },
            { SqlDbType.Date, CommonDbType.Date },
            { SqlDbType.Time, CommonDbType.Time },
            { SqlDbType.DateTimeOffset, CommonDbType.DateTimeOffset }
        };

        internal static readonly Dictionary<CommonDbType, SqlDbType> CommonDbTypeToSqlDbType = new Dictionary
            <CommonDbType, SqlDbType>
        {
            { CommonDbType.BigInt, SqlDbType.BigInt },
            { CommonDbType.Binary, SqlDbType.Binary },
            { CommonDbType.Bit, SqlDbType.Bit },
            { CommonDbType.Char, SqlDbType.Char },
            { CommonDbType.DateTime, SqlDbType.DateTime },
            { CommonDbType.Decimal, SqlDbType.Decimal },
            { CommonDbType.Float, SqlDbType.Float },
            { CommonDbType.Image, SqlDbType.Image },
            { CommonDbType.Int, SqlDbType.Int },
            { CommonDbType.Money, SqlDbType.Money },
            { CommonDbType.NChar, SqlDbType.NChar },
            { CommonDbType.NText, SqlDbType.NText },
            { CommonDbType.NVarChar, SqlDbType.NVarChar },
            { CommonDbType.Real, SqlDbType.Real },
            { CommonDbType.UniqueIdentifier, SqlDbType.UniqueIdentifier },
            { CommonDbType.SmallDateTime, SqlDbType.SmallDateTime },
            { CommonDbType.SmallInt, SqlDbType.SmallInt },
            { CommonDbType.SmallMoney, SqlDbType.SmallMoney },
            { CommonDbType.Text, SqlDbType.Text },
            { CommonDbType.Timestamp, SqlDbType.Timestamp },
            { CommonDbType.TinyInt, SqlDbType.TinyInt },
            { CommonDbType.VarBinary, SqlDbType.VarBinary },
            { CommonDbType.VarChar, SqlDbType.VarChar },
            { CommonDbType.Variant, SqlDbType.Variant },
            { CommonDbType.Xml, SqlDbType.Xml },
            { CommonDbType.Date, SqlDbType.Date },
            { CommonDbType.Time, SqlDbType.Time },
            { CommonDbType.DateTimeOffset, SqlDbType.DateTimeOffset }
        };

        public int CommandTimeout { get; set; }

        protected override VisitorState Compile(IStatement statement)
        {
            return new SqlServerVisitor().Compile(statement);
        }

        public override IDbConnection GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var csb = new SqlConnectionStringBuilder(connectionString);

            return new SqlConnection(csb.ConnectionString);
        }


        public override IDbCommand GetCommand(IStatement statement, string connectionString = null)
        {
            var state = this.Compile(statement);

            SqlCommand command;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var csb = new SqlConnectionStringBuilder(connectionString);

                var connection = new SqlConnection(csb.ConnectionString);
                connection.Open();
                command = connection.CreateCommand();
#if !BUILD_CORECLR
                command.Disposed += (s, o) => connection.Close();
#endif
            }
            else
            {
                command = new SqlCommand();
            }

            command.CommandTimeout = this.CommandTimeout;
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
            return SqlServerVisitor.GetTempTableName(name ?? Sql.Name(Guid.NewGuid().ToString("N")));
        }

        public override async System.Threading.Tasks.Task<IDbCommand> GetCommandAsync(IStatement statement,
            string connectionString, CancellationToken token)
        {
            var state = this.Compile(statement);

            var csb = new SqlConnectionStringBuilder(connectionString);

            var connection = new SqlConnection(csb.ConnectionString);

            await connection.OpenAsync(token);

            var command = connection.CreateCommand();

#if !BUILD_CORECLR
                command.Disposed += (s, o) => connection.Close();
#endif

            command.CommandTimeout = this.CommandTimeout;
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
                    var inferType = false;
                    var sp = new SqlParameter
                    {
                        ParameterName = p.Name,
                        Direction = p.Direction
                    };
                    if (p.DbType.HasValue)
                    {
                        sp.SqlDbType = CommonDbTypeToSqlDbType[p.DbType.Value];
                    }
                    else
                    {
                        inferType = true;
                    }

                    if (p.Length.HasValue)
                    {
                        sp.Size = p.Length.Value;
                    }

                    if (p.Precision.HasValue)
                    {
                        sp.Precision = p.Precision.Value;
                    }

                    if (p.Scale.HasValue)
                    {
                        sp.Scale = p.Scale.Value;
                    }

                    var value = state.ParameterValues.FirstOrDefault(pp => string.Equals(pp.Name, p.Name));
                    if (value?.Value != null)
                    {
                        sp.Value = value.Value;

                        if (inferType)
                        {
                            sp.SqlDbType = CommonDbTypeToSqlDbType[Parameter.GetCommonDbTypeFromValue(sp.Value)];
                        }
                    }
                    else if (p.DefaultValue != null)
                    {
                        sp.Value = p.DefaultValue;

                        if (inferType)
                        {
                            sp.SqlDbType = CommonDbTypeToSqlDbType[Parameter.GetCommonDbTypeFromValue(sp.Value)];
                        }
                    }
                    return sp;
                });
        }
    }
}