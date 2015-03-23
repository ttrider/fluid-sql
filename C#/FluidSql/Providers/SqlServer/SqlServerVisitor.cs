using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TTRider.FluidSql.Providers.SqlServer
{
    internal class SqlServerVisitor : Visitor
    {
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


        


        private static readonly Dictionary<Type, Action<SqlServerVisitor, Token, VisitorState>> TokenVisitors =
            new Dictionary<Type, Action<SqlServerVisitor, Token, VisitorState>>
            {
                {typeof (SelectStatement), (v,t,s)=>v.VisitStatementToken((IStatement)t, s)},
                {typeof (Union),(v,t,s)=>v.VisitStatementToken((IStatement)t,s)},
                {typeof (Intersect),(v,t,s)=>v.VisitStatementToken((IStatement)t,s)},
                {typeof (Except),(v,t,s)=>v.VisitStatementToken((IStatement)t,s)},
                {typeof (Scalar),(v,t,s)=>v.VisitScalarToken((Scalar)t,s,"N'","'")},
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

        protected override string CloseQuote
        {
            get { return "]"; }
        }
        protected override string OpenQuote
        {
            get { return "["; }
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

        protected override void VisitStatement(IStatement statement, VisitorState state)
        {
            StatementVisitors[statement.GetType()](this, statement, state);
        }
        protected override void VisitJoinType(Joins join, VisitorState state)
        {
            state.Append(JoinStrings[(int)join]);
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
                    VisitToken(when.AndCondition, state);
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
                    VisitToken(when.AndCondition, state);
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
                    VisitToken(when.AndCondition, state);
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
            state.Append(Sym.SELECT);

            if (statement.Distinct)
            {
                state.Append(Sym._DISTINCT);
            }

            if (statement.Offset == null)
            {
                VisitTop(statement.Top, state);
            }

            // assignments
            if (statement.Set.Count > 0)
            {
                VisitTokenSet(statement.Set, state, Sym.SPACE, Sym.COMMA_, String.Empty);
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
                    VisitTokenSet(statement.Output, state, Sym.SPACE, Sym.COMMA_, String.Empty, true);
                }
            }

            VisitInto(statement.Into, state);

            VisitFrom(statement.From, state);

            VisitJoin(statement.Joins, state);

            VisitWhere(statement.Where, state);

            VisitGroupBy(statement.GroupBy, state);

            VisitHaving(statement.Having, state);

            VisitOrderBy(statement.OrderBy, state);

            if (statement.Offset != null)
            {
                state.Append(Sym._OFFSET_);
                VisitToken(statement.Offset, state);
                state.Append(Sym._ROWS);
                state.Append(Sym._FETCH_NEXT_);
                if (statement.Top.Value.HasValue)
                {
                    state.Append(statement.Top.Value.Value);
                }
                else if (statement.Top.Parameters.Count > 0)
                {
                    foreach (var parameter in statement.Top.Parameters)
                    {
                        state.Parameters.Add(parameter);
                    }
                    state.Append(statement.Top.Parameters[0].Name);
                }
                state.Append(Sym._ROWS_ONLY);
            }

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
                VisitAlias(statement.From, state, " ");

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
                VisitToken(statement.ReturnExpression, state);
            }

        }
        private void VisitThrowStatement(ThrowStatement statement, VisitorState state)
        {
            state.Append("THROW");
            if (statement.ErrorNumber != null && statement.Message != null && statement.State != null)
            {
                state.Append(" ");
                VisitToken(statement.ErrorNumber, state);
                state.Append(", ");
                VisitToken(statement.Message, state);
                state.Append(", ");
                VisitToken(statement.State, state);
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
        private void VisitCreateIndexStatement(CreateIndexStatement createIndexStatement, VisitorState state)
        {
            state.Append(Sym.CREATE);

            if (createIndexStatement.Unique)
            {
                state.Append(Sym._UNIQUE);
            }

            if (createIndexStatement.Clustered.HasValue)
            {
                state.Append(createIndexStatement.Clustered.Value ? Sym._CLUSTERED : Sym._NONCLUSTERED);
            }
            state.Append(Sym._INDEX_);

            VisitToken(createIndexStatement.Name, state);

            state.Append(Sym._ON_);

            VisitToken(createIndexStatement.On, state);

            // columns
            state.Append(Sym._op);
            state.Append(string.Join(Sym.COMMA_,
                createIndexStatement.Columns.Select(
                    n => ResolveName(n.Column) + (n.Direction == Direction.Asc ? Sym._ASC : Sym._DESC))));
            state.Append(Sym.cp);

            VisitTokenSet(createIndexStatement.Include, state, Sym._INCLUDE_op, Sym.COMMA_, Sym.cp);

            VisitWhere(createIndexStatement.Where, state);

            if (createIndexStatement.With.IsDefined)
            {
                state.Append(Sym._WITH);
                state.Append(Sym._op);

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

                state.Append(Sym._cp);
            }

            state.Append(Sym.sc);
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
                    state.Append(Sym.AssignVal);
                    state.Append(dropIndexStatement.With.Online.Value ? Sym.ON : Sym.OFF);
                }
                if (dropIndexStatement.With.MaxDegreeOfParallelism.HasValue)
                {
                    state.Append(Sym._MAXDOP);
                    state.Append(Sym.AssignVal);
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
                    VisitStatement(createStatement.DefinitionStatement, state);
                }, state);
                state.Append(Sym.cpsc);
            }
            else
            {
                state.Append(Sym.CREATE_VIEW_);
                state.Append(viewName);
                state.Append(Sym._AS_);
                VisitStatement(createStatement.DefinitionStatement, state);
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
                VisitStatement(createStatement.DefinitionStatement, s);
            }, state);

            state.Append("); ELSE EXEC (");

            Stringify(s =>
            {
                s.Buffer.Append(Sym.ALTER_VIEW_);
                s.Buffer.Append(viewName);
                s.Buffer.Append(Sym._AS_);
                VisitStatement(createStatement.DefinitionStatement, s);
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


        

        private void VisitWith(bool? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Append(Sym.SPACE);
                state.Append(name);
                state.Append(Sym.AssignVal);
                state.Append(value.Value ? Sym.ON : Sym.OFF);
            }
        }

        private void VisitWith(int? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Append(Sym.SPACE);
                state.Append(name);
                state.Append(Sym.AssignVal);
                state.Append(value.Value);
            }
        }

        #endregion Select Parts

        #region Tokens

        protected override void VisitToken(Token token, VisitorState state, bool includeAlias = false)
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

        private void VisitStringifyToken(StringifyToken token, VisitorState state)
        {
            Stringify(s => VisitToken(token.Content, s), state);
        }

        #endregion Tokens
    }
}
