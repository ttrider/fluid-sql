// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider Technologies, Inc.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

// ReSharper disable InconsistentNaming

using System;

namespace TTRider.FluidSql.Providers.Redshift
{
    public partial class RedshiftVisitor : Postgres.Core.VisitorCore
    {
        private readonly string[] DbTypeStrings =
        {
            "BIGINT", //NpgsqlDbType.Bigint,
            "TEXT", //NpgsqlDbType.Bytea,
            "BOOLEAN", //NpgsqlDbType.Boolean,
            "CHAR", //NpgsqlDbType.Char,
            "TIMESTAMP", //NpgsqlDbType.Timestamp,
            "NUMERIC", //NpgsqlDbType.Numeric,
            "REAL", //NpgsqlDbType.Real,
            "TEXT", //NpgsqlDbType.Bytea,
            "INTEGER", //NpgsqlDbType.Integer,
            "MONEY", //NpgsqlDbType.Money,
            "CHAR", //NpgsqlDbType.Char,
            "TEXT", //NpgsqlDbType.Text,
            "TEXT", //NpgsqlDbType.Varchar,
            "DOUBLE PRECISION", //NpgsqlDbType.Double,
            "CHAR(36)", //NpgsqlDbType.Uuid,
            "TIMESTAMP", //NpgsqlDbType.Timestamp,
            "SMALLINT", //NpgsqlDbType.Smallint,
            "REAL", //NpgsqlDbType.Real,
            "TEXT", //NpgsqlDbType.Text,
            "TIMESTAMP", //NpgsqlDbType.Timestamp,
            "SMALLINT", //NpgsqlDbType.Smallint,
            "TEXT", //NpgsqlDbType.Bytea,
            "VARCHAR", //NpgsqlDbType.Varchar,
            "Unknown", //NpgsqlDbType.Unknown,
            "TEXT", //NpgsqlDbType.Xml,
            "DATE", //NpgsqlDbType.Date,
            "TIME", //NpgsqlDbType.Time,
            "TIMESTAMPTZ", //NpgsqlDbType.TimestampTZ
            "DATETIMEOFFSET",
            "TEXT", //NpgsqlDbType.Serial
        };

        protected override void VisitType(ITyped typedToken)
        {
            if (typedToken.DbType.HasValue)
            {
                this.State.Write(this.DbTypeStrings[(int) typedToken.DbType]);
            }
        }
    }
}
