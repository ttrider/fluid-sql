using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.SqlServer
{
    public static class SqlServerVisitor
    {
        //static SqlServerVisitor()
        //{
        //    // scan for IStatements
        //    AppDomain.CurrentDomain.GetAssemblies().SelectMany(a=>a.GetTypes()).Where(t=>t.GetInterface("IStatement")!=null)


        //}

        private const string EqualsVal = " = ";
        private const string NotEqualVal = " <> ";
        private const string LessVal = " < ";
        private const string LessOrEqualVal = " <= ";
        private const string GreaterVal = " > ";
        private const string GreaterOrEqualVal = " >= ";
        private const string AndVal = " AND ";
        private const string OrVal = " OR ";
        private const string PlusVal = " + ";
        private const string MinusVal = " - ";
        private const string DivideVal = " / ";
        private const string ModuleVal = " % ";
        private const string MultiplyVal = " * ";

        private static readonly string[] DbTypeStrings =
        {
            "BIGINT",//BigInt = 0,
            "BINARY",//Binary = 1,      ()
            "BIT",//Bit = 2,
            "CHAR",//Char = 3,          ()
            "DATETIME",//DateTime = 4,
            "DECIMAL",//Decimal = 5,    (,)
            "FLOAT",//Float = 6,
            "IMAGE",//Image = 7,
            "INT", //Int = 8,
            "MONEY",//Money = 9,
            "NCHAR", //NChar = 10,      ()
            "NTEXT", //NText = 11,
            "NVARCHAR",//NVarChar = 12, ()
            "REAL", //Real = 13,
            "UNIQUEIDENTIFIER",//UniqueIdentifier = 14,
            "SMALLDATETIME",//SmallDateTime = 15,
            "SMALLINT", //SmallInt = 16,
            "SMALLMONEY",//SmallMoney = 17,
            "TEXT",//Text = 18,
            "TIMESTAMP",//Timestamp = 19,
            "TINYINT",//TinyInt = 20,
            "VARBINARY",//VarBinary = 21,   ()
            "VARCHAR",//VarChar = 22,       ()
            "SQL_VARIANT", //Variant = 23,
            "Xml", //Xml = 25,
            "", //Udt = 29,
            "", //Structured = 30,
            "DATE",//Date = 31,
            "TIME", //Time = 32,            ()
            "DATETIME2", //DateTime2 = 33,  ()
            "DateTimeOffset", //DateTimeOffset = 34, ()
        };



        private static readonly string[] JoinStrings = new string[]
        {
            " INNER JOIN ", //Inner = 0,
            " LEFT OUTER JOIN ", //LeftOuter = 1,
            " RIGHT OUTER JOIN ", //RightOuter = 2,
            " FULL OUTER JOIN ", //FullOuter = 3,
            " CROSS JOIN ", //Cross = 4,
        };



        private static readonly Dictionary<Type, Action<Token, VisitorState>> TokenVisitors =
            new Dictionary<Type, Action<Token, VisitorState>>()
            {
                {typeof (SelectStatement), VisitStatementToken},
                {typeof (Union), VisitStatementToken},
                {typeof (Intersect), VisitStatementToken},
                {typeof (Except), VisitStatementToken},

                {typeof (Scalar), VisitScalarToken},
                {typeof (Name), VisitNameToken},
                {typeof (Parameter), VisitParameterToken},
                {typeof (Snippet), VisitSnippetToken},
                {typeof (Function), VisitFunctionToken},

                {typeof (IsEqualsToken), VisitIsEqualsToken},
                {typeof (NotEqualToken), VisitNotEqualToken},
                {typeof (LessToken), VisitLessToken},
                {typeof (LessOrEqualToken), VisitLessOrEqualToken},
                {typeof (GreaterToken), VisitGreaterToken},
                {typeof (GreaterOrEqualToken), VisitGreaterOrEqualToken},
                {typeof (AndToken), VisitAndToken},
                {typeof (OrToken), VisitOrToken},
                {typeof (PlusToken), VisitPlusToken},
                {typeof (MinusToken), VisitMinusToken},
                {typeof (DivideToken), VisitDivideToken},
                {typeof (ModuleToken), VisitModuleToken},
                {typeof (MultiplyToken), VisitMultiplyToken},

                {typeof (ContainsToken), VisitContainsToken},
                {typeof (StartsWithToken), VisitStartsWithToken},
                {typeof (EndsWithToken), VisitEndsWithToken},

                {typeof (GroupToken), VisitGroupToken},
                {typeof (NotToken), VisitNotToken},
                {typeof (IsNullToken), VisitIsNullToken},
                {typeof (IsNotNullToken), VisitIsNotNullToken},

                {typeof (BetweenToken), VisitBetweenToken},
                {typeof (InToken), VisitInToken},
                {typeof (NotInToken), VisitNotInToken},
            };

        private static readonly Dictionary<Type, Action<IStatement, VisitorState>> StatementVisitors =
            new Dictionary<Type, Action<IStatement, VisitorState>>()
            {
                {typeof (DeleteStatement), VisitDelete},

                {typeof (SelectStatement), VisitSelect},
                {typeof (Union), VisitUnion},
                {typeof (Intersect), VisitIntersect},
                {typeof (Except), VisitExcept},
                {typeof (BeginTransactionStatement), VisitBeginTransaction},
                {typeof (CommitTransactionStatement), VisitCommitTransaction},
                {typeof (RollbackTransactionStatement), VisitRollbackTransaction},
                {typeof (StatementsStatement), VisitStatementsStatement},
                {typeof (SaveTransactionStatement), VisitSaveTransaction},

                {typeof (DeclareStatement), VisitDeclareStatement},
                {typeof (IfStatement), VisitIfStatement},

                {typeof (DropTableStatement), VisitDropTableStatement},
                //{typeof (CreateTableStatement), VisitCreateTableStatement},
            };

        public static VisitorState Compile(IStatement statement)
        {
            var state = new VisitorState();

            VisitStatement(statement, state);
            // we need to make sure that the last non-whitespace character
            // is ';'
            for (int i = state.Buffer.Length - 1; i >= 0; i--)
            {
                var ch = state.Buffer[i];
                if (!Char.IsWhiteSpace(ch))
                {
                    if (ch == ';')
                    {
                        break;
                    }
                    state.Buffer.Append(";");
                    break;
                }
            }

            return state;
        }

        #region Statements
        static void VisitStatement(IStatement statement, VisitorState state)
        {
            StatementVisitors[statement.GetType()](statement, state);
        }

        static void VisitUnion(IStatement statement, VisitorState state)
        {
            var unionStatement = (Union)statement;

            VisitStatement(unionStatement.First, state);

            state.Buffer.Append((unionStatement.All ? " UNION ALL " : " UNION "));

            VisitStatement(unionStatement.Second, state);
        }


        static void VisitExcept(IStatement statement, VisitorState state)
        {
            var exceptStatement = (Except)statement;

            VisitStatement(exceptStatement.First, state);

            state.Buffer.Append(" EXCEPT ");

            VisitStatement(exceptStatement.Second, state);
        }
        static void VisitIntersect(IStatement statement, VisitorState state)
        {
            var intersectStatement = (Intersect)statement;

            VisitStatement(intersectStatement.First, state);
            state.Buffer.Append(" INTERSECT ");
            VisitStatement(intersectStatement.Second, state);
        }

        static void VisitSelect(IStatement statement, VisitorState state)
        {
            var selectStatement = (SelectStatement)statement;

            state.Buffer.Append("SELECT");

            if (selectStatement.Distinct)
            {
                state.Buffer.Append(" DISTINCT");
            }

            Visit(selectStatement.Top, state);
            VisitOutput(selectStatement.Output, state, false);

            if (selectStatement.Into != null)
            {
                state.Buffer.Append(" INTO ");
                state.Buffer.Append(selectStatement.Into.FullName);

            }

            VisitFrom(selectStatement.From, state);

            VisitJoin(selectStatement.Joins, state);

            VisitWhere(selectStatement.Where, state);

            VisitGroupBy(selectStatement.GroupBy, state);

            VisitOrderBy(selectStatement.OrderBy, state);


            //WHERE
            //WITH CUBE or WITH ROLLUP
            //HAVING
        }

        static void VisitDelete(IStatement statement, VisitorState state)
        {
            var deleteStatement = (DeleteStatement)statement;

            state.Buffer.Append("DELETE");

            Visit(deleteStatement.Top, state);

            VisitFrom(deleteStatement.From, state);

            VisitOutput(deleteStatement.Output, state, true);

            VisitWhere(deleteStatement.Where, state);
        }


        private static bool VisitTransactionName(TransactionStatement statement, VisitorState state)
        {
            if (statement.Name != null)
            {
                state.Buffer.Append(" ");
                state.Buffer.Append(statement.Name.FullName);
                return true;
            }
            if (statement.Parameter != null)
            {
                state.Buffer.Append(" ");
                state.Buffer.Append(statement.Parameter.Name);
                state.Parameters.Add(statement.Parameter);
                return true;
            }
            return false;
        }

        private static void VisitBeginTransaction(IStatement statement, VisitorState state)
        {
            var ts = (BeginTransactionStatement)statement;
            state.Buffer.Append("BEGIN TRANSACTION");
            if (VisitTransactionName(ts, state) && !string.IsNullOrWhiteSpace(ts.Description))
            {
                state.Buffer.Append(" WITH MARK '");
                state.Buffer.Append(ts.Description);
                state.Buffer.Append("'");
            }
        }
        private static void VisitCommitTransaction(IStatement statement, VisitorState state)
        {
            var ts = (TransactionStatement)statement;
            state.Buffer.Append("COMMIT TRANSACTION");
            VisitTransactionName(ts, state);
        }
        private static void VisitRollbackTransaction(IStatement statement, VisitorState state)
        {
            var ts = (TransactionStatement)statement;
            state.Buffer.Append("ROLLBACK TRANSACTION");
            VisitTransactionName(ts, state);
        }
        private static void VisitSaveTransaction(IStatement statement, VisitorState state)
        {
            var ts = (TransactionStatement)statement;
            state.Buffer.Append("SAVE TRANSACTION");
            VisitTransactionName(ts, state);
        }


        private static void VisitStatementsStatement(IStatement statement, VisitorState state)
        {
            var ss = (StatementsStatement)statement;
            foreach (var subStatement in ss.Statements)
            {
                VisitStatement(subStatement, state);
                state.Buffer.AppendLine(";");
            }
        }

        private static void VisitDeclareStatement(IStatement statement, VisitorState state)
        {
            var ss = (DeclareStatement)statement;

            if (ss.Variable != null)
            {
                state.Variables.Add(ss.Variable);

                state.Buffer.Append("DECLARE ");
                state.Buffer.Append(ss.Variable.Name);

                if (ss.Variable.DbType.HasValue)
                {
                    state.Buffer.Append(" ");
                    state.Buffer.Append(DbTypeStrings[(int)ss.Variable.DbType]);
                }

                if (ss.Variable.Length.HasValue || ss.Variable.Precision.HasValue || ss.Variable.Scale.HasValue)
                {
                    state.Buffer.Append("(");
                    if (ss.Variable.Length.HasValue)
                    {
                        state.Buffer.Append(ss.Variable.Length.Value == -1 ? "MAX" : ss.Variable.Length.Value.ToString());
                    }
                    else if (ss.Variable.Precision.HasValue)
                    {
                        state.Buffer.Append(ss.Variable.Precision.Value);

                        if (ss.Variable.Scale.HasValue)
                        {
                            state.Buffer.Append(",");
                            state.Buffer.Append(ss.Variable.Scale.Value);
                        }
                    }

                    state.Buffer.Append(")");
                }

                if (ss.Initializer != null)
                {
                    state.Buffer.Append(" = ");
                    VisitToken(ss.Initializer, false, state);
                }
            }
        }

        private static void VisitIfStatement(IStatement statement, VisitorState state)
        {
            var ifs = (IfStatement)statement;

            if (ifs.Condition != null)
            {
                state.Buffer.Append("IF ");
                VisitToken(ifs.Condition, false, state);

                if (ifs.Then != null)
                {
                    state.Buffer.Append("\r\nBEGIN;\r\n");
                    VisitStatement(ifs.Then, state);
                    state.Buffer.Append("END;\r\n");

                    if (ifs.Else != null)
                    {
                        state.Buffer.Append("ELSE\r\nBEGIN;\r\n");
                        VisitStatement(ifs.Else, state);
                        state.Buffer.Append("END;\r\n");
                    }
                }
            }
        }


        private static void VisitDropTableStatement(IStatement statement, VisitorState state)
        {
            var s = (DropTableStatement)statement;

            if (s.CheckExists)
            {
                state.Buffer.Append("IF OBJECT_ID(N'");
                state.Buffer.Append(s.Name.FullName);
                state.Buffer.Append("',N'U') IS NOT NULL ");
            }

            state.Buffer.Append("DROP TABLE ");
            state.Buffer.Append(s.Name.FullName);
            state.Buffer.Append(";");
        }
        //private static void VisitCreateTableStatement(IStatement statement, VisitorState state)
        //{
        //    var s = (CreateTableStatement)statement;
        //    state.Buffer.Append("CREATE TABLE ");
        //    state.Buffer.Append(s.Name.FullName);
        //    state.Buffer.Append(" (");


        //    state.Buffer.Append(");");
        //}

        #endregion Statements

        #region Select Parts
        private static void VisitJoin(List<Join> list, VisitorState state)
        {
            if (list.Count > 0)
            {
                foreach (var join in list)
                {
                    state.Buffer.Append(JoinStrings[(int)join.Type]);
                    VisitToken(join.Source, true, state);

                    if (join.On != null)
                    {
                        state.Buffer.Append(" ON ");
                        VisitToken(join.On, false, state);
                    }
                }
            }
        }

        private static void VisitOrderBy(List<Order> orderBy, VisitorState state)
        {
            if (orderBy.Count > 0)
            {
                state.Buffer.Append(" ORDER BY ");
                state.Buffer.Append(string.Join(", ", orderBy.Select(n => n.Column.FullName + (n.Direction == Direction.Asc ? " ASC" : " DESC"))));
            }
        }

        static void VisitGroupBy(List<Name> groupBy, VisitorState state)
        {
            if (groupBy.Count > 0)
            {
                state.Buffer.Append(" GROUP BY ");
                state.Buffer.Append(string.Join(", ", groupBy.Select(n => n.FullName)));
            }
        }

        static void VisitFrom(List<Token> recordsets, VisitorState state)
        {
            if (recordsets.Count > 0)
            {
                var separator = " FROM ";
                foreach (var recordset in recordsets)
                {
                    state.Buffer.Append(separator);
                    separator = ", ";

                    VisitToken(recordset, true, state);
                }
            }
        }
        static void VisitFrom(Token recordset, VisitorState state)
        {
            if (recordset != null)
            {
                state.Buffer.Append(" FROM ");
                VisitToken(recordset, true, state);
            }
        }

        static void VisitOutput(List<Token> columns, VisitorState state, bool includeName)
        {
            if (includeName)
            {
                if (columns.Count > 0)
                {
                    state.Buffer.Append(" OUTPUT");
                    string separator = " ";
                    foreach (var column in columns)
                    {
                        state.Buffer.Append(separator);
                        separator = " , ";

                        VisitToken(column, true, state);
                    }
                }
            }
            else
            {
                if (columns.Count == 0)
                {
                    state.Buffer.Append(" *");
                }
                else
                {
                    string separator = " ";
                    foreach (var column in columns)
                    {
                        state.Buffer.Append(separator);
                        separator = " , ";

                        VisitToken(column, true, state);
                    }
                }
            }
        }

        static void Visit(Top top, VisitorState state)
        {
            if (top != null)
            {
                state.Buffer.Append(" TOP ");
                if (top.Value.HasValue)
                {
                    state.Buffer.Append(top.Value.Value);
                }
                else if (top.Parameters.Count > 0)
                {
                    state.AppendParameters(top.Parameters);
                    state.Buffer.Append(top.Parameters[0].Name);
                }

                if (top.Percent)
                {
                    state.Buffer.Append(" PERCENT");
                }
                if (top.WithTies)
                {
                    state.Buffer.Append(" WITH TIES");
                }

            }

        }

        private static void VisitWhere(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Buffer.Append(" WHERE ");
                VisitToken(whereToken, false, state);
            }
        }
        #endregion Select Parts

        #region Tokens
        static void VisitToken(Token token, bool includeAlias, VisitorState state)
        {
            TokenVisitors[token.GetType()](token, state);

            if (includeAlias && !string.IsNullOrWhiteSpace(token.Alias))
            {
                state.Buffer.Append(" AS [");
                state.Buffer.Append(token.Alias);
                state.Buffer.Append("]");
            }

            state.Parameters.AddRange(token.Parameters);
        }
        static void VisitScalarToken(Token token, VisitorState state)
        {
            var value = ((Scalar)token).Value;

            if (value == null) return;

            if ((value is Boolean)
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
                state.Buffer.Append(value);
            }
            else
            {
                state.Buffer.Append("'" + value + "'");
            }
        }
        static void VisitStatementToken(Token token, VisitorState state)
        {
            var value = ((IStatement)token);

            state.Buffer.Append("(");
            VisitStatement(value, state);
            state.Buffer.Append(")");

        }
        static void VisitNameToken(Token token, VisitorState state)
        {
            var value = ((Name)token).FullName;
            state.Buffer.Append(value);
        }
        static void VisitParameterToken(Token token, VisitorState state)
        {
            var value = ((Parameter)token).Name;
            state.Buffer.Append(value);
        }
        static void VisitSnippetToken(Token token, VisitorState state)
        {
            var value = ((Snippet)token).Value;
            state.Buffer.Append(value);
        }
        static void VisitFunctionToken(Token token, VisitorState state)
        {
            var value = ((Function)token);
            state.Buffer.Append(" ");
            state.Buffer.Append(value.Name);
            state.Buffer.Append("(");

            var separator = "";
            foreach (var arg in value.Arguments)
            {
                state.Buffer.Append(separator);
                separator = ", ";
                VisitToken(arg, false, state);
            }

            state.Buffer.Append(")");
        }
        static void VisitBinaryToken(Token token, VisitorState state, string operation)
        {
            var value = (BinaryToken)token;
            VisitToken(value.First, false, state);
            state.Buffer.Append(operation);
            VisitToken(value.Second, false, state);
        }
        static void VisitIsEqualsToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, EqualsVal);
        }
        static void VisitNotEqualToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, NotEqualVal);
        }
        static void VisitLessToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, LessVal);
        }
        static void VisitLessOrEqualToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, LessOrEqualVal);
        }
        static void VisitGreaterToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, GreaterVal);
        }
        static void VisitGreaterOrEqualToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, GreaterOrEqualVal);
        }
        static void VisitAndToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, AndVal);
        }
        static void VisitOrToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, OrVal);
        }
        static void VisitPlusToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, PlusVal);
        }
        static void VisitMinusToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, MinusVal);
        }
        static void VisitDivideToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, DivideVal);
        }
        static void VisitModuleToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, ModuleVal);
        }
        static void VisitMultiplyToken(Token token, VisitorState state)
        {
            VisitBinaryToken(token, state, MultiplyVal);
        }

        static void VisitContainsToken(Token token, VisitorState state)
        {
            var value = (BinaryToken)token;
            VisitToken(value.First, false, state);
            state.Buffer.Append(" LIKE '%' + ");
            VisitToken(value.Second, false, state);
            state.Buffer.Append(" + '%'");
        }
        static void VisitStartsWithToken(Token token, VisitorState state)
        {
            var value = (BinaryToken)token;
            VisitToken(value.First, false, state);
            state.Buffer.Append(" LIKE ");
            VisitToken(value.Second, false, state);
            state.Buffer.Append(" + '%'");
        }
        static void VisitEndsWithToken(Token token, VisitorState state)
        {
            var value = (BinaryToken)token;
            VisitToken(value.First, false, state);
            state.Buffer.Append(" LIKE '%' + ");
            VisitToken(value.Second, false, state);
        }


        static void VisitGroupToken(Token token, VisitorState state)
        {
            state.Buffer.Append(" (");
            var value = (GroupToken)token;
            VisitToken(value.Token, false, state);
            state.Buffer.Append(" )");
        }
        static void VisitNotToken(Token token, VisitorState state)
        {
            state.Buffer.Append(" NOT");
            var value = (NotToken)token;
            VisitToken(value.Token, false, state);
        }
        static void VisitIsNullToken(Token token, VisitorState state)
        {
            var value = (IsNullToken)token;
            VisitToken(value.Token, false, state);
            state.Buffer.Append(" IS NULL");
        }
        static void VisitIsNotNullToken(Token token, VisitorState state)
        {
            var value = (IsNotNullToken)token;
            VisitToken(value.Token, false, state);
            state.Buffer.Append(" IS NOT NULL");
        }
        static void VisitBetweenToken(Token token, VisitorState state)
        {
            var value = (BetweenToken)token;

            VisitToken(value.Token, false, state);
            state.Buffer.Append(" BETWEEN");
            VisitToken(value.First, false, state);
            state.Buffer.Append(" AND");
            VisitToken(value.Second, false, state);
        }
        static void VisitInToken(Token token, VisitorState state)
        {
            var value = (InToken)token;

            VisitToken(value.Token, false, state);

            if (value.Set.Count > 0)
            {
                var separator = " IN (";
                foreach (var setToken in value.Set)
                {
                    state.Buffer.Append(separator);
                    separator = ", ";
                    VisitToken(setToken, false, state);
                }
                state.Buffer.Append(")");
            }
        }
        static void VisitNotInToken(Token token, VisitorState state)
        {
            var value = (NotInToken)token;

            VisitToken(value.Token, false, state);

            if (value.Set.Count > 0)
            {
                var separator = " NOT IN (";
                foreach (var setToken in value.Set)
                {
                    state.Buffer.Append(separator);
                    separator = ", ";
                    VisitToken(setToken, false, state);
                }
                state.Buffer.Append(")");
            }
        }
        #endregion Tokens
    }
}
