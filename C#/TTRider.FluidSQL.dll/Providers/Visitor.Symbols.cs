// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {
        protected readonly string[] JoinStrings =
        {
            "INNER JOIN", //Inner = 0,
            "LEFT OUTER JOIN", //LeftOuter = 1,
            "RIGHT OUTER JOIN", //RightOuter = 2,
            "FULL OUTER JOIN", //FullOuter = 3,
            "CROSS JOIN" //Cross = 4,
        };

        protected class Symbols
        {
            public const string ABORT = "ABORT";
            public const string ALL = "ALL";
            public const string ALLOW_PAGE_LOCKS = "ALLOW_PAGE_LOCKS";
            public const string ALLOW_ROW_LOCKS = "ALLOW_ROW_LOCKS";
            public const string ALTER = "ALTER";
            public const string AND = "AND";
            public const string ANY = "ANY";
            public const string ARRAY = "ARRAY";
            public const string AS = "AS";
            public const string ASC = "ASC";
            public const string AT = "AT";
            public const string AUTHORIZATION = "AUTHORIZATION";
            public const string AUTOINCREMENT = "AUTOINCREMENT";
            public const string BEGIN = "BEGIN";
            public const string BEGIN_TRY = "BEGIN TRY";
            public const string BETWEEN = "BETWEEN";
            public const string BREAK = "BREAK";
            public const string BY = "BY";
            public const string CALL = "CALL";
            public const string CASCADE = "CASCADE";
            public const string CASE = "CASE";
            public const string CAST = "CAST";
            public const string CATCH = "CATCH";
            public const string CLUSTERED = "CLUSTERED";
            public const string COALESCE = "COALESCE";
            public const string COMMIT = "COMMIT";
            public const string COMMITED = "COMMITED";
            public const string CONFLICT = "CONFLICT";
            public const string CONSTRAINT = "CONSTRAINT";
            public const string CONTINUE = "CONTINUE";
            public const string CREATE = "CREATE";
            public const string CURRENT = "CURRENT";
            public const string DATETIME = "DATETIME";
            public const string DEALLOCATE = "DEALLOCATE";
            public const string DECLARE = "DECLARE";
            public const string DEFAULT = "DEFAULT";
            public const string DEFERRED = "DEFERRED";
            public const string DELAY = "DELAY";
            public const string DELETE = "DELETE";
            public const string DESC = "DESC";
            public const string DISABLE = "DISABLE";
            public const string DISTINCT = "DISTINCT";
            public const string DIV = "DIV";
            public const string DO = "DO";
            public const string DROP = "DROP";
            public const string DROP_EXISTING = "DROP_EXISTING";
            public const string ELSE = "ELSE";
            public const string END = "END";
            public const string EXCEPT = "EXCEPT";
            public const string EXCEPTION = "EXCEPTION";
            public const string EXCLUSIVE = "EXCLUSIVE";
            public const string EXEC = "EXEC";
            public const string EXECUTE = "EXECUTE";
            public const string EXISTS = "EXISTS";
            public const string EXIT = "EXIT";
            public const string FAIL = "FAIL";
            public const string FETCH = "FETCH";
            public const string FILETABLE = "FILETABLE";
            public const string FILLFACTOR = "FILLFACTOR";
            public const string FROM = "FROM";
            public const string FUNCTION = "FUNCTION";
            public const string GO = "GO";
            public const string GOTO = "GOTO";
            public const string GROUP_BY = "GROUP BY";
            public const string HAVING = "HAVING";
            public const string IDENTITY = "IDENTITY";
            public const string IF = "IF";
            public const string IGNORE = "IGNORE";
            public const string IGNORE_DUP_KEY = "IGNORE_DUP_KEY";
            public const string IIF = "IIF";
            public const string IMMIDIATE = "IMMIDIATE";
            public const string IN = "IN";
            public const string INCLUDE = "INCLUDE";
            public const string INDEX = "INDEX";
            public const string INSERT = "INSERT";
            public const string INTERSECT = "INTERSECT";
            public const string INTERVAL = "INTERVAL";
            public const string INTO = "INTO";
            public const string INOUT = "INOUT";
            public const string IS = "IS";
            public const string ISOLATION = "ISOLATION";
            public const string KEY = "KEY";
            public const string LEVEL = "LEVEL";
            public const string LIKE = "LIKE";
            public const string LIMIT = "LIMIT";
            public const string LOOP = "LOOP";
            public const string MARK = "MARK";
            public const string MATCHED = "MATCHED";
            public const string MAX = "MAX";
            public const string MAXDOP = "MAXDOP";
            public const string MERGE = "MERGE";
            public const string NEXT = "NEXT";
            public const string NONCLUSTERED = "NONCLUSTERED";
            public const string NOT = "NOT";
            public const string NOTHING = "NOTHING";
            public const string NOW = "NOW";
            public const string NULL = "NULL";
            public const string NULLIF = "NULLIF";
            public const string OBJECT_ID = "OBJECT_ID";
            public const string OF = "OF";
            public const string OFF = "OFF";
            public const string OFFSET = "OFFSET";
            public const string ON = "ON";
            public const string ONLINE = "ONLINE";
            public const string ONLY = "ONLY";
            public const string OR = "OR";
            public const string ORDER_BY = "ORDER BY";
            public const string OUT = "OUT";
            public const string OUTPUT = "OUTPUT";
            public const string OWNER = "OWNER";
            public const string PAD_INDEX = "PAD_INDEX";
            public const string PERCENT = "PERCENT";
            public const string PERFORM = "PERFORM";
            public const string PREPARE = "PREPARE";
            public const string PRIMARY = "PRIMARY";
            public const string PROCEDURE = "PROCEDURE";
			public const string RAISE = "RAISE";
            public const string READONLY = "READONLY";
            public const string READ = "READ";
            public const string REBUILD = "REBUILD";
            public const string RECOMPILE = "RECOMPILE";
            public const string RECURSIVE = "RECURSIVE";
            public const string REINDEX = "REINDEX";
            public const string RELEASE = "RELEASE";
            public const string RENAME = "RENAME";
            public const string REORGANIZE = "REORGANIZE";
            public const string REPEATABLE = "REPEATABLE";
            public const string REPLACE = "REPLACE";
            public const string RESTRICT = "RESTRICT";
            public const string RETURN = "RETURN";
            public const string RETURNS = "RETURNS";
            public const string RETURNING = "RETURNING";
            public const string ROLLBACK = "ROLLBACK";
            public const string ROWGUIDCOL = "ROWGUIDCOL";
            public const string ROWS = "ROWS";
            public const string SAVE = "SAVE";
            public const string SAVEPOINT = "SAVEPOINT";
            public const string SCHEMA = "SCHEMA";
            public const string SELECT = "SELECT";
            public const string SERIALIZABLE = "SERIALIZABLE";
            public const string SET = "SET";
            public const string SORT_IN_TEMPDB = "SORT_IN_TEMPDB";
            public const string SOURCE = "SOURCE";
            public const string SPARSE = "SPARSE";
            public const string STATISTICS_NORECOMPUTE = "STATISTICS_NORECOMPUTE";
            public const string START = "START";
            public const string TABLE = "TABLE";
            public const string TARGET = "TARGET";
            public const string TEMPORARY = "TEMPORARY";
            public const string THEN = "THEN";
            public const string THROW = "THROW";
            public const string TIES = "TIES";
            public const string TIME = "TIME";
            public const string TO = "TO";
            public const string TOP = "TOP";
            public const string TRANSACTION = "TRANSACTION";
            public const string TRUNC = "TRUNC";
            public const string TRY = "TRY";
            public const string UNCOMMITED = "UNCOMMITED";
            public const string UNION = "UNION";
            public const string UNIQUE = "UNIQUE";
            public const string UPDATE = "UPDATE";
            public const string USING = "USING";
            public const string VALUES = "VALUES";
            public const string VIEW = "VIEW";
            public const string VOID = "VOID";
            public const string WAITFOR = "WAITFOR";
            public const string WHEN = "WHEN";
            public const string WHERE = "WHERE";
            public const string WHILE = "WHILE";
            public const string WITH = "WITH";
            public const string WRITE = "WRITE";
            public const string UTC = "UTC";
            public const string ZONE = "ZONE";
            public const string Comma = ",";
            public const string CloseParenthesis = ")";
            public const string OpenParenthesis = "(";
            public const string CloseBracket = "]";
            public const string OpenBracket = "[";
            public const string Semicolon = ";";
            public const string Colon = ":";
            public const string Asterisk = "*";
            public const string Pound = "#";
            public const string At = "@";
            public const string Empty = "";
            public const string Space = " ";

            public const string EqualsVal = "=";
            public const string AssignVal = "=";
            public const string NotEqualVal = "<>";
            public const string LessVal = "<";
            public const string NotLessVal = "!<";
            public const string LessOrEqualVal = "<=";
            public const string GreaterVal = ">";
            public const string NotGreaterVal = "!>";
            public const string GreaterOrEqualVal = ">=";
            public const string AndVal = "AND";
            public const string OrVal = "OR";
            public const string PlusVal = "+";
            public const string MinusVal = "-";
            public const string DivideVal = "/";
            public const string ModuloVal = "%";
            public const string MultiplyVal = "*";
            public const string BitwiseAndVal = "&";
            public const string BitwiseOrVal = "|";
            public const string BitwiseXorVal = "^";
            public const string BitwiseNotVal = "~";
            public const string PlusEqVal = "+=";
            public const string MinusEqVal = "-=";
            public const string DivideEqVal = "/=";
            public const string ModuloEqVal = "%=";
            public const string MultiplyEqVal = "*=";
            public const string BitwiseAndEqVal = "&=";
            public const string BitwiseOrEqVal = "|=";
            public const string BitwiseXorEqVal = "^=";
            public const string BitwiseNotEqVal = "~=";
            public const string DotVal = ".";

            public const string SingleQuote = "'";
            public const string DoubleQuote = "\"";

            //for postgresql
            public const string TechId = "ctid";
            public const string CurrentSetting = "current_setting";
            public const string TimeZone = "TimeZone";

        }
    }
}