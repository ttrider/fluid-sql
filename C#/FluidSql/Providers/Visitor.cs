using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers
{
    public abstract class Visitor
    {
        protected abstract string OpenQuote { get; }
        protected abstract string CloseQuote { get; }
        protected abstract void VisitStatement(IStatement token, VisitorState state);
        protected abstract void VisitToken(Token token, VisitorState state, bool includeAlias = false);
        protected abstract void VisitJoinType(Joins join, VisitorState state);

        protected virtual string ResolveName(Name name)
        {
            return name.GetFullName(this.OpenQuote, this.CloseQuote);
        }

        protected virtual void VisitConflict(OnConflict? conflict, VisitorState state)
        {
            if (conflict.HasValue)
            {
                state.Write(Sym.ON_CONFLICT);
                switch (conflict.Value)
                {
                    case OnConflict.Abort:
                        state.Write(Sym.ABORT);
                        break;
                    case OnConflict.Fail:
                        state.Write(Sym.FAIL);
                        break;
                    case OnConflict.Ignore:
                        state.Write(Sym.IGNORE);
                        break;
                    case OnConflict.Replace:
                        state.Write(Sym.REPLACE);
                        break;
                    case OnConflict.Rollback:
                        state.Write(Sym.ROLLBACK);
                        break;
                }

            }

        }

        protected virtual bool VisitTransactionName(TransactionStatement statement, VisitorState state)
        {
            if (statement.Name != null)
            {
                state.Write(ResolveName(statement.Name));
                return true;
            }
            if (statement.Parameter != null)
            {
                state.Write(statement.Parameter.Name);
                state.Parameters.Add(statement.Parameter);
                return true;
            }
            return false;
        }

        protected virtual void VisitStatementsStatement(StatementsStatement statement, VisitorState state)
        {
            bool first = true;
            foreach (var subStatement in statement.Statements)
            {
                state.WriteCRLF(first);
                first = false;
                VisitStatement(subStatement, state);
                state.WriteStatementTerminator();
            }
        }


        protected virtual void VisitTokenSet(IEnumerable<Token> tokens, VisitorState state, string prefix, string separator, string suffix, bool includeAlias = false)
        {
            var enumerator = tokens.GetEnumerator();
            if (enumerator.MoveNext())
            {
                state.Write(prefix);
                VisitToken(enumerator.Current, state, includeAlias);

                while (enumerator.MoveNext())
                {
                    state.Write(separator);
                    VisitToken(enumerator.Current, state, includeAlias);
                }
                state.Write(suffix);
            }
        }

        protected virtual void VisitScalarToken(Scalar token, VisitorState state, string openQuote, string closeQuote)
        {
            var value = token.Value;
            if (value == null) return;

            if (value is DBNull)
            {
                state.Write(Sym.NULL);
            }
            else if ((value is Boolean)
                || (value is SByte)
                || (value is Byte)
                || (value is Int16)
                || (value is UInt16)
                || (value is Int32)
                || (value is UInt32)
                || (value is Int64)
                || (value is UInt64)
                || (value is Single)
                || (value is Double)
                || (value is Decimal))
            {
                state.Write(value.ToString());
            }
            else if (value is TimeSpan)
            {
                state.Write(openQuote);
                state.Write(((TimeSpan)value).ToString("HH:mm:ss"));
                state.Write(closeQuote);
            }
            else if (value is DateTime)
            {
                state.Write(openQuote);
                state.Write(((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss"));
                state.Write(closeQuote);
            }
            else if (value is DateTimeOffset)
            {
                state.Write(openQuote);
                state.Write(((DateTimeOffset)value).ToString("yyyy-MM-ddTHH:mm:ss"));
                state.Write(closeQuote);
            }
            else
            {
                state.Write(openQuote + value + closeQuote);
            }
        }

        protected virtual void VisitStatementToken(IStatement token, VisitorState state)
        {
            state.Write(Sym.op);
            VisitStatement(token, state);
            state.Write(Sym.cp);
        }

        protected virtual void VisitWhere(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Write(Sym.WHERE);
                VisitToken(whereToken, state);
            }
        }
        protected virtual void VisitNameToken(Name token, VisitorState state)
        {
            state.Write(ResolveName(token));
        }

        protected virtual void VisitParameterToken(Parameter token, VisitorState state)
        {
            state.Write(token.Name);
        }

        protected virtual void VisitSnippetToken(Snippet token, VisitorState state)
        {
            state.Write(token.Value);
        }

        protected virtual void VisitFunctionToken(Function token, VisitorState state)
        {
            state.Write(token.Name, Sym.op);
            VisitTokenSet(token.Arguments, state, null, Sym.COMMA, null);
            state.Write(Sym.cp);
        }

        protected virtual void VisitBinaryToken(BinaryToken token, VisitorState state, string operation)
        {
            VisitToken(token.First, state, false);
            state.Write(operation);
            VisitToken(token.Second, state, false);
        }

        protected virtual void VisitIsEqualsToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.EqualsVal);
        }

        protected virtual void VisitNotEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.NotEqualVal);
        }

        protected virtual void VisitLessToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.LessVal);
        }

        protected virtual void VisitNotLessToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.NotLessVal);
        }

        protected virtual void VisitLessOrEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.LessOrEqualVal);
        }

        protected virtual void VisitGreaterToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.GreaterVal);
        }

        protected virtual void VisitNotGreaterToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.NotGreaterVal);
        }

        protected virtual void VisitGreaterOrEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.GreaterOrEqualVal);
        }

        protected virtual void VisitAllToken(AllToken token, VisitorState state)
        {
            state.Write(Sym.ALL);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitAnyToken(AnyToken token, VisitorState state)
        {
            state.Write(Sym.ANY);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitAndToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.AndVal);
        }

        protected virtual void VisitOrToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.OrVal);
        }

        protected virtual void VisitPlusToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.PlusEqVal : Sym.PlusVal);
        }

        protected virtual void VisitMinusToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.MinusEqVal : Sym.MinusVal);
        }

        protected virtual void VisitDivideToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.DivideEqVal : Sym.DivideVal);
        }

        protected virtual void VisitModuloToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.ModuloEqVal : Sym.ModuloVal);
        }

        protected virtual void VisitMultiplyToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.MultiplyEqVal : Sym.MultiplyVal);
        }

        protected virtual void VisitBitwiseAndToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.BitwiseAndEqVal : Sym.BitwiseAndVal);
        }

        protected virtual void VisitBitwiseOrToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.BitwiseOrEqVal : Sym.BitwiseOrVal);
        }

        protected virtual void VisitBitwiseXorToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.BitwiseXorEqVal : Sym.BitwiseXorVal);
        }

        protected virtual void VisitBitwiseNotToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? Sym.BitwiseNotEqVal : Sym.BitwiseNotVal);
        }

        protected virtual void VisitAssignToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, Sym.AssignVal);
        }

        protected virtual void VisitLikeToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Write(Sym.LIKE);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitContainsToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state, false);
            state.Write("LIKE '%' +");
            VisitToken(token.Second, state, false);
            state.Write("+ '%'");
        }

        protected virtual void VisitStartsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state, false);
            state.Write(Sym.LIKE);
            VisitToken(token.Second, state, false);
            state.Write("+ '%'");
        }

        protected virtual void VisitEndsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state, false);
            state.Write("LIKE '%' +");
            VisitToken(token.Second, state, false);
        }

        protected virtual void VisitGroupToken(GroupToken token, VisitorState state)
        {
            state.Write(Sym.op);
            VisitToken(token.Token, state, false);
            state.Write(Sym.cp);
        }

        protected virtual void VisitExistsToken(ExistsToken token, VisitorState state)
        {
            state.Write(Sym.EXISTS);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitNotToken(NotToken token, VisitorState state)
        {
            state.Write(Sym.NOT_op);
            VisitToken(token.Token, state);
            state.Write(Sym.cp);
        }

        protected virtual void VisitIsNullToken(IsNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Sym.IS_NULL);
        }

        protected virtual void VisitIsNotNullToken(IsNotNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state, false);
            state.Write(Sym.IS_NOT_NULL);
        }

        protected virtual void VisitBetweenToken(BetweenToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Write(Sym.BETWEEN);
            VisitToken(token.First, state);
            state.Write(Sym.AND);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitInToken(InToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            VisitTokenSet(token.Set, state, Sym.IN_op, Sym.COMMA, Sym.cp);
        }

        protected virtual void VisitNotInToken(NotInToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            VisitTokenSet(token.Set, state, Sym.NOT_IN_op, Sym.COMMA, Sym.cp);
        }
        protected virtual void VisitCommentToken(CommentToken token, VisitorState state)
        {
            state.Write("/*");

            VisitToken(token.Content, state);

            state.Write("*/");
        }






        protected virtual void VisitCommentStatement(CommentStatement statement, VisitorState state)
        {
            state.Write("/*");
            VisitStatement(statement.Content, state);
            state.Write("*/");
        }
        protected virtual void VisitSnippetStatement(SnippetStatement statement, VisitorState state)
        {
            state.Write(statement.Value);
        }
        protected virtual void VisitUnion(Union statement, VisitorState state)
        {
            VisitStatement(statement.First, state);

            state.Write((statement.All ? Sym.UNION_ALL: Sym.UNION));

            VisitStatement(statement.Second, state);
        }
        protected virtual void VisitExcept(Except statement, VisitorState state)
        {
            VisitStatement(statement.First, state);

            state.Write(Sym.EXCEPT);

            VisitStatement(statement.Second, state);
        }
        protected virtual void VisitIntersect(Intersect statement, VisitorState state)
        {
            VisitStatement(statement.First, state);
            state.Write(Sym.INTERSECT);
            VisitStatement(statement.Second, state);
        }
        protected virtual void VisitFrom(IEnumerable<Token> recordsets, VisitorState state)
        {
            VisitTokenSet(recordsets, state, Sym.FROM, Sym.COMMA, null, true);
        }

        protected virtual void VisitFrom(Token recordset, VisitorState state)
        {
            if (recordset != null)
            {
                state.Write(Sym.FROM);
                VisitToken(recordset, state, true);
            }
        }

        protected virtual void VisitJoin(ICollection<Join> list, VisitorState state)
        {
            if (list.Count > 0)
            {
                foreach (var join in list)
                {
                    VisitJoinType(join.Type, state);
                    VisitToken(@join.Source, state, true);

                    if (join.On != null)
                    {
                        state.Write(Sym.ON);
                        VisitToken(@join.On, state);
                    }
                }
            }
        }

        protected virtual void VisitGroupBy(ICollection<Name> groupBy, VisitorState state)
        {
            VisitTokenSet(groupBy, state, Sym.GROUP_BY, Sym.COMMA, null);
        }

        protected virtual void VisitHaving(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Write(Sym.HAVING);
                VisitToken(whereToken, state);
            }
        }

        protected virtual void VisitOrderToken(Order orderToken, VisitorState state)
        {
            VisitNameToken(orderToken.Column, state);
            state.Write(orderToken.Direction == Direction.Asc ? Sym.ASC : Sym.DESC);
        }


        protected virtual void VisitOrderBy(ICollection<Order> orderBy, VisitorState state)
        {
            VisitTokenSet(orderBy, state, Sym.ORDER_BY, Sym.COMMA, null);
        }



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
            
            public const string OUTPUT_ = "OUTPUT ";
            public const string PERCENT = "PERCENT";
            public const string PRIMARY_KEY = "PRIMARY KEY";

            public const string TOP_op = "TOP (";
            public const string WHERE = "WHERE";
            public const string WITH = "WITH";
            public const string WITH_TIES = "WITH TIES";
            public const string ALTER_VIEW = "ALTER VIEW";
            public const string COMMA = ",";
            public const string cp = ")";
            public const string cpsc = ");";

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
