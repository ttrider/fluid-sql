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

        protected virtual void EnsureSemicolumn(VisitorState state)
        {
            // we need to make sure that the last non-whitespace character
            // is ';' unless it is */ or :
            //TODO: need a better code here
            for (var i = state.Buffer.Length - 1; i >= 0; i--)
            {
                var ch = state.Buffer[i];
                if (!Char.IsWhiteSpace(ch))
                {
                    if (ch == ';')
                    {
                        break;
                    }
                    if (ch == ':')
                    {
                        break;
                    }
                    if (ch == '/')
                    {
                        if (i > 0 && state.Buffer[i - 1] == '*')
                        {
                            break;
                        }
                    }

                    state.Append(";");
                    break;
                }
            }
        }

        protected virtual string ResolveName(Name name)
        {
            return name.GetFullName(this.OpenQuote, this.CloseQuote);
        }

        protected virtual void VisitConflict(OnConflict? conflict, VisitorState state)
        {
            if (conflict.HasValue)
            {
                state.Append(Sym._ON_CONFLICT);
                switch (conflict.Value)
                {
                    case OnConflict.Abort:
                        state.Append(Sym._ABORT);
                        break;
                    case OnConflict.Fail:
                        state.Append(Sym._FAIL);
                        break;
                    case OnConflict.Ignore:
                        state.Append(Sym._IGNORE);
                        break;
                    case OnConflict.Replace:
                        state.Append(Sym._REPLACE);
                        break;
                    case OnConflict.Rollback:
                        state.Append(Sym._ROLLBACK);
                        break;
                }

            }

        }



        protected virtual void VisitTokenSet(IEnumerable<Token> tokens, VisitorState state, string prefix, string separator, string suffix, bool includeAlias = false)
        {
            var enumerator = tokens.GetEnumerator();
            if (enumerator.MoveNext())
            {
                state.Append(prefix);
                VisitToken(enumerator.Current, state, includeAlias);

                while (enumerator.MoveNext())
                {
                    state.Append(separator);
                    VisitToken(enumerator.Current, state, includeAlias);
                }
                state.Append(suffix);
            }
        }

        protected virtual void VisitAlias(Token token, VisitorState state, string prefix = Sym._AS_)
        {
            if (!string.IsNullOrWhiteSpace(token.Alias))
            {
                state.Append(prefix);
                state.Append(this.OpenQuote);
                state.Append(token.Alias);
                state.Append(this.CloseQuote);
            }
        }
        protected virtual void VisitScalarToken(Scalar token, VisitorState state, string openQuote, string closeQuote)
        {
            var value = token.Value;
            if (value == null) return;

            if (value is DBNull)
            {
                state.Append(Sym.NULL);
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
                state.Append(value);
            }
            else if (value is TimeSpan)
            {
                state.Append(openQuote);
                state.Append(((TimeSpan)value).ToString("HH:mm:ss"));
                state.Append(closeQuote);
            }
            else if (value is DateTime)
            {
                state.Append(openQuote);
                state.Append(((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss"));
                state.Append(closeQuote);
            }
            else if (value is DateTimeOffset)
            {
                state.Append(openQuote);
                state.Append(((DateTimeOffset)value).ToString("yyyy-MM-ddTHH:mm:ss"));
                state.Append(closeQuote);
            }
            else
            {
                state.Append(openQuote + value + closeQuote);
            }
        }

        protected virtual void VisitStatementToken(IStatement token, VisitorState state)
        {
            state.Append(Sym.op);
            VisitStatement(token, state);
            state.Append(Sym.cp);
        }

        protected virtual void VisitWhere(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Append(Sym._WHERE_);
                VisitToken(whereToken, state);
            }
        }
        protected virtual void VisitNameToken(Name token, VisitorState state)
        {
            state.Append(ResolveName(token));
        }

        protected virtual void VisitParameterToken(Parameter token, VisitorState state)
        {
            state.Append(token.Name);
        }

        protected virtual void VisitSnippetToken(Snippet token, VisitorState state)
        {
            state.Append(token.Value);
        }

        protected virtual void VisitFunctionToken(Function token, VisitorState state)
        {
            state.Append(Sym.SPACE);
            state.Append(token.Name);

            VisitTokenSet(token.Arguments, state, Sym.op, Sym.COMMA_, Sym.cp);
        }

        protected virtual void VisitBinaryToken(BinaryToken token, VisitorState state, string operation)
        {
            VisitToken(token.First, state, false);
            state.Append(operation);
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
            state.Append(Sym._ALL_);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitAnyToken(AnyToken token, VisitorState state)
        {
            state.Append(Sym._ANY_);
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
            state.Append(Sym._LIKE_);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitContainsToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state, false);
            state.Append(" LIKE '%' + ");
            VisitToken(token.Second, state, false);
            state.Append(" + '%'");
        }

        protected virtual void VisitStartsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state, false);
            state.Append(Sym._LIKE_);
            VisitToken(token.Second, state, false);
            state.Append(" + '%'");
        }

        protected virtual void VisitEndsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state, false);
            state.Append(" LIKE '%' + ");
            VisitToken(token.Second, state, false);
        }

        protected virtual void VisitGroupToken(GroupToken token, VisitorState state)
        {
            state.Append(Sym._op);
            VisitToken(token.Token, state, false);
            state.Append(Sym._cp);
        }

        protected virtual void VisitExistsToken(ExistsToken token, VisitorState state)
        {
            state.Append(Sym._EXISTS_);
            VisitToken(token.Token, state);
        }

        protected virtual void VisitNotToken(NotToken token, VisitorState state)
        {
            state.Append(Sym._NOT_op);
            VisitToken(token.Token, state);
            state.Append(Sym._cp);
        }

        protected virtual void VisitIsNullToken(IsNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Append(Sym._IS_NULL);
        }

        protected virtual void VisitIsNotNullToken(IsNotNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state, false);
            state.Append(Sym._IS_NOT_NULL);
        }

        protected virtual void VisitBetweenToken(BetweenToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Append(Sym._BETWEEN_);
            VisitToken(token.First, state);
            state.Append(Sym._AND_);
            VisitToken(token.Second, state);
        }

        protected virtual void VisitInToken(InToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            VisitTokenSet(token.Set, state, Sym._IN_op, Sym.COMMA_, Sym.cp);
        }

        protected virtual void VisitNotInToken(NotInToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            VisitTokenSet(token.Set, state, Sym._NOT_IN_op, Sym.COMMA_, Sym.cp);
        }
        protected virtual void VisitCommentToken(CommentToken token, VisitorState state)
        {
            state.Append(" /* ");

            VisitToken(token.Content, state);

            state.Append(" */ ");
        }






        protected virtual void VisitCommentStatement(CommentStatement statement, VisitorState state)
        {
            state.Append(" /* ");
            VisitStatement(statement.Content, state);
            state.Append(" */ ");
        }
        protected virtual void VisitSnippetStatement(SnippetStatement statement, VisitorState state)
        {
            state.Append(statement.Value);
        }
        protected virtual void VisitUnion(Union statement, VisitorState state)
        {
            VisitStatement(statement.First, state);

            state.Append((statement.All ? Sym._UNION_ALL_: Sym._UNION_));

            VisitStatement(statement.Second, state);
        }
        protected virtual void VisitExcept(Except statement, VisitorState state)
        {
            VisitStatement(statement.First, state);

            state.Append(Sym._EXCEPT_);

            VisitStatement(statement.Second, state);
        }
        protected virtual void VisitIntersect(Intersect statement, VisitorState state)
        {
            VisitStatement(statement.First, state);
            state.Append(Sym._INTERSECT_);
            VisitStatement(statement.Second, state);
        }
        protected virtual void VisitFrom(IEnumerable<Token> recordsets, VisitorState state)
        {
            VisitTokenSet(recordsets, state, Sym._FROM_, Sym.COMMA_, String.Empty, true);
        }

        protected virtual void VisitFrom(Token recordset, VisitorState state)
        {
            if (recordset != null)
            {
                state.Append(Sym._FROM_);
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
                        state.Append(Sym._ON_);
                        VisitToken(@join.On, state);
                    }
                }
            }
        }

        protected virtual void VisitGroupBy(ICollection<Name> groupBy, VisitorState state)
        {
            if (groupBy.Count > 0)
            {
                state.Append(Sym._GROUP_BY_);
                state.Append(string.Join(Sym.COMMA_, groupBy.Select(ResolveName)));
            }
        }

        protected virtual void VisitHaving(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Append(Sym._HAVING_);
                VisitToken(whereToken, state);
            }
        }
        protected virtual void VisitOrderBy(ICollection<Order> orderBy, VisitorState state)
        {
            if (orderBy.Count > 0)
            {
                state.Append(Sym._ORDER_BY_);
                state.Append(string.Join(Sym.COMMA_,
                    orderBy.Select(n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym._ASC : Sym._DESC))));
            }
        }



        protected readonly string[] JoinStrings =
        {
            " INNER JOIN ", //Inner = 0,
            " LEFT OUTER JOIN ", //LeftOuter = 1,
            " RIGHT OUTER JOIN ", //RightOuter = 2,
            " FULL OUTER JOIN ", //FullOuter = 3,
            " CROSS JOIN " //Cross = 4,
        };

        protected class Sym
        {
            public const string _ALL_ = " ALL ";
            public const string _AND_ = " AND ";
            public const string _ANY_ = " ANY ";
            public const string _AUTOINCREMENT = " AUTOINCREMENT";
            public const string _AS_ = " AS ";
            public const string _AS_osp = " AS [";
            public const string _ASC = " ASC";
            public const string _BETWEEN_ = " BETWEEN ";
            public const string _cp = " )";
            public const string _CLUSTERED = " CLUSTERED";
            public const string _NONCLUSTERED = " NONCLUSTERED";

            public const string _INCLUDE_op = " INCLUDE (";

            public const string _DESC = " DESC";
            public const string _DEFAULT_op = " DEFAULT (";
            public const string _EXISTS_ = " EXISTS ";
            public const string _FROM_ = " FROM ";
            public const string _GROUP_BY_ = " GROUP BY ";
            public const string _HAVING_ = " HAVING ";
            public const string _IDENTITY_ = " IDENTITY ";
            public const string _IN_op = " IN (";
            public const string _INTO_ = " INTO ";
            public const string _IS_NOT_NULL = " IS NOT NULL";
            public const string _IS_NULL = " IS NULL";
            public const string _LIKE_ = " LIKE ";
            public const string _MAXDOP = " MAXDOP";
            public const string _NOT_IN_op = " NOT IN (";
            public const string _NOT_op = " NOT (";
            public const string _NOT_NULL = " NOT NULL";

            public const string _INTERSECT_ = " INTERSECT ";
            public const string _EXCEPT_ = " EXCEPT ";
            public const string _UNION_ = " UNION ";
            public const string _UNION_ALL_ = " UNION ALL ";
            

            public const string _NULL = " NULL";


            public const string _ABORT = " ABORT";
            public const string _FAIL = " FAIL";
            public const string _IGNORE = " IGNORE";
            public const string _REPLACE = " REPLACE";
            public const string _ROLLBACK = " ROLLBACK";

            public const string _ON_ = " ON ";
            public const string _ON_CONFLICT = " ON CONFLICT";


            public const string _ONLINE = " ONLINE";
            public const string _op = " (";
            public const string _ORDER_BY_ = " ORDER BY ";
            public const string _osp = " [";
            public const string _OUTPUT_ = " OUTPUT ";
            public const string _PERCENT = " PERCENT";
            public const string _PRIMARY_KEY = " PRIMARY KEY";

            public const string _TOP_op = " TOP (";
            public const string _WHERE_ = " WHERE ";
            public const string _WITH_ = " WITH ";
            public const string _WITH = " WITH";
            public const string _WITH_TIES = " WITH TIES";
            public const string ALTER_VIEW_ = "ALTER VIEW ";
            public const string COMMA_ = ", ";
            public const string cp = ")";
            public const string cpsc = ");";

            public const string CONSTRAINT_ = "CONSTRAINT ";
            public const string _UNIQUE = " UNIQUE";

            public const string CREATE = "CREATE";
            public const string CREATE_TEMP_TABLE_ = "CREATE TEMPORARY TABLE ";
            public const string CREATE_TABLE_ = "CREATE TABLE ";
            public const string CREATE_VIEW_ = "CREATE VIEW ";
            public const string CREATE_TEMPORARY_VIEW_ = "CREATE TEMPORARY VIEW ";
            public const string csp = "]";
            public const string DELETE = "DELETE";
            public const string DROP_INDEX_ = "DROP INDEX ";
            public const string DROP_VIEW_ = "DROP VIEW ";
            public const string DROP_TABLE_ = "DROP TABLE ";

            public const string IF_NOT_EXISTS_ = "IF NOT EXISTS ";
            public const string IF_EXISTS_ = "IF EXISTS ";

            public const string REINDEX_ = "REINDEX ";
            public const string _LIMIT_ = " LIMIT ";

            public const string _OFFSET_ = " OFFSET ";
            public const string _ROWS = " ROWS";
            public const string _FETCH_NEXT_ = " FETCH NEXT ";
            public const string _ROWS_ONLY = " ROWS ONLY";

            public const string BEGIN_TRANSACTION = "BEGIN TRANSACTION";
            public const string BEGIN_IMMIDIATE_TRANSACTION = "BEGIN IMMIDIATE TRANSACTION";
            public const string BEGIN_DEFERRED_TRANSACTION = "BEGIN DEFERRED TRANSACTION";
            public const string BEGIN_EXCLUSIVE_TRANSACTION = "BEGIN EXCLUSIVE TRANSACTION";
            

            public const string _INDEX_ = " INDEX ";

            public const string NULL = "NULL";
            public const string OFF = "OFF";
            public const string ON = "ON";
            public const string op = "(";
            public const string osp = "[";
            public const string sc = ";";
            public const string SPACE = " ";
            public const string SELECT = "SELECT";
            public const string _DISTINCT = " DISTINCT";
            


            public const string EqualsVal = " = ";
            public const string AssignVal = " = ";
            public const string NotEqualVal = " <> ";
            public const string LessVal = " < ";
            public const string NotLessVal = " !< ";
            public const string LessOrEqualVal = " <= ";
            public const string GreaterVal = " > ";
            public const string NotGreaterVal = " !> ";
            public const string GreaterOrEqualVal = " >= ";
            public const string AndVal = " AND ";
            public const string OrVal = " OR ";
            public const string PlusVal = " + ";
            public const string MinusVal = " - ";
            public const string DivideVal = " / ";
            public const string ModuloVal = " % ";
            public const string MultiplyVal = " * ";
            public const string BitwiseAndVal = " & ";
            public const string BitwiseOrVal = " | ";
            public const string BitwiseXorVal = " ^ ";
            public const string BitwiseNotVal = " ~ ";
            public const string PlusEqVal = " += ";
            public const string MinusEqVal = " -= ";
            public const string DivideEqVal = " /= ";
            public const string ModuloEqVal = " %= ";
            public const string MultiplyEqVal = " *= ";
            public const string BitwiseAndEqVal = " &= ";
            public const string BitwiseOrEqVal = " |= ";
            public const string BitwiseXorEqVal = " ^= ";
            public const string BitwiseNotEqVal = " ~= ";
        }
    }
}
