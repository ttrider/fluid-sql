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


namespace TTRider.FluidSql.Providers.PostgreBased
{
    public partial class PostgreBasedSQLVisitor : Visitor
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



        protected  override string[] SupportedDialects { get { return supportedDialects; } }

        public PostgreBasedSQLVisitor()
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

        protected override void VisitValue(object value)
        {
            if (value is bool)
            {
                State.Write((bool)value ? "true" : "false");
            }
            else
            {
                base.VisitValue(value);
            }
        }

        protected class PostgrSQLSymbols : Symbols
        {
            public const string DATEPART = "DATE_PART";
            public const string TIMESTAMP = "TIMESTAMP";

            public const string BEGIN_LABEL = "<<";
            public const string END_LABEL = ">>";

            public const string DELAY_FORMAT = "pg_sleep({0})";

            //public const string DATEADD = "DATEADD";
            //public const string DATEDIFF = "DATEDIFF";

            //public const string DATETIMEFROMPARTS = "DATETIMEFROMPARTS";
            //public const string GETDATE = "GETDATE";
            //public const string GETUTCDATE = "GETUTCDATE";
            //public const string IDENTITY_INSERT = "IDENTITY_INSERT";
            //public const string NEWID = "NEWID";
            //public const string TIMEFROMPARTS = "TIMEFROMPARTS";
            public const string other = "other";
            public const string AssignValSign = ":=";

            public const string d = "day";
            public const string hh = "hour";
            public const string m = "month";
            public const string mi = "minute";
            public const string ms = "microsecond";
            //public const string s = "s";
            public const string ss = "second";
            public const string ww = "week";
            public const string yy = "year";

            public const string monthsInYear = "12";
            public const string daysInWeek = "7";
            public const string hoursInDay = "24";
            public const string minutesInHour = "60";
            public const string secondInMinute = "60";
            public const string milisecondInSecond = "1000";
        }
    }
}
