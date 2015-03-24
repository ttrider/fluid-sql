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
                {typeof (Order),(v,t,s)=>v.VisitOrderToken((Order)t,s)},

                
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
            state.WriteStatementTerminator();

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
            state.WriteBeginStringify("N'", "'");
            fragment(state);
            state.WriteEndStringify();
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
                state.Write(DbTypeStrings[(int)typedToken.DbType]);
            }

            if (typedToken.Length.HasValue || typedToken.Precision.HasValue || typedToken.Scale.HasValue)
            {
                state.Write(Sym.op);
                if (typedToken.Length.HasValue)
                {
                    state.Write(typedToken.Length.Value == -1 ? Sym.MAX : typedToken.Length.Value.ToString(CultureInfo.InvariantCulture));
                }
                else if (typedToken.Precision.HasValue)
                {
                    state.Write(typedToken.Precision.Value.ToString());

                    if (typedToken.Scale.HasValue)
                    {
                        state.Write(Sym.COMMA);
                        state.Write(typedToken.Scale.Value.ToString());
                    }
                }

                state.Write(")");
            }
        }


        #region Statements

        protected override void VisitStatement(IStatement statement, VisitorState state)
        {
            StatementVisitors[statement.GetType()](this, statement, state);
        }
        protected override void VisitJoinType(Joins join, VisitorState state)
        {
            state.Write(JoinStrings[(int)join]);
        }

        private void VisitSet(SetStatement statement, VisitorState state)
        {
            state.Write(Sym.SET);

            if (statement.Assign != null)
            {
                VisitToken(statement.Assign, state, false);
            }
        }
        private void VisitMerge(MergeStatement statement, VisitorState state)
        {
            state.Write(Sym.MERGE);

            VisitTop(statement.Top, state);

            VisitInto(statement.Into, state);

            if (!string.IsNullOrWhiteSpace(statement.Into.Alias))
            {
                state.Write(Sym.AS);
                state.Write(this.OpenQuote, statement.Into.Alias, this.CloseQuote);
            }

            state.Write(Sym.USING);
            VisitToken(statement.Using, state);
            if (!string.IsNullOrWhiteSpace(statement.Using.Alias))
            {
                state.Write(Sym.AS);
                state.Write(this.OpenQuote, statement.Using.Alias, this.CloseQuote);
            }

            state.Write(Sym.ON);

            VisitToken(statement.On, state, false);

            foreach (var when in statement.WhenMatched)
            {
                state.Write(Sym.WHEN_MATCHED);
                if (when.AndCondition != null)
                {
                    state.Write(Sym.AND);
                    VisitToken(when.AndCondition, state);
                }
                state.Write(Sym.THEN);

                VisitToken(when, state, false);
            }

            foreach (var when in statement.WhenNotMatched)
            {
                state.Write(Sym.WHEN_NOT_MATCHED_BY_TARGET);
                if (when.AndCondition != null)
                {
                    state.Write(Sym.AND);
                    VisitToken(when.AndCondition, state);
                }
                state.Write(Sym.THEN);

                VisitToken(when, state, false);
            }

            foreach (var when in statement.WhenNotMatchedBySource)
            {
                state.Write(Sym.WHEN_NOT_MATCHED_BY_SOURCE);
                if (when.AndCondition != null)
                {
                    state.Write(Sym.AND);
                    VisitToken(when.AndCondition, state);
                }
                state.Write(Sym.THEN);

                VisitToken(when, state, false);
            }

            VisitOutput(statement.Output, statement.OutputInto, state);
        }

        private void VisitWhenMatchedThenDelete(WhenMatchedThenDelete token, VisitorState state)
        {
            state.Write(Sym.DELETE);
        }
        private void VisitWhenMatchedThenUpdateSet(WhenMatchedThenUpdateSet token, VisitorState state)
        {
            state.Write(Sym.UPDATE_SET);
            VisitTokenSet(token.Set, state, null, Sym.COMMA, null);
        }
        private void VisitWhenNotMatchedThenInsert(WhenNotMatchedThenInsert token, VisitorState state)
        {
            state.Write(Sym.INSERT);
            if (token.Columns.Count > 0)
            {
                VisitTokenSet(token.Columns, state, Sym.op, Sym.COMMA, Sym.cp);
            }
            if (token.Values.Count > 0)
            {
                VisitTokenSet(token.Values, state, Sym.VALUES_op, Sym.COMMA, Sym.cp);
            }
            else
            {
                state.Write(Sym.DEFAULT_VALUES);
            }
        }

        private void VisitSelect(SelectStatement statement, VisitorState state)
        {
            state.Write(Sym.SELECT);

            if (statement.Distinct)
            {
                state.Write(Sym.DISTINCT);
            }

            if (statement.Offset == null)
            {
                VisitTop(statement.Top, state);
            }

            // assignments
            if (statement.Set.Count > 0)
            {
                VisitTokenSet(statement.Set, state, null, Sym.COMMA, null);
            }
            else
            {
                // output columns
                if (statement.Output.Count == 0)
                {
                    state.Write(Sym.asterisk);
                }
                else
                {
                    VisitTokenSet(statement.Output, state, null, Sym.COMMA, null, true);
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
                state.Write(Sym.OFFSET);
                VisitToken(statement.Offset, state);
                state.Write(Sym.ROWS);
                state.Write(Sym.FETCH_NEXT);
                if (statement.Top.Value.HasValue)
                {
                    state.Write(statement.Top.Value.Value.ToString());
                }
                else if (statement.Top.Parameters.Count > 0)
                {
                    foreach (var parameter in statement.Top.Parameters)
                    {
                        state.Parameters.Add(parameter);
                    }
                    state.Write(statement.Top.Parameters[0].Name);
                }
                state.Write(Sym.ROWS_ONLY);
            }

            //WITH CUBE or WITH ROLLUP
        }
        private void VisitInto(Name into, VisitorState state)
        {
            if (into != null)
            {
                state.Write(Sym.INTO);
                VisitNameToken(into, state);
            }
        }
        private void VisitDelete(DeleteStatement statement, VisitorState state)
        {
            state.Write(Sym.DELETE);

            VisitTop(statement.Top, state);

            if (statement.Joins.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(statement.From.Alias))
                {
                    state.Write(this.OpenQuote, statement.From.Alias, this.CloseQuote);
                }

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
            state.Write(Sym.UPDATE);

            VisitTop(updateStatement.Top, state);

            VisitToken(updateStatement.Target, state, true);

            state.Write(Sym.SET);
            VisitTokenSet(updateStatement.Set, state, null, Sym.COMMA, null);

            VisitOutput(updateStatement.Output, updateStatement.OutputInto, state);

            VisitFrom(updateStatement.From, state);

            VisitJoin(updateStatement.Joins, state);

            VisitWhere(updateStatement.Where, state);
        }
        private void VisitInsert(InsertStatement insertStatement, VisitorState state)
        {
            state.Write(Sym.INSERT);

            VisitTop(insertStatement.Top, state);

            VisitInto(insertStatement.Into, state);

            VisitTokenSet(insertStatement.Columns, state, Sym.op, Sym.COMMA, Sym.cp, true);

            VisitOutput(insertStatement.Output, insertStatement.OutputInto, state);

            if (insertStatement.DefaultValues)
            {
                state.Write(Sym.DEFAULT_VALUES);
            }
            else if (insertStatement.Values.Count > 0)
            {
                var separator = Sym.VALUES;
                foreach (var valuesSet in insertStatement.Values)
                {
                    state.Write(separator);
                    separator = Sym.COMMA;

                    VisitTokenSet(valuesSet, state, Sym.op, Sym.COMMA, Sym.cp);
                }
            }
            else if (insertStatement.From != null)
            {
                VisitStatement(insertStatement.From, state);
            }
        }
        
        private void VisitBeginTransaction(BeginTransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.BEGIN_TRANSACTION);
            if (VisitTransactionName(statement, state) && !string.IsNullOrWhiteSpace(statement.Description))
            {
                state.Write(Sym.WITH_MARK);
                state.Write("N'",statement.Description,"'");
            }
        }
        private void VisitCommitTransaction(TransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.COMMIT_TRANSACTION);
            VisitTransactionName(statement, state);
        }
        private void VisitRollbackTransaction(TransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.ROLLBACK_TRANSACTION);
            VisitTransactionName(statement, state);
        }
        private void VisitSaveTransaction(TransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.SAVE_TRANSACTION);
            VisitTransactionName(statement, state);
        }
       
        private void VisitDeclareStatement(DeclareStatement statement, VisitorState state)
        {
            if (statement.Variable != null)
            {
                state.Variables.Add(statement.Variable);

                state.Write(Sym.DECLARE);
                state.Write(statement.Variable.Name);

                VisitType(statement.Variable, state);

                if (statement.Initializer != null)
                {
                    state.Write(Sym.AssignVal);
                    VisitToken(statement.Initializer, state);
                }
            }
        }

        // ReSharper disable once UnusedParameter.Local
        private void VisitBreakStatement(BreakStatement statement, VisitorState state)
        {
            state.Write(Sym.BREAK);
        }
        private void VisitContinueStatement(ContinueStatement statement, VisitorState state)
        {
            state.Write(Sym.CONTINUE);
        }
        private void VisitGotoStatement(GotoStatement statement, VisitorState state)
        {
            state.Write(Sym.GOTO);
            state.Write(statement.Label);
        }
        private void VisitReturnStatement(ReturnStatement statement, VisitorState state)
        {
            state.Write(Sym.RETURN);
            if (statement.ReturnExpression != null)
            {
                VisitToken(statement.ReturnExpression, state);
            }

        }
        private void VisitThrowStatement(ThrowStatement statement, VisitorState state)
        {
            state.Write(Sym.THROW);
            if (statement.ErrorNumber != null && statement.Message != null && statement.State != null)
            {
                VisitToken(statement.ErrorNumber, state);
                state.Write(Sym.COMMA);
                VisitToken(statement.Message, state);
                state.Write(Sym.COMMA);
                VisitToken(statement.State, state);
            }
        }
        private void VisitTryCatchStatement(TryCatchStatement stmt, VisitorState state)
        {
            state.Write(Sym.BEGIN_TRY);
            state.WriteCRLF();
            VisitStatement(stmt.TryStatement, state);
            state.WriteStatementTerminator();
            state.Write(Sym.END_TRY);
            state.WriteCRLF();
            state.Write(Sym.BEGIN_CATCH);
            state.WriteCRLF();
            if (stmt.CatchStatement != null)
            {
                VisitStatement(stmt.CatchStatement, state);
                state.WriteStatementTerminator();
            }
            state.Write(Sym.END_CATCH);
            state.WriteStatementTerminator();
        }
        private void VisitLabelStatement(LabelStatement stmt, VisitorState state)
        {
            state.Write(stmt.Label);
            state.Write(":");
        }
        private void VisitWaitforDelayStatement(WaitforDelayStatement stmt, VisitorState state)
        {
            state.Write(Sym.WAITFOR_DELAY);
            state.Write("N'",stmt.Delay.ToString("HH:mm:ss"),"'");
        }
        private void VisitWaitforTimeStatement(WaitforTimeStatement stmt, VisitorState state)
        {
            state.Write(Sym.WAITFOR_TIME);
            state.Write("N'",stmt.Time.ToString("yyyy-MM-ddTHH:mm:ss"),"'");
        }
        private void VisitWhileStatement(WhileStatement stmt, VisitorState state)
        {
            if (stmt.Condition != null)
            {
                state.Write(Sym.WHILE);
                VisitToken(stmt.Condition, state, false);

                if (stmt.Do != null)
                {
                    state.WriteCRLF();
                    state.Write(Sym.BEGIN);
                    state.WriteStatementTerminator();
                    
                    VisitStatement(stmt.Do, state);
                    state.WriteStatementTerminator();
                    
                    state.Write(Sym.END);
                    state.WriteStatementTerminator();
                }
            }
        }
        private void VisitIfStatement(IfStatement ifs, VisitorState state)
        {
            if (ifs.Condition != null)
            {
                state.Write(Sym.IF);
                VisitToken(ifs.Condition, state, false);

                if (ifs.Then != null)
                {
                    state.WriteCRLF();
                    state.Write(Sym.BEGIN);
                    state.WriteStatementTerminator();

                    VisitStatement(ifs.Then, state);
                    state.WriteStatementTerminator();
                    
                    state.Write(Sym.END);
                    state.WriteStatementTerminator();

                    if (ifs.Else != null)
                    {
                        state.Write(Sym.ELSE);
                        state.WriteCRLF();
                        state.Write(Sym.BEGIN);
                        state.WriteStatementTerminator();

                        VisitStatement(ifs.Else, state);
                        state.WriteStatementTerminator();
                    
                        state.Write(Sym.END);
                        state.WriteStatementTerminator();
                    }
                }
            }
        }
        private void VisitDropTableStatement(DropTableStatement statement, VisitorState state)
        {
            var tableName = ResolveName((statement.IsTemporary) ? GetTempTableName(statement.Name) : statement.Name);

            if (statement.CheckExists)
            {
                state.Write("IF OBJECT_ID ( N'", tableName, "', N'U' ) IS NOT NULL");
            }

            state.Write(Sym.DROP_TABLE);
            state.Write(tableName);
        }
        private void VisitCreateTableStatement(CreateTableStatement createStatement, VisitorState state)
        {
            if (createStatement.IsTableVariable)
            {
                state.Write(Sym.DECLARE);
                state.Write(GetTableVariableName(createStatement.Name));
                state.Write(Sym.TABLE);
            }
            else
            {
                var tableName =
                    ResolveName((createStatement.IsTemporary) ? GetTempTableName(createStatement.Name) : createStatement.Name);

                if (createStatement.CheckIfNotExists)
                {
                    state.Write("IF OBJECT_ID ( N'", tableName, "', N'U' ) IS NULL");
                    state.WriteCRLF(); 
                    state.Write(Sym.BEGIN);
                    state.WriteStatementTerminator();
                }

                state.Write(Sym.CREATE_TABLE);
                state.Write(tableName);
                if (createStatement.AsFiletable)
                {
                    state.Write(Sym.AS);
                    state.Write(Sym.FILETABLE);
                }
            }


            var separator = Sym.op;
            foreach (var column in createStatement.Columns)
            {
                state.Write(separator);
                separator = Sym.COMMA;

                state.Write(this.OpenQuote,column.Name,this.CloseQuote);

                VisitType(column, state);

                if (column.Sparse)
                {
                    state.Write(Sym.SPARSE);
                }
                if (column.Null.HasValue)
                {
                    state.Write(column.Null.Value ? Sym.NULL : Sym.NOT_NULL);
                }
                if (column.Identity.On)
                {
                    state.Write(Sym.IDENTITY);
                    state.Write(Sym.op);
                    state.Write(column.Identity.Seed.ToString());
                    state.Write(Sym.COMMA);
                    state.Write(column.Identity.Increment.ToString());
                    state.Write(Sym.cp);
                }
                if (column.RowGuid)
                {
                    state.Write(Sym.ROWGUIDCOL);
                }
                if (column.DefaultValue != null)
                {
                    state.Write(Sym.DEFAULT);
                    state.Write(Sym.op);
                    VisitToken(column.DefaultValue, state, false);
                    state.Write(Sym.cp);
                }
            }

            if (createStatement.PrimaryKey != null)
            {
                state.Write(Sym.COMMA);
                if (!createStatement.IsTableVariable)
                {
                    state.Write(Sym.CONSTRAINT);
                    VisitNameToken(createStatement.PrimaryKey.Name, state);
                }

                state.Write(Sym.PRIMARY_KEY);
                VisitTokenSet(createStatement.PrimaryKey.Columns, state, Sym.op, Sym.COMMA, Sym.cp);
            }

            foreach (var unique in createStatement.UniqueConstrains)
            {
                state.Write(Sym.COMMA);
                if (!createStatement.IsTableVariable)
                {
                    state.Write(Sym.CONSTRAINT);
                    VisitNameToken(unique.Name, state);
                }

                state.Write(Sym.UNIQUE);
                if (unique.Clustered.HasValue)
                {
                    state.Write(unique.Clustered.Value ? Sym.CLUSTERED:Sym.NONCLUSTERED);
                }
                VisitTokenSet(unique.Columns, state, Sym.op, Sym.COMMA, Sym.cp);
            }
                
            state.Write(Sym.cp);
            state.WriteStatementTerminator();

            // if indecies are set, create them
            if (createStatement.Indicies.Count > 0 && !createStatement.IsTableVariable)
            {

                foreach (var createIndexStatement in createStatement.Indicies)
                {
                    VisitCreateIndexStatement(createIndexStatement, state);
                    state.WriteStatementTerminator();
                }
            }

            if (createStatement.CheckIfNotExists && !createStatement.IsTableVariable)
            {
                state.Write(Sym.END);
            }
        }
        
        private void VisitStringifyStatement(StringifyStatement statement, VisitorState state)
        {
            this.Stringify(statement.Content, state);
        }
        private void VisitExecuteStatement(ExecuteStatement statement, VisitorState state)
        {
            state.Write(Sym.EXEC);
            state.Write(Sym.op);
            this.Stringify(statement.Target, state);
            state.Write(Sym.cp);
        }
        private void VisitCreateIndexStatement(CreateIndexStatement createIndexStatement, VisitorState state)
        {
            state.Write(Sym.CREATE);

            if (createIndexStatement.Unique)
            {
                state.Write(Sym.UNIQUE);
            }

            if (createIndexStatement.Clustered.HasValue)
            {
                state.Write(createIndexStatement.Clustered.Value ? Sym.CLUSTERED : Sym.NONCLUSTERED);
            }
            state.Write(Sym.INDEX);

            VisitToken(createIndexStatement.Name, state);

            state.Write(Sym.ON);

            VisitToken(createIndexStatement.On, state);

            // columns
            VisitTokenSet(createIndexStatement.Columns, state, Sym.op, Sym.COMMA, Sym.cp);

            VisitTokenSet(createIndexStatement.Include, state, Sym.INCLUDE_op, Sym.COMMA, Sym.cp);

            VisitWhere(createIndexStatement.Where, state);

            if (createIndexStatement.With.IsDefined)
            {
                state.Write(Sym.WITH);
                state.Write(Sym.op);

                VisitWith(createIndexStatement.With.PadIndex, Sym.PAD_INDEX, state);
                VisitWith(createIndexStatement.With.Fillfactor, Sym.FILLFACTOR, state);
                VisitWith(createIndexStatement.With.SortInTempdb, Sym.SORT_IN_TEMPDB, state);
                VisitWith(createIndexStatement.With.IgnoreDupKey, Sym.IGNORE_DUP_KEY, state);
                VisitWith(createIndexStatement.With.StatisticsNorecompute, Sym.STATISTICS_NORECOMPUTE, state);
                VisitWith(createIndexStatement.With.DropExisting, Sym.DROP_EXISTING, state);
                VisitWith(createIndexStatement.With.Online, Sym.ONLINE, state);
                VisitWith(createIndexStatement.With.AllowRowLocks, Sym.ALLOW_ROW_LOCKS, state);
                VisitWith(createIndexStatement.With.AllowPageLocks, Sym.ALLOW_PAGE_LOCKS, state);
                VisitWith(createIndexStatement.With.MaxDegreeOfParallelism, Sym.MAXDOP, state);

                state.Write(Sym.cp);
            }

            state.Write(Sym.sc);
        }
        private void VisitAlterIndexStatement(AlterIndexStatement alterStatement, VisitorState state)
        {
            state.Write(Sym.ALTER_INDEX);

            if (alterStatement.Name == null)
            {
                state.Write(Sym.ALL);
            }
            else
            {
                VisitToken(alterStatement.Name, state, false);
            }

            state.Write(Sym.ON);

            VisitToken(alterStatement.On, state, false);

            if (alterStatement.Rebuild)
            {
                state.Write(Sym.REBUILD);

                //TODO: [PARTITION = ALL]
                if (alterStatement.RebuildWith.IsDefined)
                {
                    state.Write(Sym.WITH);
                    state.Write(Sym.op);

                    VisitWith(alterStatement.RebuildWith.PadIndex, Sym.PAD_INDEX, state);
                    VisitWith(alterStatement.RebuildWith.Fillfactor, Sym.FILLFACTOR, state);
                    VisitWith(alterStatement.RebuildWith.SortInTempdb, Sym.SORT_IN_TEMPDB, state);
                    VisitWith(alterStatement.RebuildWith.IgnoreDupKey, Sym.IGNORE_DUP_KEY, state);
                    VisitWith(alterStatement.RebuildWith.StatisticsNorecompute, Sym.STATISTICS_NORECOMPUTE, state);
                    VisitWith(alterStatement.RebuildWith.DropExisting, Sym.DROP_EXISTING, state);
                    VisitWith(alterStatement.RebuildWith.Online, Sym.ONLINE, state);
                    VisitWith(alterStatement.RebuildWith.AllowRowLocks, Sym.ALLOW_ROW_LOCKS, state);
                    VisitWith(alterStatement.RebuildWith.AllowPageLocks, Sym.ALLOW_PAGE_LOCKS, state);
                    VisitWith(alterStatement.RebuildWith.MaxDegreeOfParallelism, Sym.MAXDOP, state);

                    state.Write(Sym.cp);
                }
            }
            else if (alterStatement.Disable)
            {
                state.Write(Sym.DISABLE);
            }
            else if (alterStatement.Reorganize)
            {
                state.Write(Sym.REORGANIZE);
            }
            else
            {
                VisitWith(alterStatement.Set.AllowRowLocks, Sym.ALLOW_ROW_LOCKS, state);
                VisitWith(alterStatement.Set.AllowPageLocks, Sym.ALLOW_PAGE_LOCKS, state);
                VisitWith(alterStatement.Set.IgnoreDupKey, Sym.IGNORE_DUP_KEY, state);
                VisitWith(alterStatement.Set.StatisticsNorecompute, Sym.STATISTICS_NORECOMPUTE, state);
            }
        }
        private void VisitDropIndexStatement(DropIndexStatement dropIndexStatement, VisitorState state)
        {
            state.Write(Sym.DROP_INDEX);
            VisitToken(dropIndexStatement.Name, state);

            state.Write(Sym.ON);

            VisitToken(dropIndexStatement.On, state);

            if (dropIndexStatement.With.IsDefined)
            {
                state.Write(Sym.WITH);
                state.Write(Sym.op);

                if (dropIndexStatement.With.Online.HasValue)
                {
                    state.Write(Sym.ONLINE);
                    state.Write(Sym.AssignVal);
                    state.Write(dropIndexStatement.With.Online.Value ? Sym.ON : Sym.OFF);
                }
                if (dropIndexStatement.With.MaxDegreeOfParallelism.HasValue)
                {
                    state.Write(Sym.MAXDOP);
                    state.Write(Sym.AssignVal);
                    state.Write(dropIndexStatement.With.MaxDegreeOfParallelism.Value.ToString());
                }

                state.Write(Sym.cp);
            }
        }
        private void VisitCreateViewStatement(CreateViewStatement createStatement, VisitorState state)
        {
            var viewName = ResolveName(createStatement.Name);

            if (createStatement.CheckIfNotExists)
            {
                state.Write("IF OBJECT_ID ( N'", viewName, "' ) IS NULL EXEC ( ");

                Stringify(s =>
                {
                    s.Write(Sym.CREATE_VIEW);
                    s.Write(viewName);
                    s.Write(Sym.AS);
                    VisitStatement(createStatement.DefinitionStatement, state);
                }, state);
                state.Write(Sym.cp);
            }
            else
            {
                state.Write(Sym.CREATE_VIEW);
                state.Write(viewName);
                state.Write(Sym.AS);
                VisitStatement(createStatement.DefinitionStatement, state);
            }
        }
        private void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement createStatement, VisitorState state)
        {
            var viewName = ResolveName(createStatement.Name);

            state.Write("IF OBJECT_ID ( N'", viewName, "' ) IS NULL EXEC ( ");

            Stringify(s =>
            {
                s.Write(Sym.CREATE_VIEW);
                s.Write(viewName);
                s.Write(Sym.AS);
                VisitStatement(createStatement.DefinitionStatement, s);
            }, state);

            state.Write(" );\r\nELSE EXEC ( ");

            Stringify(s =>
            {
                s.Write(Sym.ALTER_VIEW);
                s.Write(viewName);
                s.Write(Sym.AS);
                VisitStatement(createStatement.DefinitionStatement, s);
            }, state);

            state.Write(Sym.cp);
        }

        private void VisitDropViewStatement(DropViewStatement statement, VisitorState state)
        {
            var viewName = ResolveName(statement.Name);

            if (statement.CheckExists)
            {
                state.Write("IF OBJECT_ID ( N'", viewName, "' ) IS NOT NULL EXEC ( ");

                Stringify(s =>
                {
                    s.Write(Sym.DROP_VIEW);
                    s.Write(viewName);
                    s.WriteStatementTerminator(false);
                }, state);

                state.Write(Sym.cp);
            }
            else
            {
                state.Write(Sym.DROP_VIEW);
                state.Write(ResolveName(statement.Name));
            }
        }

        private void VisitAlterViewStatement(AlterViewStatement statement, VisitorState state)
        {
            state.Write(Sym.ALTER_VIEW);
            state.Write(ResolveName(statement.Name));
            state.Write(Sym.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }


        #endregion Statements

        #region Select Parts

        



        private void VisitOutput(IEnumerable<Token> columns, Name outputInto, VisitorState state)
        {
            VisitTokenSet(columns, state, Sym.OUTPUT, Sym.COMMA, outputInto == null ? null : Sym.INTO + ResolveName(outputInto), true);
        }

        private void VisitTop(Top top, VisitorState state)
        {
            if (top != null)
            {
                state.Write(Sym.TOP_op);
                if (top.Value.HasValue)
                {
                    state.Write(top.Value.Value.ToString());
                }
                else if (top.Parameters.Count > 0)
                {
                    foreach (var parameter in top.Parameters)
                    {
                        state.Parameters.Add(parameter);
                    }
                    state.Write(top.Parameters[0].Name);
                }
                state.Write(Sym.cp);

                if (top.Percent)
                {
                    state.Write(Sym.PERCENT);
                }
                if (top.WithTies)
                {
                    state.Write(Sym.WITH_TIES);
                }
            }
        }


        

        private void VisitWith(bool? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Write(name);
                state.Write(Sym.AssignVal);
                state.Write(value.Value ? Sym.ON : Sym.OFF);
            }
        }

        private void VisitWith(int? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Write(name);
                state.Write(Sym.AssignVal);
                state.Write(value.Value.ToString());
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
                if (!string.IsNullOrWhiteSpace(token.Alias))
                {
                    state.Write(Sym.AS);
                    state.Write(this.OpenQuote, token.Alias, this.CloseQuote);
                }
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
