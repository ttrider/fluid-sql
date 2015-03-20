using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TTRider.FluidSql.Providers.SqlServer
{
    public class SqlServerVisitor : Visitor
    {
        private const string EqualsVal = " = ";
        private const string AssignVal = " = ";
        private const string NotEqualVal = " <> ";
        private const string LessVal = " < ";
        private const string NotLessVal = " !< ";
        private const string LessOrEqualVal = " <= ";
        private const string GreaterVal = " > ";
        private const string NotGreaterVal = " !> ";
        private const string GreaterOrEqualVal = " >= ";
        private const string AndVal = " AND ";
        private const string OrVal = " OR ";

        private const string PlusVal = " + ";
        private const string MinusVal = " - ";
        private const string DivideVal = " / ";
        private const string ModuloVal = " % ";
        private const string MultiplyVal = " * ";

        private const string BitwiseAndVal = " & ";
        private const string BitwiseOrVal = " | ";
        private const string BitwiseXorVal = " ^ ";
        private const string BitwiseNotVal = " ~ ";

        private const string PlusEqVal = " += ";
        private const string MinusEqVal = " -= ";
        private const string DivideEqVal = " /= ";
        private const string ModuloEqVal = " %= ";
        private const string MultiplyEqVal = " *= ";

        private const string BitwiseAndEqVal = " &= ";
        private const string BitwiseOrEqVal = " |= ";
        private const string BitwiseXorEqVal = " ^= ";
        private const string BitwiseNotEqVal = " ~= ";


        private readonly string[] DbTypeStrings =
        {
            "BIGINT", //BigInt = 0,
            "BINARY", //Binary = 1,      ()
            "BIT", //Bit = 2,
            "CHAR", //Char = 3,          ()
            "DATETIME", //DateTime = 4,
            "DECIMAL", //Decimal = 5,    (,)
            "FLOAT", //Float = 6,
            "IMAGE", //Image = 7,
            "INT", //Int = 8,
            "MONEY", //Money = 9,
            "NCHAR", //NChar = 10,      ()
            "NTEXT", //NText = 11,
            "NVARCHAR", //NVarChar = 12, ()
            "REAL", //Real = 13,
            "UNIQUEIDENTIFIER", //UniqueIdentifier = 14,
            "SMALLDATETIME", //SmallDateTime = 15,
            "SMALLINT", //SmallInt = 16,
            "SMALLMONEY", //SmallMoney = 17,
            "TEXT", //Text = 18,
            "TIMESTAMP", //Timestamp = 19,
            "TINYINT", //TinyInt = 20,
            "VARBINARY", //VarBinary = 21,   ()
            "VARCHAR", //VarChar = 22,       ()
            "SQL_VARIANT", //Variant = 23,
            "Xml", //Xml = 25,
            "", //Udt = 29,
            "", //Structured = 30,
            "DATE", //Date = 31,
            "TIME", //Time = 32,            ()
            "DATETIME2", //DateTime2 = 33,  ()
            "DateTimeOffset" //DateTimeOffset = 34, ()
        };


        private readonly string[] JoinStrings =
        {
            " INNER JOIN ", //Inner = 0,
            " LEFT OUTER JOIN ", //LeftOuter = 1,
            " RIGHT OUTER JOIN ", //RightOuter = 2,
            " FULL OUTER JOIN ", //FullOuter = 3,
            " CROSS JOIN " //Cross = 4,
        };


        private static readonly Dictionary<Type, Action<SqlServerVisitor, Token, VisitorState>> TokenVisitors =
            new Dictionary<Type, Action<SqlServerVisitor, Token, VisitorState>>
            {
                {typeof (SelectStatement), (v,t,s)=>v.VisitStatementToken((IStatement)t, s)},
                {typeof (Union),(v,t,s)=>v.VisitStatementToken((IStatement)t,s)},
                {typeof (Intersect),(v,t,s)=>v.VisitStatementToken((IStatement)t,s)},
                {typeof (Except),(v,t,s)=>v.VisitStatementToken((IStatement)t,s)},
                {typeof (Scalar),(v,t,s)=>v.VisitScalarToken((Scalar)t,s)},
                {typeof (Name),(v,t,s)=>v.VisitNameToken((Name)t,s)},
                {typeof (Parameter),(v,t,s)=>v.VisitParameterToken((Parameter)t,s)},
                {typeof (Snippet),(v,t,s)=>v.VisitSnippetToken((Snippet)t,s)},
                {typeof (Function),(v,t,s)=>v.VisitFunctionToken((Function)t,s)},
                {typeof (IsEqualsToken),(v,t,s)=>v.VisitIsEqualsToken((IsEqualsToken)t,s)},
                {typeof (NotEqualToken),(v,t,s)=>v.VisitNotEqualToken((NotEqualToken)t,s)},
                {typeof (LessToken),(v,t,s)=>v.VisitLessToken((LessToken)t,s)},
                {typeof (NotLessToken),(v,t,s)=>v.VisitNotLessToken((NotLessToken)t,s)},
                {typeof (LessOrEqualToken),(v,t,s)=>v.VisitLessOrEqualToken((LessOrEqualToken)t,s)},
                {typeof (GreaterToken),(v,t,s)=>v.VisitGreaterToken((GreaterToken)t,s)},
                {typeof (NotGreaterToken),(v,t,s)=>v.VisitNotGreaterToken((NotGreaterToken)t,s)},
                {typeof (GreaterOrEqualToken),(v,t,s)=>v.VisitGreaterOrEqualToken((GreaterOrEqualToken)t,s)},
                {typeof (AndToken),(v,t,s)=>v.VisitAndToken((AndToken)t,s)},
                {typeof (OrToken),(v,t,s)=>v.VisitOrToken((OrToken)t,s)},
                {typeof (PlusToken),(v,t,s)=>v.VisitPlusToken((PlusToken)t,s)},
                {typeof (MinusToken),(v,t,s)=>v.VisitMinusToken((MinusToken)t,s)},
                {typeof (DivideToken),(v,t,s)=>v.VisitDivideToken((DivideToken)t,s)},
                {typeof (ModuloToken),(v,t,s)=>v.VisitModuloToken((ModuloToken)t,s)},
                {typeof (MultiplyToken),(v,t,s)=>v.VisitMultiplyToken((MultiplyToken)t,s)},
                {typeof (BitwiseAndToken),(v,t,s)=>v.VisitBitwiseAndToken((BitwiseAndToken)t,s)},
                {typeof (BitwiseOrToken),(v,t,s)=>v.VisitBitwiseOrToken((BitwiseOrToken)t,s)},
                {typeof (BitwiseXorToken),(v,t,s)=>v.VisitBitwiseXorToken((BitwiseXorToken)t,s)},
                {typeof (BitwiseNotToken),(v,t,s)=>v.VisitBitwiseNotToken((BitwiseNotToken)t,s)},
                {typeof (ContainsToken),(v,t,s)=>v.VisitContainsToken((ContainsToken)t,s)},
                {typeof (StartsWithToken),(v,t,s)=>v.VisitStartsWithToken((StartsWithToken)t,s)},
                {typeof (EndsWithToken),(v,t,s)=>v.VisitEndsWithToken((EndsWithToken)t,s)},
                {typeof (LikeToken),(v,t,s)=>v.VisitLikeToken((LikeToken)t,s)},
                {typeof (GroupToken),(v,t,s)=>v.VisitGroupToken((GroupToken)t,s)},
                {typeof (NotToken),(v,t,s)=>v.VisitNotToken((NotToken)t,s)},
                {typeof (IsNullToken),(v,t,s)=>v.VisitIsNullToken((IsNullToken)t,s)},
                {typeof (IsNotNullToken),(v,t,s)=>v.VisitIsNotNullToken((IsNotNullToken)t,s)},
                {typeof (ExistsToken),(v,t,s)=>v.VisitExistsToken((ExistsToken)t,s)},
                {typeof (AllToken),(v,t,s)=>v.VisitAllToken((AllToken)t,s)},
                {typeof (AnyToken),(v,t,s)=>v.VisitAnyToken((AnyToken)t,s)},
                {typeof (AssignToken),(v,t,s)=>v.VisitAssignToken((AssignToken)t,s)},
                {typeof (BetweenToken),(v,t,s)=>v.VisitBetweenToken((BetweenToken)t,s)},
                {typeof (InToken),(v,t,s)=>v.VisitInToken((InToken)t,s)},
                {typeof (NotInToken),(v,t,s)=>v.VisitNotInToken((NotInToken)t,s)},
                {typeof (CommentToken),(v,t,s)=>v.VisitCommentToken((CommentToken)t,s)},
                {typeof (StringifyToken),(v,t,s)=>v.VisitStringifyToken((StringifyToken)t,s)},
                {typeof (WhenMatchedThenDelete),(v,t,s)=>v.VisitWhenMatchedThenDelete((WhenMatchedThenDelete)t,s)},
                {typeof (WhenMatchedThenUpdateSet),(v,t,s)=>v.VisitWhenMatchedThenUpdateSet((WhenMatchedThenUpdateSet)t,s)},
                {typeof (WhenNotMatchedThenInsert),(v,t,s)=>v.VisitWhenNotMatchedThenInsert((WhenNotMatchedThenInsert)t,s)},
            };

        private static readonly Dictionary<Type, Action<SqlServerVisitor, IStatement, VisitorState>> StatementVisitors =
            new Dictionary<Type, Action<SqlServerVisitor, IStatement, VisitorState>>
            {
                {typeof (DeleteStatement), (v,stm,s)=>v.VisitDelete((DeleteStatement)stm, s)},
                {typeof (UpdateStatement), (v,stm,s)=>v.VisitUpdate((UpdateStatement)stm, s)},
                {typeof (InsertStatement), (v,stm,s)=>v.VisitInsert((InsertStatement)stm,s)},
                {typeof (SelectStatement), (v,stm,s)=>v.VisitSelect((SelectStatement)stm,s)},
                {typeof (MergeStatement),(v,stm,s)=>v. VisitMerge((MergeStatement)stm,s)},
                {typeof (SetStatement), (v,stm,s)=>v.VisitSet((SetStatement)stm,s)},
                {typeof (Union), (v,stm,s)=>v.VisitUnion((Union)stm,s)},
                {typeof (Intersect), (v,stm,s)=>v.VisitIntersect((Intersect)stm,s)},
                {typeof (Except), (v,stm,s)=>v.VisitExcept((Except)stm,s)},
                {typeof (BeginTransactionStatement), (v,stm,s)=>v.VisitBeginTransaction((BeginTransactionStatement)stm,s)},
                {typeof (CommitTransactionStatement), (v,stm,s)=>v.VisitCommitTransaction((CommitTransactionStatement)stm,s)},
                {typeof (RollbackTransactionStatement), (v,stm,s)=>v.VisitRollbackTransaction((RollbackTransactionStatement)stm,s)},
                {typeof (StatementsStatement), (v,stm,s)=>v.VisitStatementsStatement((StatementsStatement)stm,s)},
                {typeof (SaveTransactionStatement), (v,stm,s)=>v.VisitSaveTransaction((SaveTransactionStatement)stm,s)},
                {typeof (DeclareStatement), (v,stm,s)=>v.VisitDeclareStatement((DeclareStatement)stm,s)},
                {typeof (IfStatement), (v,stm,s)=>v.VisitIfStatement((IfStatement)stm,s)},
                {typeof (CreateTableStatement), (v,stm,s)=>v.VisitCreateTableStatement((CreateTableStatement)stm,s)},
                {typeof (DropTableStatement), (v,stm,s)=>v.VisitDropTableStatement((DropTableStatement)stm,s)},
                {typeof (CreateIndexStatement), (v,stm,s)=>v.VisitCreateIndexStatement((CreateIndexStatement)stm,s)},
                {typeof (AlterIndexStatement), (v,stm,s)=>v.VisitAlterIndexStatement((AlterIndexStatement)stm,s)},
                {typeof (DropIndexStatement), (v,stm,s)=>v.VisitDropIndexStatement((DropIndexStatement)stm,s)},
                {typeof (CommentStatement), (v,stm,s)=>v.VisitCommentStatement((CommentStatement)stm,s)},
                {typeof (StringifyStatement), (v,stm,s)=>v.VisitStringifyStatement((StringifyStatement)stm,s)},
                {typeof (SnippetStatement), (v,stm,s)=>v.VisitSnippetStatement((SnippetStatement)stm,s)},
                {typeof (BreakStatement), (v,stm,s)=>v.VisitBreakStatement((BreakStatement)stm,s)},
                {typeof (ContinueStatement), (v,stm,s)=>v.VisitContinueStatement((ContinueStatement)stm,s)},
                {typeof (GotoStatement), (v,stm,s)=>v.VisitGotoStatement((GotoStatement)stm,s)},
                {typeof (ReturnStatement), (v,stm,s)=>v.VisitReturnStatement((ReturnStatement)stm,s)},
                {typeof (ThrowStatement), (v,stm,s)=>v.VisitThrowStatement((ThrowStatement)stm,s)},
                {typeof (TryCatchStatement), (v,stm,s)=>v.VisitTryCatchStatement((TryCatchStatement)stm,s)},
                {typeof (LabelStatement), (v,stm,s)=>v.VisitLabelStatement((LabelStatement)stm,s)},
                {typeof (WaitforDelayStatement), (v,stm,s)=>v.VisitWaitforDelayStatement((WaitforDelayStatement)stm,s)},
                {typeof (WaitforTimeStatement), (v,stm,s)=>v.VisitWaitforTimeStatement((WaitforTimeStatement)stm,s)},
                {typeof (WhileStatement), (v,stm,s)=>v.VisitWhileStatement((WhileStatement)stm,s)},
                {typeof (CreateViewStatement), (v,stm,s)=>v.VisitCreateViewStatement((CreateViewStatement)stm,s)},
                {typeof (CreateOrAlterViewStatement), (v,stm,s)=>v.VisitCreateOrAlterViewStatement((CreateOrAlterViewStatement)stm,s)},
                {typeof (AlterViewStatement), (v,stm,s)=>v.VisitAlterViewStatement((AlterViewStatement)stm,s)},
                {typeof (DropViewStatement), (v,stm,s)=>v.VisitDropViewStatement((DropViewStatement)stm,s)},
                {typeof (ExecuteStatement), (v,stm,s)=>v.VisitExecuteStatement((ExecuteStatement)stm,s)},
            };

        public static VisitorState Compile(IStatement statement)
        {
            var state = new VisitorState();

            var visitor = new SqlServerVisitor();

            visitor.VisitStatement(statement, state);
            visitor.EnsureSemicolumn(state);

            return state;
        }

        private void EnsureSemicolumn(VisitorState state)
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

        internal static VisitorState Compile(Token token)
        {
            var statement = token as IStatement;
            if (statement != null)
            {
                return Compile(statement);
            }

            var state = new VisitorState();
            var visitor = new SqlServerVisitor();
            visitor.VisitToken(token, state, false);
            return state;
        }

        internal static Name GetTempTableName(Name name)
        {
            var namePart = name.LastPart;

            if (!namePart.StartsWith("#"))
            {
                namePart = "#" + namePart;
            }
            return Sql.Name("tempdb", "", namePart);
        }

        internal static string GetTableVariableName(Name name)
        {
            var namePart = name.LastPart;

            if (!namePart.StartsWith("@"))
            {
                namePart = "@" + namePart;
            }
            return namePart;
        }

        private void Stringify(IStatement statement, VisitorState state)
        {
            Stringify(s => VisitStatement(statement, s), state);
        }

        private void Stringify(Action<VisitorState> fragment, VisitorState state)
        {
            state.Append("N'");

            var startIndex = state.Buffer.Length;
            fragment(state);
            var endIndex = state.Buffer.Length;

            // replace all "'" characters with "''"
            if (endIndex > startIndex)
            {
                state.Buffer.Replace("'", "''", startIndex, endIndex - startIndex);
            }

            state.Append("'");
        }

        private string ResolveName(Name name)
        {
            return name.GetFullName("[", "]");
        }

        private void VisitType(TypedToken typedToken, VisitorState state)
        {
            if (typedToken.DbType.HasValue)
            {
                state.Append(" ");
                state.Append(DbTypeStrings[(int)typedToken.DbType]);
            }

            if (typedToken.Length.HasValue || typedToken.Precision.HasValue || typedToken.Scale.HasValue)
            {
                state.Append("(");
                if (typedToken.Length.HasValue)
                {
                    state.Append(typedToken.Length.Value == -1 ? "MAX" : typedToken.Length.Value.ToString(CultureInfo.InvariantCulture));
                }
                else if (typedToken.Precision.HasValue)
                {
                    state.Append(typedToken.Precision.Value);

                    if (typedToken.Scale.HasValue)
                    {
                        state.Append(",");
                        state.Append(typedToken.Scale.Value);
                    }
                }

                state.Append(")");
            }
        }


        #region Statements

        private void VisitStatement(IStatement statement, VisitorState state)
        {
            StatementVisitors[statement.GetType()](this, statement, state);
        }

        private void VisitUnion(Union statement, VisitorState state)
        {
            VisitStatement(statement.First, state);

            state.Append((statement.All ? " UNION ALL " : " UNION "));

            VisitStatement(statement.Second, state);
        }
        private void VisitExcept(Except statement, VisitorState state)
        {
            VisitStatement(statement.First, state);

            state.Append(" EXCEPT ");

            VisitStatement(statement.Second, state);
        }
        private void VisitIntersect(Intersect statement, VisitorState state)
        {
            VisitStatement(statement.First, state);
            state.Append(" INTERSECT ");
            VisitStatement(statement.Second, state);
        }
        private void VisitSet(SetStatement statement, VisitorState state)
        {
            state.Append("SET ");

            if (statement.Assign != null)
            {
                VisitToken(statement.Assign, state, false);
            }
        }
        private void VisitMerge(MergeStatement statement, VisitorState state)
        {
            state.Append("MERGE");

            VisitTop(statement.Top, state);

            VisitInto(statement.Into, state);

            VisitAlias(statement.Into, state);

            state.Append(" USING ");
            VisitToken(statement.Using, state, false);
            VisitAlias(statement.Using, state);

            state.Append(" ON ");

            VisitToken(statement.On, state, false);

            foreach (var when in statement.WhenMatched)
            {
                state.Append(" WHEN MATCHED");
                if (when.AndCondition != null)
                {
                    state.Append(" AND ");
                    VisitToken(when.AndCondition, state, false);
                }
                state.Append(" THEN");

                VisitToken(when, state, false);
            }

            foreach (var when in statement.WhenNotMatched)
            {
                state.Append(" WHEN NOT MATCHED BY TARGET");
                if (when.AndCondition != null)
                {
                    state.Append(" AND");
                    VisitToken(when.AndCondition, state, false);
                }
                state.Append(" THEN");

                VisitToken(when, state, false);
            }

            foreach (var when in statement.WhenNotMatchedBySource)
            {
                state.Append(" WHEN NOT MATCHED BY SOURCE");
                if (when.AndCondition != null)
                {
                    state.Append(" AND ");
                    VisitToken(when.AndCondition, state, false);
                }
                state.Append(" THEN");

                VisitToken(when, state, false);
            }

            VisitOutput(statement.Output, statement.OutputInto, state);
        }

        private void VisitWhenMatchedThenDelete(WhenMatchedThenDelete token, VisitorState state)
        {
            state.Append(" DELETE");
        }
        private void VisitWhenMatchedThenUpdateSet(WhenMatchedThenUpdateSet token, VisitorState state)
        {
            state.Append(" UPDATE SET");
            VisitTokenSet(token.Set, state, " ", ", ", "", false);
        }
        private void VisitWhenNotMatchedThenInsert(WhenNotMatchedThenInsert token, VisitorState state)
        {
            state.Append(" INSERT");
            if (token.Columns.Count > 0)
            {
                VisitTokenSet(token.Columns, state, " (", ", ", ")", false);
            }
            if (token.Values.Count > 0)
            {
                VisitTokenSet(token.Values, state, " VALUES (", ", ", ")", false);
            }
            else
            {
                state.Append(" DEFAULT VALUES");
            }
        }

        private void VisitSelect(SelectStatement statement, VisitorState state)
        {
            state.Append("SELECT");

            if (statement.Distinct)
            {
                state.Append(" DISTINCT");
            }

            VisitTop(statement.Top, state);

            // assignments
            if (statement.Set.Count > 0)
            {
                VisitTokenSet(statement.Set, state, " ", ", ", "", false);
            }
            else
            {
                // output columns
                if (statement.Output.Count == 0)
                {
                    state.Append(" *");
                }
                else
                {
                    VisitTokenSet(statement.Output, state, " ", ", ", "", true);
                }
            }

            VisitInto(statement.Into, state);

            VisitFrom(statement.From, state);

            VisitJoin(statement.Joins, state);

            VisitWhere(statement.Where, state);

            VisitGroupBy(statement.GroupBy, state);

            VisitHaving(statement.Having, state);

            VisitOrderBy(statement.OrderBy, state);


            //WHERE
            //WITH CUBE or WITH ROLLUP
        }
        private void VisitInto(Name into, VisitorState state)
        {
            if (into != null)
            {
                state.Append(Sym._INTO_);
                state.Append(ResolveName(into));
            }
        }
        private void VisitDelete(DeleteStatement statement, VisitorState state)
        {
            state.Append(Sym.DELETE);

            VisitTop(statement.Top, state);

            if (statement.Joins.Count > 0)
            {
                VisitAlias(statement.From, state, Sym._osp);

                VisitOutput(statement.Output, statement.OutputInto, state);

                VisitFrom(statement.From, state);

                VisitJoin(statement.Joins, state);

                VisitWhere(statement.Where, state);
            }
            else
            {
                VisitFrom(statement.From, state);

                VisitOutput(statement.Output, statement.OutputInto, state);

                VisitWhere(statement.Where, state);
            }
        }
        private void VisitUpdate(UpdateStatement updateStatement, VisitorState state)
        {
            state.Append("UPDATE");

            VisitTop(updateStatement.Top, state);

            state.Append(" ");
            VisitToken(updateStatement.Target, state, true);

            state.Append(" SET");
            VisitTokenSet(updateStatement.Set, state, " ", ", ", "", false);

            VisitOutput(updateStatement.Output, updateStatement.OutputInto, state);

            VisitFrom(updateStatement.From, state);

            VisitJoin(updateStatement.Joins, state);

            VisitWhere(updateStatement.Where, state);
        }
        private void VisitInsert(InsertStatement insertStatement, VisitorState state)
        {
            state.Append("INSERT");

            VisitTop(insertStatement.Top, state);

            VisitInto(insertStatement.Into, state);

            VisitTokenSet(insertStatement.Columns, state, " (", ", ", ")", true);

            VisitOutput(insertStatement.Output, insertStatement.OutputInto, state);

            if (insertStatement.DefaultValues)
            {
                state.Append(" DEFAULT VALUES");
            }
            else if (insertStatement.Values.Count > 0)
            {
                var separator = " VALUES";
                foreach (var valuesSet in insertStatement.Values)
                {
                    state.Append(separator);
                    separator = ",";

                    VisitTokenSet(valuesSet, state, " (", ", ", ")", false);
                }
            }
            else if (insertStatement.From != null)
            {
                state.Append(" ");
                VisitStatement(insertStatement.From, state);
            }
        }
        private bool VisitTransactionName(TransactionStatement statement, VisitorState state)
        {
            if (statement.Name != null)
            {
                state.Append(Sym.SPACE);
                state.Append(ResolveName(statement.Name));
                return true;
            }
            if (statement.Parameter != null)
            {
                state.Append(Sym.SPACE);
                state.Append(statement.Parameter.Name);
                state.Parameters.Add(statement.Parameter);
                return true;
            }
            return false;
        }
        private void VisitBeginTransaction(BeginTransactionStatement statement, VisitorState state)
        {
            state.Append("BEGIN TRANSACTION");
            if (VisitTransactionName(statement, state) && !string.IsNullOrWhiteSpace(statement.Description))
            {
                state.Append(" WITH MARK '");
                state.Append(statement.Description);
                state.Append("'");
            }
        }
        private void VisitCommitTransaction(TransactionStatement statement, VisitorState state)
        {
            state.Append("COMMIT TRANSACTION");
            VisitTransactionName(statement, state);
        }
        private void VisitRollbackTransaction(TransactionStatement statement, VisitorState state)
        {
            state.Append("ROLLBACK TRANSACTION");
            VisitTransactionName(statement, state);
        }
        private void VisitSaveTransaction(TransactionStatement statement, VisitorState state)
        {
            state.Append("SAVE TRANSACTION");
            VisitTransactionName(statement, state);
        }
        private void VisitStatementsStatement(StatementsStatement statement, VisitorState state)
        {
            var last = state.Buffer.Length - 1;

            if (last >= 0 && state.Buffer[last] != '\n')
            {
                state.Append("\r\n");
            }

            var separator = string.Empty;
            foreach (var subStatement in statement.Statements)
            {
                state.Append(separator);
                separator = "\r\n";
                VisitStatement(subStatement, state);
                EnsureSemicolumn(state);
            }
        }
        private void VisitDeclareStatement(DeclareStatement statement, VisitorState state)
        {
            if (statement.Variable != null)
            {
                state.Variables.Add(statement.Variable);

                state.Append("DECLARE ");
                state.Append(statement.Variable.Name);

                VisitType(statement.Variable, state);

                if (statement.Initializer != null)
                {
                    state.Append(" = ");
                    VisitToken(statement.Initializer, state, false);
                }
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private void VisitBreakStatement(BreakStatement statement, VisitorState state)
        {
            state.Append("BREAK");
        }
        private void VisitContinueStatement(ContinueStatement statement, VisitorState state)
        {
            state.Append("CONTINUE");
        }
        private void VisitGotoStatement(GotoStatement statement, VisitorState state)
        {
            state.Append("GOTO ");
            state.Append(statement.Label);
        }
        private void VisitReturnStatement(ReturnStatement statement, VisitorState state)
        {
            state.Append("RETURN");
            if (statement.ReturnExpression != null)
            {
                state.Append(" ");
                VisitToken(statement.ReturnExpression, state, false);
            }

        }
        private void VisitThrowStatement(ThrowStatement statement, VisitorState state)
        {
            state.Append("THROW");
            if (statement.ErrorNumber != null && statement.Message != null && statement.State != null)
            {
                state.Append(" ");
                VisitToken(statement.ErrorNumber, state, false);
                state.Append(", ");
                VisitToken(statement.Message, state, false);
                state.Append(", ");
                VisitToken(statement.State, state, false);
            }
        }
        private void VisitTryCatchStatement(TryCatchStatement stmt, VisitorState state)
        {
            state.Append("BEGIN TRY\r\n");
            VisitStatement(stmt.TryStatement, state);
            state.Append(";\r\nEND TRY\r\nBEGIN CATCH\r\n");
            if (stmt.CatchStatement != null)
            {
                VisitStatement(stmt.CatchStatement, state);
                state.Append(";\r\nEND CATCH");
            }
        }
        private void VisitLabelStatement(LabelStatement stmt, VisitorState state)
        {
            state.Append(stmt.Label);
            state.Append(":");
        }
        private void VisitWaitforDelayStatement(WaitforDelayStatement stmt, VisitorState state)
        {
            state.Append("WAITFOR DELAY N'");
            state.Append(stmt.Delay.ToString("HH:mm:ss"));
            state.Append("'");
        }
        private void VisitWaitforTimeStatement(WaitforTimeStatement stmt, VisitorState state)
        {
            state.Append("WAITFOR TIME N'");
            state.Append(stmt.Time.ToString("yyyy-MM-ddTHH:mm:ss"));
            state.Append("'");
        }
        private void VisitWhileStatement(WhileStatement stmt, VisitorState state)
        {
            if (stmt.Condition != null)
            {
                state.Append("WHILE ");
                VisitToken(stmt.Condition, state, false);

                if (stmt.Do != null)
                {
                    state.Append("\r\nBEGIN;\r\n");
                    VisitStatement(stmt.Do, state);
                    state.Append("\r\nEND;");
                }
            }
        }
        private void VisitIfStatement(IfStatement ifs, VisitorState state)
        {
            if (ifs.Condition != null)
            {
                state.Append("IF ");
                VisitToken(ifs.Condition, state, false);

                if (ifs.Then != null)
                {
                    state.Append("\r\nBEGIN;\r\n");
                    VisitStatement(ifs.Then, state);
                    state.Append("\r\nEND;");

                    if (ifs.Else != null)
                    {
                        state.Append("\r\nELSE\r\nBEGIN;\r\n");
                        VisitStatement(ifs.Else, state);
                        state.Append("\r\nEND;");
                    }
                }
            }
        }
        private void VisitDropTableStatement(DropTableStatement statement, VisitorState state)
        {
            var tableName = ResolveName((statement.IsTemporary) ? GetTempTableName(statement.Name) : statement.Name);

            if (statement.CheckExists)
            {
                state.Append("IF OBJECT_ID(N'");
                state.Append(tableName);
                state.Append("',N'U') IS NOT NULL ");
            }

            state.Append("DROP TABLE ");
            state.Append(tableName);
            state.Append(";");
        }
        private void VisitCreateTableStatement(CreateTableStatement createStatement, VisitorState state)
        {
            if (createStatement.IsTableVariable)
            {
                state.Append("DECLARE ");
                state.Append(GetTableVariableName(createStatement.Name));
                state.Append(" TABLE");
            }
            else
            {
                var tableName =
                    ResolveName((createStatement.IsTemporary) ? GetTempTableName(createStatement.Name) : createStatement.Name);

                if (createStatement.CheckIfNotExists)
                {
                    state.Append("IF OBJECT_ID(N'");
                    state.Append(tableName);
                    state.Append("',N'U') IS NULL ");
                    state.Append(" BEGIN; ");
                }

                state.Append("CREATE TABLE ");
                state.Append(tableName);
                if (createStatement.AsFiletable)
                {
                    state.Append(" AS FileTable");
                }
            }


            var separator = " (";
            foreach (var column in createStatement.Columns)
            {
                state.Append(separator);
                separator = ", ";

                state.Append("[");
                state.Append(column.Name);
                state.Append("]");

                VisitType(column, state);

                if (column.Sparse)
                {
                    state.Append(" SPARSE");
                }
                if (column.Null.HasValue)
                {
                    state.Append(column.Null.Value ? " NULL" : " NOT NULL");
                }
                if (column.Identity.On)
                {
                    state.Append(Sym._IDENTITY_);
                    state.Append(Sym.op);
                    state.Append(column.Identity.Seed);
                    state.Append(Sym.COMMA_);
                    state.Append(column.Identity.Increment);
                    state.Append(Sym.cp);
                }
                if (column.RowGuid)
                {
                    state.Append(" ROWGUIDCOL");
                }
                if (column.DefaultValue != null)
                {
                    state.Append(" DEFAULT (");
                    VisitToken(column.DefaultValue, state, false);
                    state.Append(" )");
                }
            }

            if (createStatement.PrimaryKey != null)
            {
                if (createStatement.IsTableVariable)
                {
                    state.Append(",");
                }
                else
                {
                    state.Append(", CONSTRAINT ");
                    state.Append(ResolveName(createStatement.PrimaryKey.Name));
                }

                state.Append(" PRIMARY KEY (");
                state.Append(string.Join(", ",
                    createStatement.PrimaryKey.Columns.Select(
                        n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? " ASC" : " DESC"))));
                state.Append(" )");
            }

            foreach (var unique in createStatement.UniqueConstrains)
            {
                if (createStatement.IsTableVariable)
                {
                    state.Append(",");
                }
                else
                {
                    state.Append(", CONSTRAINT ");
                    state.Append(ResolveName(unique.Name));
                }
                state.Append(" UNIQUE");
                if (unique.Clustered.HasValue)
                {
                    state.Append(unique.Clustered.Value ? " CLUSTERED (" : " NONCLUSTERED (");
                }
                state.Append(string.Join(", ",
                    unique.Columns.Select(n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? " ASC" : " DESC"))));
                state.Append(" )");
            }


            state.Append(");");

            // if indecies are set, create them
            if (createStatement.Indicies.Count > 0 && !createStatement.IsTableVariable)
            {
                foreach (var createIndexStatement in createStatement.Indicies)
                {
                    VisitCreateIndexStatement(createIndexStatement, state);
                }
            }


            if (createStatement.CheckIfNotExists && !createStatement.IsTableVariable)
            {
                state.Append(" END;");
            }
        }
        private void VisitCommentStatement(CommentStatement statement, VisitorState state)
        {
            state.Append(" /* ");
            VisitStatement(statement.Content, state);
            state.Append(" */ ");
        }
        private void VisitStringifyStatement(StringifyStatement statement, VisitorState state)
        {
            this.Stringify(statement.Content, state);
        }
        private void VisitExecuteStatement(ExecuteStatement statement, VisitorState state)
        {
            state.Append("EXEC (");
            this.Stringify(statement.Target, state);
            state.Append(");");
        }
        private void VisitSnippetStatement(SnippetStatement statement, VisitorState state)
        {
            state.Append(statement.Value);
        }
        private void VisitCreateIndexStatement(CreateIndexStatement createIndexStatement, VisitorState state)
        {
            state.Append("CREATE");

            if (createIndexStatement.Unique)
            {
                state.Append(" UNIQUE");
            }

            if (createIndexStatement.Clustered.HasValue)
            {
                state.Append(createIndexStatement.Clustered.Value ? " CLUSTERED" : " NONCLUSTERED");
            }
            state.Append(" INDEX ");

            VisitToken(createIndexStatement.Name, state, false);

            state.Append(" ON ");

            VisitToken(createIndexStatement.On, state, false);

            // columns
            state.Append(" (");
            state.Append(string.Join(", ",
                createIndexStatement.Columns.Select(
                    n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? " ASC" : " DESC"))));
            state.Append(")");

            VisitTokenSet(createIndexStatement.Include, state, " INCLUDE (", ", ", ")", false);

            VisitWhere(createIndexStatement.Where, state);

            if (createIndexStatement.With.IsDefined)
            {
                state.Append(" WITH (");

                VisitWith(createIndexStatement.With.PadIndex, "PAD_INDEX", state);

                VisitWith(createIndexStatement.With.Fillfactor, "FILLFACTOR", state);
                VisitWith(createIndexStatement.With.SortInTempdb, "SORT_IN_TEMPDB", state);
                VisitWith(createIndexStatement.With.IgnoreDupKey, "IGNORE_DUP_KEY", state);
                VisitWith(createIndexStatement.With.StatisticsNorecompute, "STATISTICS_NORECOMPUTE", state);
                VisitWith(createIndexStatement.With.DropExisting, "DROP_EXISTING", state);
                VisitWith(createIndexStatement.With.Online, "ONLINE", state);
                VisitWith(createIndexStatement.With.AllowRowLocks, "ALLOW_ROW_LOCKS", state);
                VisitWith(createIndexStatement.With.AllowPageLocks, "ALLOW_PAGE_LOCKS", state);
                VisitWith(createIndexStatement.With.MaxDegreeOfParallelism, "MAXDOP", state);

                state.Append(" )");
            }

            state.Append(";");
        }
        private void VisitAlterIndexStatement(AlterIndexStatement alterStatement, VisitorState state)
        {
            state.Append("ALTER INDEX ");

            if (alterStatement.Name == null)
            {
                state.Append("ALL");
            }
            else
            {
                VisitToken(alterStatement.Name, state, false);
            }

            state.Append(" ON ");

            VisitToken(alterStatement.On, state, false);

            if (alterStatement.Rebuild)
            {
                state.Append(" REBUILD");

                //TODO: [PARTITION = ALL]
                if (alterStatement.RebuildWith.IsDefined)
                {
                    state.Append(" WITH (");

                    VisitWith(alterStatement.RebuildWith.PadIndex, "PAD_INDEX", state);
                    VisitWith(alterStatement.RebuildWith.Fillfactor, "FILLFACTOR", state);
                    VisitWith(alterStatement.RebuildWith.SortInTempdb, "SORT_IN_TEMPDB", state);
                    VisitWith(alterStatement.RebuildWith.IgnoreDupKey, "IGNORE_DUP_KEY", state);
                    VisitWith(alterStatement.RebuildWith.StatisticsNorecompute, "STATISTICS_NORECOMPUTE", state);
                    VisitWith(alterStatement.RebuildWith.DropExisting, "DROP_EXISTING", state);
                    VisitWith(alterStatement.RebuildWith.Online, "ONLINE", state);
                    VisitWith(alterStatement.RebuildWith.AllowRowLocks, "ALLOW_ROW_LOCKS", state);
                    VisitWith(alterStatement.RebuildWith.AllowPageLocks, "ALLOW_PAGE_LOCKS", state);
                    VisitWith(alterStatement.RebuildWith.MaxDegreeOfParallelism, "MAXDOP", state);

                    state.Append(" )");
                }
            }
            else if (alterStatement.Disable)
            {
                state.Append(" DISABLE");
            }
            else if (alterStatement.Reorganize)
            {
                state.Append(" REORGANIZE");
            }
            else
            {
                VisitWith(alterStatement.Set.AllowRowLocks, "ALLOW_ROW_LOCKS", state);
                VisitWith(alterStatement.Set.AllowPageLocks, "ALLOW_PAGE_LOCKS", state);
                VisitWith(alterStatement.Set.IgnoreDupKey, "IGNORE_DUP_KEY", state);
                VisitWith(alterStatement.Set.StatisticsNorecompute, "STATISTICS_NORECOMPUTE", state);
            }
        }
        private void VisitDropIndexStatement(DropIndexStatement dropIndexStatement, VisitorState state)
        {
            state.Append(Sym.DROP_INDEX_);
            VisitToken(dropIndexStatement.Name, state);

            state.Append(Sym._ON_);

            VisitToken(dropIndexStatement.On, state);

            if (dropIndexStatement.With.IsDefined)
            {
                state.Append(Sym._WITH);
                state.Append(Sym._op);

                if (dropIndexStatement.With.Online.HasValue)
                {
                    state.Append(Sym._ONLINE);
                    state.Append(AssignVal);
                    state.Append(dropIndexStatement.With.Online.Value ? Sym.ON : Sym.OFF);
                }
                if (dropIndexStatement.With.MaxDegreeOfParallelism.HasValue)
                {
                    state.Append(Sym._MAXDOP);
                    state.Append(AssignVal);
                    state.Append(dropIndexStatement.With.MaxDegreeOfParallelism.Value);
                }

                state.Append(Sym._cp);
            }
        }
        private void VisitCreateViewStatement(CreateViewStatement createStatement, VisitorState state)
        {
            var viewName = ResolveName(createStatement.Name);

            if (createStatement.CheckIfNotExists)
            {
                state.Append("IF OBJECT_ID(N'");
                state.Append(viewName);
                state.Append("') IS NULL EXEC (");

                Stringify(s =>
                {
                    s.Buffer.Append(Sym.CREATE_VIEW_);
                    s.Buffer.Append(viewName);
                    s.Buffer.Append(Sym._AS_);
                    VisitStatement(createStatement.DefinitionQuery, state);
                }, state);
                state.Append(Sym.cpsc);
            }
            else
            {
                state.Append(Sym.CREATE_VIEW_);
                state.Append(viewName);
                state.Append(Sym._AS_);
                VisitStatement(createStatement.DefinitionQuery, state);
            }
        }
        private void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement createStatement, VisitorState state)
        {
            var viewName = ResolveName(createStatement.Name);

            state.Append("IF OBJECT_ID(N'");
            state.Append(viewName);
            state.Append("') IS NULL EXEC (");

            Stringify(s =>
            {
                s.Buffer.Append(Sym.CREATE_VIEW_);
                s.Buffer.Append(viewName);
                s.Buffer.Append(Sym._AS_);
                VisitStatement(createStatement.DefinitionQuery, s);
            }, state);

            state.Append("); ELSE EXEC (");

            Stringify(s =>
            {
                s.Buffer.Append(Sym.ALTER_VIEW_);
                s.Buffer.Append(viewName);
                s.Buffer.Append(Sym._AS_);
                VisitStatement(createStatement.DefinitionQuery, s);
            }, state);

            state.Append(Sym.cpsc);
        }

        private void VisitDropViewStatement(DropViewStatement statement, VisitorState state)
        {
            var viewName = ResolveName(statement.Name);

            if (statement.CheckExists)
            {
                state.Append("IF OBJECT_ID(N'");
                state.Append(viewName);
                state.Append("') IS NOT NULL EXEC (");

                Stringify(s =>
                {
                    s.Buffer.Append(Sym.DROP_VIEW_);
                    s.Buffer.Append(viewName);
                    s.Buffer.Append(Sym.sc);
                }, state);

                state.Append(Sym.cpsc);
            }
            else
            {
                state.Append(Sym.DROP_VIEW_);
                state.Append(ResolveName(statement.Name));
            }
        }

        private void VisitAlterViewStatement(AlterViewStatement statement, VisitorState state)
        {
            state.Append(Sym.ALTER_VIEW_);
            state.Append(ResolveName(statement.Name));
            state.Append(Sym._AS_);
            VisitStatement(statement.DefinitionStatement, state);
        }


        #endregion Statements

        #region Select Parts

        private void VisitJoin(ICollection<Join> list, VisitorState state)
        {
            if (list.Count > 0)
            {
                foreach (var join in list)
                {
                    state.Append(JoinStrings[(int)join.Type]);
                    VisitToken(@join.Source, state, true);

                    if (join.On != null)
                    {
                        state.Append(Sym._ON_);
                        VisitToken(@join.On, state);
                    }
                }
            }
        }

        private void VisitOrderBy(ICollection<Order> orderBy, VisitorState state)
        {
            if (orderBy.Count > 0)
            {
                state.Append(Sym._ORDER_BY_);
                state.Append(string.Join(Sym.COMMA_,
                    orderBy.Select(n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym._ASC : Sym._DESC))));
            }
        }

        private void VisitGroupBy(ICollection<Name> groupBy, VisitorState state)
        {
            if (groupBy.Count > 0)
            {
                state.Append(Sym._GROUP_BY_);
                state.Append(string.Join(Sym.COMMA_, groupBy.Select(n => ResolveName(n))));
            }
        }

        private void VisitFrom(IEnumerable<Token> recordsets, VisitorState state)
        {
            VisitTokenSet(recordsets, state, Sym._FROM_, Sym.COMMA_, String.Empty, true);
        }

        private void VisitFrom(Token recordset, VisitorState state)
        {
            if (recordset != null)
            {
                state.Append(Sym._FROM_);
                VisitToken(recordset, state, true);
            }
        }

        private void VisitOutput(IEnumerable<Token> columns, Name outputInto, VisitorState state)
        {
            VisitTokenSet(columns, state, Sym._OUTPUT_, Sym.COMMA_, outputInto == null ? String.Empty : Sym._INTO_ + ResolveName(outputInto), true);
        }

        private void VisitTop(Top top, VisitorState state)
        {
            if (top != null)
            {
                state.Append(Sym._TOP_op);
                if (top.Value.HasValue)
                {
                    state.Append(top.Value.Value);
                }
                else if (top.Parameters.Count > 0)
                {
                    foreach (var parameter in top.Parameters)
                    {
                        state.Parameters.Add(parameter);
                    }
                    state.Append(top.Parameters[0].Name);
                }
                state.Append(")");

                if (top.Percent)
                {
                    state.Append(Sym._PERCENT);
                }
                if (top.WithTies)
                {
                    state.Append(Sym._WITH_TIES);
                }
            }
        }

        private void VisitWhere(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Append(Sym._WHERE_);
                VisitToken(whereToken, state);
            }
        }

        private void VisitHaving(Token whereToken, VisitorState state)
        {
            if (whereToken != null)
            {
                state.Append(Sym._HAVING_);
                VisitToken(whereToken, state);
            }
        }

        private void VisitWith(bool? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Append(Sym.SPACE);
                state.Append(name);
                state.Append(AssignVal);
                state.Append(value.Value ? Sym.ON : Sym.OFF);
            }
        }

        private void VisitWith(int? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Append(Sym.SPACE);
                state.Append(name);
                state.Append(AssignVal);
                state.Append(value.Value);
            }
        }

        #endregion Select Parts

        #region Tokens

        private void VisitToken(Token token, VisitorState state, bool includeAlias = false)
        {
            // todo check for statement
            TokenVisitors[token.GetType()](this, token, state);

            if (includeAlias)
            {
                VisitAlias(token, state);
            }

            state.Parameters.AddRange(token.Parameters);
            state.ParameterValues.AddRange(token.ParameterValues);
        }

        private void VisitAlias(Token token, VisitorState state, string open = Sym._AS_osp)
        {
            if (!string.IsNullOrWhiteSpace(token.Alias))
            {
                state.Append(open);
                state.Append(token.Alias);
                state.Append(Sym.csp);
            }
        }

        private void VisitScalarToken(Scalar token, VisitorState state)
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
                state.Append("N'");
                state.Append(((TimeSpan)value).ToString("HH:mm:ss"));
                state.Append("'");
            }
            else if (value is DateTime)
            {
                state.Append("N'");
                state.Append(((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss"));
                state.Append("'");
            }
            else if (value is DateTimeOffset)
            {
                state.Append("N'");
                state.Append(((DateTimeOffset)value).ToString("yyyy-MM-ddTHH:mm:ss"));
                state.Append("'");
            }
            else
            {
                state.Append("N'" + value + "'");
            }
        }

        private void VisitStatementToken(IStatement token, VisitorState state)
        {
            state.Append(Sym.op);
            VisitStatement(token, state);
            state.Append(Sym.cp);
        }

        private void VisitNameToken(Name token, VisitorState state)
        {
            state.Append(ResolveName(token));
        }

        private void VisitParameterToken(Parameter token, VisitorState state)
        {
            state.Append(token.Name);
        }

        private void VisitSnippetToken(Snippet token, VisitorState state)
        {
            state.Append(token.Value);
        }

        private void VisitFunctionToken(Function token, VisitorState state)
        {
            state.Append(Sym.SPACE);
            state.Append(token.Name);

            VisitTokenSet(token.Arguments, state, Sym.op, Sym.COMMA_, Sym.cp);
        }

        private void VisitBinaryToken(BinaryToken token, VisitorState state, string operation)
        {
            VisitToken(token.First, state, false);
            state.Append(operation);
            VisitToken(token.Second, state, false);
        }

        private void VisitIsEqualsToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, EqualsVal);
        }

        private void VisitNotEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, NotEqualVal);
        }

        private void VisitLessToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, LessVal);
        }

        private void VisitNotLessToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, NotLessVal);
        }

        private void VisitLessOrEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, LessOrEqualVal);
        }

        private void VisitGreaterToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, GreaterVal);
        }

        private void VisitNotGreaterToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, NotGreaterVal);
        }

        private void VisitGreaterOrEqualToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, GreaterOrEqualVal);
        }

        private void VisitAllToken(AllToken token, VisitorState state)
        {
            state.Append(Sym._ALL_);
            VisitToken(token.Token, state);
        }

        private void VisitAnyToken(AnyToken token, VisitorState state)
        {
            state.Append(Sym._ANY_);
            VisitToken(token.Token, state);
        }

        private void VisitAndToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, AndVal);
        }

        private void VisitOrToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, OrVal);
        }

        private void VisitPlusToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? PlusEqVal : PlusVal);
        }

        private void VisitMinusToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? MinusEqVal : MinusVal);
        }

        private void VisitDivideToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? DivideEqVal : DivideVal);
        }

        private void VisitModuloToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? ModuloEqVal : ModuloVal);
        }

        private void VisitMultiplyToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? MultiplyEqVal : MultiplyVal);
        }

        private void VisitBitwiseAndToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? BitwiseAndEqVal : BitwiseAndVal);
        }

        private void VisitBitwiseOrToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? BitwiseOrEqVal : BitwiseOrVal);
        }

        private void VisitBitwiseXorToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? BitwiseXorEqVal : BitwiseXorVal);
        }

        private void VisitBitwiseNotToken(BinaryEqualToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, token.Equal ? BitwiseNotEqVal : BitwiseNotVal);
        }

        private void VisitAssignToken(BinaryToken token, VisitorState state)
        {
            VisitBinaryToken(token, state, AssignVal);
        }

        private void VisitLikeToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state);
            state.Append(Sym._LIKE_);
            VisitToken(token.Second, state);
        }

        private void VisitContainsToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state, false);
            state.Append(" LIKE '%' + ");
            VisitToken(token.Second, state, false);
            state.Append(" + '%'");
        }

        private void VisitStartsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state, false);
            state.Append(Sym._LIKE_);
            VisitToken(token.Second, state, false);
            state.Append(" + '%'");
        }

        private void VisitEndsWithToken(BinaryToken token, VisitorState state)
        {
            VisitToken(token.First, state, false);
            state.Append(" LIKE '%' + ");
            VisitToken(token.Second, state, false);
        }

        private void VisitGroupToken(GroupToken token, VisitorState state)
        {
            state.Append(Sym._op);
            VisitToken(token.Token, state, false);
            state.Append(Sym._cp);
        }

        private void VisitExistsToken(ExistsToken token, VisitorState state)
        {
            state.Append(Sym._EXISTS_);
            VisitToken(token.Token, state);
        }

        private void VisitNotToken(NotToken token, VisitorState state)
        {
            state.Append(Sym._NOT_op);
            VisitToken(token.Token, state);
            state.Append(Sym._cp);
        }

        private void VisitIsNullToken(IsNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Append(Sym._IS_NULL);
        }

        private void VisitIsNotNullToken(IsNotNullToken token, VisitorState state)
        {
            VisitToken(token.Token, state, false);
            state.Append(Sym._IS_NOT_NULL);
        }

        private void VisitBetweenToken(BetweenToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            state.Append(Sym._BETWEEN_);
            VisitToken(token.First, state);
            state.Append(Sym._AND_);
            VisitToken(token.Second, state);
        }

        private void VisitInToken(InToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            VisitTokenSet(token.Set, state, Sym._IN_op, Sym.COMMA_, Sym.cp);
        }

        private void VisitNotInToken(NotInToken token, VisitorState state)
        {
            VisitToken(token.Token, state);
            VisitTokenSet(token.Set, state, Sym._NOT_IN_op, Sym.COMMA_, Sym.cp);
        }

        private void VisitTokenSet(IEnumerable<Token> tokens, VisitorState state, string prefix, string separator, string suffix, bool includeAlias = false)
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

        private void VisitCommentToken(CommentToken token, VisitorState state)
        {
            state.Append(" /* ");

            VisitToken(token.Content, state);

            state.Append(" */ ");
        }

        private void VisitStringifyToken(StringifyToken token, VisitorState state)
        {
            Stringify(s => VisitToken(token.Content, s), state);
        }

        #endregion Tokens
    }
}