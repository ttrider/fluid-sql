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
using TTRider.FluidSql.Providers.PostgreBased;

namespace TTRider.FluidSql.Providers.Redshift
{
    public class RedshiftSQLProvider : PostgreBasedSQLProvider
    {
        internal static readonly Dictionary<CommonDbType, NpgsqlDbType> CommonDbTypeToDbType = new Dictionary
            <CommonDbType, NpgsqlDbType>
        {
            {CommonDbType.BigInt, NpgsqlDbType.Bigint},
            {CommonDbType.Binary, NpgsqlDbType. Text},
            {CommonDbType.Bit, NpgsqlDbType.Boolean},
            {CommonDbType.Char, NpgsqlDbType.Char},
            {CommonDbType.DateTime, NpgsqlDbType.Text},
            {CommonDbType.Decimal, NpgsqlDbType.Numeric},
            {CommonDbType.Float,NpgsqlDbType.Real},
            {CommonDbType.Image,NpgsqlDbType.Text},
            {CommonDbType.Int,NpgsqlDbType.Integer},
            {CommonDbType.Money,NpgsqlDbType.Money},
            {CommonDbType.NChar,NpgsqlDbType.Char},
            {CommonDbType.NText,NpgsqlDbType.Text},
            {CommonDbType.NVarChar,NpgsqlDbType.Text},
            {CommonDbType.Real,NpgsqlDbType.Double},
            {CommonDbType.UniqueIdentifier,NpgsqlDbType.Text},
            {CommonDbType.SmallDateTime,NpgsqlDbType.Text},
            {CommonDbType.SmallInt,NpgsqlDbType.Smallint},
            {CommonDbType.SmallMoney,NpgsqlDbType.Real},
            {CommonDbType.Text,NpgsqlDbType.Text},
            {CommonDbType.Timestamp,NpgsqlDbType.Text},
            {CommonDbType.TinyInt,NpgsqlDbType.Smallint},
            {CommonDbType.VarBinary,NpgsqlDbType.Text},
            {CommonDbType.VarChar,NpgsqlDbType.Varchar},
            {CommonDbType.Variant,NpgsqlDbType.Unknown},
            {CommonDbType.Xml,NpgsqlDbType.Xml},
            {CommonDbType.Date,NpgsqlDbType.Text},
            {CommonDbType.Time,NpgsqlDbType.Text},
            {CommonDbType.DateTimeOffset,NpgsqlDbType.Text},
        };

        protected override VisitorState Compile(IStatement statement)
        {
            return new RedshiftSQLVisitor().Compile(statement);
        }
    }
}