using System;
using System.Collections.Generic;

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

        protected class Sym
        {
            public const string ALL = "ALL";
            public const string AND = "AND";
            public const string ANY = "ANY";
            public const string AUTOINCREMENT = "AUTOINCREMENT";
            public const string AS = "AS";
            public const string AS_osp = "AS [";
            public const string ASC = "ASC";
            public const string BETWEEN = "BETWEEN";
            public const string CLUSTERED = "CLUSTERED";
            public const string NONCLUSTERED = "NONCLUSTERED";

            public const string INCLUDE_op = "INCLUDE (";

            public const string DESC = "DESC";
            public const string DEFAULT_op = "DEFAULT (";
            public const string EXISTS = "EXISTS";
            public const string FROM = "FROM";
            public const string GROUP_BY = "GROUP BY";
            public const string HAVING = "HAVING";
            public const string IDENTITY = "IDENTITY";
            public const string IN_op = "IN (";
            public const string INTO = "INTO";
            public const string IS_NOT_NULL = "IS NOT NULL";
            public const string IS_NULL = "IS NULL";
            public const string LIKE = "LIKE";

            public const string MAXDOP = "MAXDOP";
            public const string NOT_IN_op = "NOT IN (";
            public const string NOT_op = "NOT (";
            public const string NOT_NULL = "NOT NULL";

            public const string INTERSECT = "INTERSECT";
            public const string EXCEPT = "EXCEPT";
            public const string UNION = "UNION";
            public const string UNION_ALL = "UNION ALL";


            public const string ABORT = "ABORT";
            public const string FAIL = "FAIL";
            public const string IGNORE = "IGNORE";
            public const string REPLACE = "REPLACE";
            public const string ROLLBACK = "ROLLBACK";

            public const string ON_CONFLICT = "ON CONFLICT";


            public const string ONLINE = "ONLINE";
            public const string ORDER_BY = "ORDER BY";

            public const string OUTPUT = "OUTPUT";
            public const string PERCENT = "PERCENT";
            public const string PRIMARY_KEY = "PRIMARY KEY";

            public const string TOP_op = "TOP (";
            public const string WHERE = "WHERE";
            public const string WITH = "WITH";
            public const string WITH_TIES = "WITH TIES";
            public const string ALTER_VIEW = "ALTER VIEW";
            public const string COMMA = ",";
            public const string cp = ")";

            public const string CONSTRAINT = "CONSTRAINT";
            public const string UNIQUE = "UNIQUE";

            public const string CREATE = "CREATE";
            public const string CREATE_TEMP_TABLE = "CREATE TEMPORARY TABLE";
            public const string CREATE_TABLE = "CREATE TABLE";
            public const string CREATE_VIEW = "CREATE VIEW";
            public const string CREATE_TEMPORARY_VIEW = "CREATE TEMPORARY VIEW";
            public const string csp = "]";
            public const string DELETE = "DELETE";
            public const string DROP_INDEX = "DROP INDEX";
            public const string DROP_VIEW = "DROP VIEW";
            public const string DROP_TABLE = "DROP TABLE";

            public const string IF_NOT_EXISTS = "IF NOT EXISTS";
            public const string IF_EXISTS = "IF EXISTS";

            public const string REINDEX = "REINDEX";
            public const string LIMIT = "LIMIT";

            public const string OFFSET = "OFFSET";
            public const string ROWS = "ROWS";
            public const string FETCH_NEXT = "FETCH NEXT";
            public const string ROWS_ONLY = "ROWS ONLY";

            public const string BEGIN_TRANSACTION = "BEGIN TRANSACTION";
            public const string BEGIN_IMMIDIATE_TRANSACTION = "BEGIN IMMIDIATE TRANSACTION";
            public const string BEGIN_DEFERRED_TRANSACTION = "BEGIN DEFERRED TRANSACTION";
            public const string BEGIN_EXCLUSIVE_TRANSACTION = "BEGIN EXCLUSIVE TRANSACTION";


            public const string INDEX = "INDEX";

            public const string NULL = "NULL";
            public const string OFF = "OFF";
            public const string ON = "ON";
            public const string op = "(";
            public const string osp = "[";
            public const string sc = ";";
            public const string SPACE = " ";
            public const string SELECT = "SELECT";
            public const string DISTINCT = "DISTINCT";

            public const string SAVEPOINT_ = "SAVEPOINT";

            public const string MAX = "MAX";
            public const string SET = "SET";
            public const string MERGE = "MERGE";
            public const string USING = "USING";
            public const string WHEN_MATCHED = "WHEN MATCHED";
            public const string WHEN_NOT_MATCHED_BY_TARGET = "WHEN NOT MATCHED BY TARGET";
            public const string WHEN_NOT_MATCHED_BY_SOURCE = "WHEN NOT MATCHED BY SOURCE";
            public const string THEN = "THEN";
            public const string UPDATE_SET = "UPDATE SET";
            public const string INSERT = "INSERT";
            public const string VALUES_op = "VALUES (";
            public const string DEFAULT_VALUES = "DEFAULT VALUES";
            public const string UPDATE = "UPDATE";
            public const string VALUES = "VALUES";
            public const string WITH_MARK = "WITH MARK";
            public const string COMMIT_TRANSACTION = "COMMIT TRANSACTION";
            public const string ROLLBACK_TRANSACTION = "ROLLBACK TRANSACTION";
            public const string SAVE_TRANSACTION = "SAVE TRANSACTION";
            public const string DECLARE = "DECLARE";
            public const string BREAK = "BREAK";
            public const string CONTINUE = "CONTINUE";
            public const string GOTO = "GOTO";
            public const string RETURN = "RETURN";
            public const string THROW = "THROW";
            public const string BEGIN_TRY = "BEGIN TRY";
            public const string END_TRY = "END TRY";
            public const string BEGIN_CATCH = "BEGIN CATCH";
            public const string END_CATCH = "END CATCH";
            public const string WAITFOR_DELAY = "WAITFOR DELAY";
            public const string WAITFOR_TIME = "WAITFOR TIME";
            public const string WHILE = "WHILE";
            public const string BEGIN = "BEGIN";
            public const string END = "END";
            public const string IF = "IF";
            public const string ELSE = "ELSE";
            public const string TABLE = "TABLE";
            public const string FILETABLE = "FILETABLE";
            public const string SPARSE = "SPARSE";
            public const string ROWGUIDCOL = "ROWGUIDCOL";
            public const string DEFAULT = "DEFAULT";
            public const string EXEC = "EXEC";
            public const string PAD_INDEX = "PAD_INDEX";
            public const string FILLFACTOR = "FILLFACTOR";
            public const string SORT_IN_TEMPDB = "SORT_IN_TEMPDB";
            public const string DROP_EXISTING = "DROP_EXISTING";
            public const string ALLOW_ROW_LOCKS = "ALLOW_ROW_LOCKS";
            public const string ALLOW_PAGE_LOCKS = "ALLOW_PAGE_LOCKS";
            public const string ALTER_INDEX = "ALTER INDEX";
            public const string REBUILD = "REBUILD";
            public const string IGNORE_DUP_KEY = "IGNORE_DUP_KEY";
            public const string STATISTICS_NORECOMPUTE = " STATISTICS_NORECOMPUTE";
            public const string DISABLE = "DISABLE";
            public const string REORGANIZE = "REORGANIZE";


            public const string asterisk = "*";

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
        }
    }
}