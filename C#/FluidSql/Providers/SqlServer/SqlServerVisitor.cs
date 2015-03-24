using System;
using System.Collections.Generic;
using System.Globalization;

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
            visitor.VisitToken(token, state);
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

        protected override string IdentifierCloseQuote { get { return "]"; } }
        protected override string IdentifierOpenQuote { get { return "["; } }
        protected override string LiteralOpenQuote { get { return "N'"; } }
        protected override string LiteralCloseQuote { get { return "'"; } }
        protected override string CommentOpenQuote { get { return "/*"; } }
        protected override string CommentCloseQuote { get { return "*/"; } }



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
                    state.Write(typedToken.Precision.Value.ToString(CultureInfo.InvariantCulture));

                    if (typedToken.Scale.HasValue)
                    {
                        state.Write(Sym.COMMA);
                        state.Write(typedToken.Scale.Value.ToString(CultureInfo.InvariantCulture));
                    }
                }

                state.Write(")");
            }
        }


        #region Statements

        protected override void VisitJoinType(Joins join, VisitorState state)
        {
            state.Write(JoinStrings[(int)join]);
        }

        protected override void VisitSet(SetStatement statement, VisitorState state)
        {
            state.Write(Sym.SET);

            if (statement.Assign != null)
            {
                VisitToken(statement.Assign, state);
            }
        }
        protected override void VisitMerge(MergeStatement statement, VisitorState state)
        {
            state.Write(Sym.MERGE);

            VisitTop(statement.Top, state);

            VisitInto(statement.Into, state);

            if (!string.IsNullOrWhiteSpace(statement.Into.Alias))
            {
                state.Write(Sym.AS);
                state.Write(this.IdentifierOpenQuote, statement.Into.Alias, this.IdentifierCloseQuote);
            }

            state.Write(Sym.USING);
            VisitToken(statement.Using, state);
            if (!string.IsNullOrWhiteSpace(statement.Using.Alias))
            {
                state.Write(Sym.AS);
                state.Write(this.IdentifierOpenQuote, statement.Using.Alias, this.IdentifierCloseQuote);
            }

            state.Write(Sym.ON);

            VisitToken(statement.On, state);

            foreach (var when in statement.WhenMatched)
            {
                state.Write(Sym.WHEN_MATCHED);
                if (when.AndCondition != null)
                {
                    state.Write(Sym.AND);
                    VisitToken(when.AndCondition, state);
                }
                state.Write(Sym.THEN);

                VisitToken(when, state);
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

                VisitToken(when, state);
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

                VisitToken(when, state);
            }

            VisitOutput(statement.Output, statement.OutputInto, state);
        }

        // ReSharper disable once UnusedParameter.Local
        protected override void VisitWhenMatchedThenDelete(WhenMatchedThenDelete token, VisitorState state)
        {
            state.Write(Sym.DELETE);
        }
        protected override void VisitWhenMatchedThenUpdateSet(WhenMatchedThenUpdateSet token, VisitorState state)
        {
            state.Write(Sym.UPDATE_SET);
            VisitTokenSet(token.Set, state, null, Sym.COMMA, null);
        }
        protected override void VisitWhenNotMatchedThenInsert(WhenNotMatchedThenInsert token, VisitorState state)
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

        protected override void VisitSelect(SelectStatement statement, VisitorState state)
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

            VisitFromToken(statement.From, state);

            VisitJoin(statement.Joins, state);

            VisitWhereToken(statement.Where, state);

            VisitGroupByToken(statement.GroupBy, state);

            VisitHavingToken(statement.Having, state);

            VisitOrderByToken(statement.OrderBy, state);

            if (statement.Offset != null)
            {
                state.Write(Sym.OFFSET);
                VisitToken(statement.Offset, state);
                state.Write(Sym.ROWS);
                state.Write(Sym.FETCH_NEXT);
                if (statement.Top.Value.HasValue)
                {
                    state.Write(statement.Top.Value.Value.ToString(CultureInfo.InvariantCulture));
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
        protected override void VisitDelete(DeleteStatement statement, VisitorState state)
        {
            state.Write(Sym.DELETE);

            VisitTop(statement.Top, state);

            if (statement.Joins.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(statement.From.Alias))
                {
                    state.Write(this.IdentifierOpenQuote, statement.From.Alias, this.IdentifierCloseQuote);
                }

                VisitOutput(statement.Output, statement.OutputInto, state);

                VisitFromToken(statement.From, state);

                VisitJoin(statement.Joins, state);

                VisitWhereToken(statement.Where, state);
            }
            else
            {
                VisitFromToken(statement.From, state);

                VisitOutput(statement.Output, statement.OutputInto, state);

                VisitWhereToken(statement.Where, state);
            }
        }
        protected override void VisitUpdate(UpdateStatement updateStatement, VisitorState state)
        {
            state.Write(Sym.UPDATE);

            VisitTop(updateStatement.Top, state);

            VisitToken(updateStatement.Target, state, true);

            state.Write(Sym.SET);
            VisitTokenSet(updateStatement.Set, state, null, Sym.COMMA, null);

            VisitOutput(updateStatement.Output, updateStatement.OutputInto, state);

            VisitFromToken(updateStatement.From, state);

            VisitJoin(updateStatement.Joins, state);

            VisitWhereToken(updateStatement.Where, state);
        }
        protected override void VisitInsert(InsertStatement insertStatement, VisitorState state)
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

        protected override void VisitBeginTransaction(BeginTransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.BEGIN_TRANSACTION);
            if (VisitTransactionName(statement, state) && !string.IsNullOrWhiteSpace(statement.Description))
            {
                state.Write(Sym.WITH_MARK);
                state.Write("N'", statement.Description, "'");
            }
        }
        protected override void VisitCommitTransaction(CommitTransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.COMMIT_TRANSACTION);
            VisitTransactionName(statement, state);
        }
        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.ROLLBACK_TRANSACTION);
            VisitTransactionName(statement, state);
        }
        protected override void VisitSaveTransaction(SaveTransactionStatement statement, VisitorState state)
        {
            state.Write(Sym.SAVE_TRANSACTION);
            VisitTransactionName(statement, state);
        }

        protected override void VisitDeclareStatement(DeclareStatement statement, VisitorState state)
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
        protected override void VisitBreakStatement(BreakStatement statement, VisitorState state)
        {
            state.Write(Sym.BREAK);
        }

        // ReSharper disable once UnusedParameter.Local
        protected override void VisitContinueStatement(ContinueStatement statement, VisitorState state)
        {
            state.Write(Sym.CONTINUE);
        }
        protected override void VisitGotoStatement(GotoStatement statement, VisitorState state)
        {
            state.Write(Sym.GOTO);
            state.Write(statement.Label);
        }
        protected override void VisitReturnStatement(ReturnStatement statement, VisitorState state)
        {
            state.Write(Sym.RETURN);
            if (statement.ReturnExpression != null)
            {
                VisitToken(statement.ReturnExpression, state);
            }

        }
        protected override void VisitThrowStatement(ThrowStatement statement, VisitorState state)
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
        protected override void VisitTryCatchStatement(TryCatchStatement stmt, VisitorState state)
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
        protected override void VisitLabelStatement(LabelStatement stmt, VisitorState state)
        {
            state.Write(stmt.Label, ":");
        }
        protected override void VisitWaitforDelayStatement(WaitforDelayStatement stmt, VisitorState state)
        {
            state.Write(Sym.WAITFOR_DELAY);
            state.Write("N'", stmt.Delay.ToString("HH:mm:ss"), "'");
        }
        protected override void VisitWaitforTimeStatement(WaitforTimeStatement stmt, VisitorState state)
        {
            state.Write(Sym.WAITFOR_TIME);
            state.Write("N'", stmt.Time.ToString("yyyy-MM-ddTHH:mm:ss"), "'");
        }
        protected override void VisitWhileStatement(WhileStatement stmt, VisitorState state)
        {
            if (stmt.Condition != null)
            {
                state.Write(Sym.WHILE);
                VisitToken(stmt.Condition, state);

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
        protected override void VisitIfStatement(IfStatement ifs, VisitorState state)
        {
            if (ifs.Condition != null)
            {
                state.Write(Sym.IF);
                VisitToken(ifs.Condition, state);

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
        protected override void VisitDropTableStatement(DropTableStatement statement, VisitorState state)
        {
            var tableName = ResolveName((statement.IsTemporary) ? GetTempTableName(statement.Name) : statement.Name);

            if (statement.CheckExists)
            {
                state.Write("IF OBJECT_ID ( N'", tableName, "', N'U' ) IS NOT NULL");
            }

            state.Write(Sym.DROP_TABLE);
            state.Write(tableName);
        }
        protected override void VisitCreateTableStatement(CreateTableStatement createStatement, VisitorState state)
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

                state.Write(this.IdentifierOpenQuote, column.Name, this.IdentifierCloseQuote);

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
                    state.Write(column.Identity.Seed.ToString(CultureInfo.InvariantCulture));
                    state.Write(Sym.COMMA);
                    state.Write(column.Identity.Increment.ToString(CultureInfo.InvariantCulture));
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
                    VisitToken(column.DefaultValue, state);
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
                    state.Write(unique.Clustered.Value ? Sym.CLUSTERED : Sym.NONCLUSTERED);
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

        protected override void VisitStringifyStatement(StringifyStatement statement, VisitorState state)
        {
            this.Stringify(statement.Content, state);
        }
        protected override void VisitExecuteStatement(ExecuteStatement statement, VisitorState state)
        {
            state.Write(Sym.EXEC);
            state.Write(Sym.op);
            this.Stringify(statement.Target, state);
            state.Write(Sym.cp);
        }
        protected override void VisitCreateIndexStatement(CreateIndexStatement createIndexStatement, VisitorState state)
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

            VisitWhereToken(createIndexStatement.Where, state);

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
        protected override void VisitAlterIndexStatement(AlterIndexStatement alterStatement, VisitorState state)
        {
            state.Write(Sym.ALTER_INDEX);

            if (alterStatement.Name == null)
            {
                state.Write(Sym.ALL);
            }
            else
            {
                VisitToken(alterStatement.Name, state);
            }

            state.Write(Sym.ON);

            VisitToken(alterStatement.On, state);

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
        protected override void VisitDropIndexStatement(DropIndexStatement dropIndexStatement, VisitorState state)
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
                    state.Write(dropIndexStatement.With.MaxDegreeOfParallelism.Value.ToString(CultureInfo.InvariantCulture));
                }

                state.Write(Sym.cp);
            }
        }
        protected override void VisitCreateViewStatement(CreateViewStatement createStatement, VisitorState state)
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
        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement createStatement, VisitorState state)
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

        protected override void VisitDropViewStatement(DropViewStatement statement, VisitorState state)
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
                VisitNameToken(statement.Name, state);
            }
        }

        protected override void VisitAlterViewStatement(AlterViewStatement statement, VisitorState state)
        {
            state.Write(Sym.ALTER_VIEW);
            VisitNameToken(statement.Name, state);
            state.Write(Sym.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }


        #endregion Statements

        #region Select Parts





        private void VisitOutput(IEnumerable<Token> columns, Name outputInto, VisitorState state)
        {
            VisitTokenSet(columns, state, Sym.OUTPUT, Sym.COMMA, null, true);
            if (outputInto != null)
            {
                state.Write(Sym.INTO);
                VisitNameToken(outputInto, state);
            }

        }

        private void VisitTop(Top top, VisitorState state)
        {
            if (top != null)
            {
                state.Write(Sym.TOP_op);
                if (top.Value.HasValue)
                {
                    state.Write(top.Value.Value.ToString(CultureInfo.InvariantCulture));
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
                state.Write(value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        #endregion Select Parts

        #region Tokens


        protected override void VisitStringifyToken(StringifyToken token, VisitorState state)
        {
            Stringify(s => VisitToken(token.Content, s), state);
        }

        #endregion Tokens
    }
}
