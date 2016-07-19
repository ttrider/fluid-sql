// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>
using System;
using System.Globalization;
// ReSharper disable InconsistentNaming


namespace TTRider.FluidSql.Providers.MySql
{
    internal partial class MySqlVisitor : Visitor
    {
        private static readonly string[] supportedDialects = new[] { "mysql", "ansi" };

        private readonly string[] DbTypeStrings =
        {
            "BIGINT", // BigInt = 0,
            "BINARY", // Binary = 1,
            "BIT", // Bit = 2,
            "CHAR", // Char = 3,
            "DATETIME", // DateTime = 4,
            "DECIMAL", // Decimal = 5,
            "FLOAT", // Float = 6,
            "LONGBLOB", // Image = 7,
            "INTEGER", // Int = 8,
            "DECIMAL", // Money = 9,
            "CHAR", // NChar = 10,
            "TEXT", // NText = 11,
            "VARCHAR", // NVarChar = 12,
            "REAL", // Real = 13,
            "CHAR ( 38 )", // UniqueIdentifier = 14,
            "DATETIME", // SmallDateTime = 15,
            "SMALLINT", // SmallInt = 16,
            "DECIMAL", // SmallMoney = 17,
            "TEXT", // Text = 18,
            "TIMESTAMP", // Timestamp = 19,
            "TINYINT", // TinyInt = 20,
            "VARBINARY", // VarBinary = 21,
            "VARCHAR", // VarChar = 22,
            "BLOB", // Variant = 23,
            "LONGTEXT", // Xml = 24,
            "DATE", // Date = 25,
            "TIME", // Time = 26,
            "DATETIME", // DateTime2 = 27,
            "DATETIME" // DateTimeOffset = 28,
        };

        protected override string[] SupportedDialects { get { return supportedDialects; } }

        public MySqlVisitor()
        {
            this.IdentifierOpenQuote = "`";
            this.IdentifierCloseQuote = "`";
            this.LiteralOpenQuote = "N'";
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
            if (typedToken.Length.HasValue)
            {
                State.Write(Symbols.OpenParenthesis);
                State.Write(typedToken.Length.Value == -1
                        ? "65535"                                                           //max lengh for mySql
                        : typedToken.Length.Value.ToString(CultureInfo.InvariantCulture));
                
                State.Write(Symbols.CloseParenthesis);
            }
        }


        protected class MySqlSymbols : Symbols
        {
            public const string AUTO_INCREMENT = "AUTO_INCREMENT";
            public const string UTC_TIMESTAMP = "UTC_TIMESTAMP";
            public const string UUID = "UUID";

            public const string YEAR = "YEAR";
            public const string MONTH = "MONTH";
            public const string DAY = "DAY";
            public const string WEEK = "WEEK";
            public const string HOUR = "HOUR";
            public const string MINUTE = "MINUTE";
            public const string SECOND = "SECOND";
            public const string MICROSECOND = "MICROSECOND";

            public const string TIMESTAMPDIFF = "TIMESTAMPDIFF";
            public const string MAKEDATE = "MAKEDATE";
            public const string MAKETIME = "MAKETIME";
            public const string TIMESTAMP = "TIMESTAMP";

            public const string DATE_ADD = "DATE_ADD";
            public const string DATE_SUB = "DATE_SUB";

            public const string milisecondInSecond = "1000";

            public const string ITERATE = "ITERATE";
            public const string LEAVE = "LEAVE";

            public const string DELIMITER = "DELIMITER";
        }

    }
}
