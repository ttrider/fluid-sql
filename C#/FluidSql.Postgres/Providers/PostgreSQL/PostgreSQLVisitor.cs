// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>
using System;
using System.Collections.Generic;
using NpgsqlTypes;

// ReSharper disable InconsistentNaming


namespace TTRider.FluidSql.Providers.PostgreSQL
{
    internal partial class PostgreSQLVisitor : Visitor
    {
        private static readonly string[] supportedDialects = new[] { "npgsql", "postgresql", "postgres", "ansi" };


        private readonly string[] DbTypeStrings =
        {
            "BIGINT",       //NpgsqlDbType.Bigint,
            "BYTEA",        //NpgsqlDbType.Bytea,
            "BOOLEAN",      //NpgsqlDbType.Boolean,
            "CHAR",         //NpgsqlDbType.Char,
            "TIMESTAMP",    //NpgsqlDbType.Timestamp,
            "NUMERIC",      //NpgsqlDbType.Numeric,
            "REAL",         //NpgsqlDbType.Real,
            "BYTEA",        //NpgsqlDbType.Bytea,
            "INTEGER",      //NpgsqlDbType.Integer,
            "MONEY",        //NpgsqlDbType.Money,
            "CHAR",         //NpgsqlDbType.Char,
            "TEXT",         //NpgsqlDbType.Text,
            "TEXT",      //NpgsqlDbType.Varchar,
            "DOUBLE PRECISION",//NpgsqlDbType.Double,
            "UUID",         //NpgsqlDbType.Uuid,
            "TIMESTAMP",    //NpgsqlDbType.Timestamp,
            "SMALLINT",     //NpgsqlDbType.Smallint,
            "REAL",         //NpgsqlDbType.Real,
            "TEXT",         //NpgsqlDbType.Text,
            "TIMESTAMP",    //NpgsqlDbType.Timestamp,
            "SMALLINT",     //NpgsqlDbType.Smallint,
            "BYTEA",        //NpgsqlDbType.Bytea,
            "VARCHAR",      //NpgsqlDbType.Varchar,
            "Unknown",      //NpgsqlDbType.Unknown,
            "XM,",          //NpgsqlDbType.Xml,
            "DATE",         //NpgsqlDbType.Date,
            "TIME",         //NpgsqlDbType.Time,
            "TIMESTAMPTZ",  //NpgsqlDbType.TimestampTZ
            "DATETIMEOFFSET", 
            "SERIAL",       //NpgsqlDbType.Serial
        };

        protected override string[] SupportedDialects { get { return supportedDialects; } }

        public PostgreSQLVisitor()
        {
            this.IdentifierOpenQuote = "\"";
            this.IdentifierCloseQuote = "\"";
            this.LiteralOpenQuote = "'";
            this.LiteralCloseQuote = "'";
            this.CommentOpenQuote = "/*";
            this.CommentCloseQuote = "*/";
        }

        protected override void VisitJoinType(Joins join)
        {
            State.Write(JoinStrings[(int)join]);
        }

        protected override void VisitType(ITyped typedToken)
        {
            if (typedToken.DbType.HasValue)
            {
                State.Write(DbTypeStrings[(int)typedToken.DbType]);
            }
        }

        protected class PostgrSQLSymbols : Symbols
        {
            //public const string AUTO_INCREMENT = "AUTO_INCREMENT";
        }
    }
}
