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

            if (!namePart.StartsWith(Symbols.Pound))
            {
                namePart = Symbols.Pound + namePart;
            }
            return Sql.Name("tempdb", "", namePart);
        }

        internal static string GetTableVariableName(Name name)
        {
            var namePart = name.LastPart;

            if (!namePart.StartsWith(Symbols.At))
            {
                namePart = Symbols.At + namePart;
            }
            return namePart;
        }

        private void Stringify(IStatement statement, VisitorState state)
        {
            Stringify(s => VisitStatement(statement, s), state);
        }

        private void Stringify(Action<VisitorState> fragment, VisitorState state)
        {
            state.WriteBeginStringify(LiteralOpenQuote, LiteralCloseQuote);
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
                state.Write(Symbols.OpenParenthesis);
                if (typedToken.Length.HasValue)
                {
                    state.Write(typedToken.Length.Value == -1 ? Symbols.MAX : typedToken.Length.Value.ToString(CultureInfo.InvariantCulture));
                }
                else if (typedToken.Precision.HasValue)
                {
                    state.Write(typedToken.Precision.Value.ToString(CultureInfo.InvariantCulture));

                    if (typedToken.Scale.HasValue)
                    {
                        state.Write(Symbols.Comma);
                        state.Write(typedToken.Scale.Value.ToString(CultureInfo.InvariantCulture));
                    }
                }

                state.Write(Symbols.CloseParenthesis);
            }
        }


        #region Statements

        protected override void VisitJoinType(Joins join, VisitorState state)
        {
            state.Write(JoinStrings[(int)join]);
        }

        protected override void VisitSet(SetStatement statement, VisitorState state)
        {
            state.Write(Symbols.SET);

            if (statement.Assign != null)
            {
                VisitToken(statement.Assign, state);
            }
        }
        protected override void VisitMerge(MergeStatement statement, VisitorState state)
        {
            state.Write(Symbols.MERGE);

            VisitTop(statement.Top, state);

            VisitInto(statement.Into, state);

            if (!string.IsNullOrWhiteSpace(statement.Into.Alias))
            {
                state.Write(Symbols.AS);
                state.Write(this.IdentifierOpenQuote, statement.Into.Alias, this.IdentifierCloseQuote);
            }

            state.Write(Symbols.USING);
            VisitToken(statement.Using, state);
            if (!string.IsNullOrWhiteSpace(statement.Using.Alias))
            {
                state.Write(Symbols.AS);
                state.Write(this.IdentifierOpenQuote, statement.Using.Alias, this.IdentifierCloseQuote);
            }

            state.Write(Symbols.ON);

            VisitToken(statement.On, state);

            foreach (var when in statement.WhenMatched)
            {
                state.Write(Symbols.WHEN);
                state.Write(Symbols.MATCHED);
                if (when.AndCondition != null)
                {
                    state.Write(Symbols.AND);
                    VisitToken(when.AndCondition, state);
                }
                state.Write(Symbols.THEN);

                VisitToken(when, state);
            }

            foreach (var when in statement.WhenNotMatched)
            {
                state.Write(Symbols.WHEN);
                state.Write(Symbols.NOT);
                state.Write(Symbols.MATCHED);
                state.Write(Symbols.BY);
                state.Write(Symbols.TARGET);
                if (when.AndCondition != null)
                {
                    state.Write(Symbols.AND);
                    VisitToken(when.AndCondition, state);
                }
                state.Write(Symbols.THEN);

                VisitToken(when, state);
            }

            foreach (var when in statement.WhenNotMatchedBySource)
            {
                state.Write(Symbols.WHEN);
                state.Write(Symbols.NOT);
                state.Write(Symbols.MATCHED);
                state.Write(Symbols.BY);
                state.Write(Symbols.SOURCE);
                if (when.AndCondition != null)
                {
                    state.Write(Symbols.AND);
                    VisitToken(when.AndCondition, state);
                }
                state.Write(Symbols.THEN);

                VisitToken(when, state);
            }

            VisitOutput(statement.Output, statement.OutputInto, state);
        }

        // ReSharper disable once UnusedParameter.Local
        protected override void VisitWhenMatchedThenDelete(WhenMatchedThenDelete token, VisitorState state)
        {
            state.Write(Symbols.DELETE);
        }
        protected override void VisitWhenMatchedThenUpdateSet(WhenMatchedThenUpdateSet token, VisitorState state)
        {
            state.Write(Symbols.UPDATE);
            state.Write(Symbols.SET);
            VisitTokenSet(token.Set, state, null, Symbols.Comma, null);
        }
        protected override void VisitWhenNotMatchedThenInsert(WhenNotMatchedThenInsert token, VisitorState state)
        {
            state.Write(Symbols.INSERT);
            if (token.Columns.Count > 0)
            {
                VisitTokenSet(token.Columns, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
            }
            if (token.Values.Count > 0)
            {
                state.Write(Symbols.VALUES);
                VisitTokenSet(token.Values, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
            }
            else
            {
                state.Write(Symbols.DEFAULT);
                state.Write(Symbols.VALUES);
            }
        }

        protected override void VisitSelect(SelectStatement statement, VisitorState state)
        {
            state.Write(Symbols.SELECT);

            if (statement.Distinct)
            {
                state.Write(Symbols.DISTINCT);
            }

            if (statement.Offset == null)
            {
                VisitTop(statement.Top, state);
            }

            // assignments
            if (statement.Set.Count > 0)
            {
                VisitTokenSet(statement.Set, state, null, Symbols.Comma, null);
            }
            else
            {
                // output columns
                if (statement.Output.Count == 0)
                {
                    state.Write(Symbols.Asterisk);
                }
                else
                {
                    VisitTokenSet(statement.Output, state, null, Symbols.Comma, null, true);
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
                state.Write(Symbols.OFFSET);
                VisitToken(statement.Offset, state);
                state.Write(Symbols.ROWS);
                state.Write(Symbols.FETCH);
                state.Write(Symbols.NEXT);
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
                state.Write(Symbols.ROWS);
                state.Write(Symbols.ONLY);
            }

            //WITH CUBE or WITH ROLLUP
        }
        private void VisitInto(Name into, VisitorState state)
        {
            if (into != null)
            {
                state.Write(Symbols.INTO);
                VisitNameToken(into, state);
            }
        }
        protected override void VisitDelete(DeleteStatement statement, VisitorState state)
        {
            state.Write(Symbols.DELETE);

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
            state.Write(Symbols.UPDATE);

            VisitTop(updateStatement.Top, state);

            VisitToken(updateStatement.Target, state, true);

            state.Write(Symbols.SET);
            VisitTokenSet(updateStatement.Set, state, null, Symbols.Comma, null);

            VisitOutput(updateStatement.Output, updateStatement.OutputInto, state);

            VisitFromToken(updateStatement.From, state);

            VisitJoin(updateStatement.Joins, state);

            VisitWhereToken(updateStatement.Where, state);
        }
        protected override void VisitInsert(InsertStatement insertStatement, VisitorState state)
        {
            state.Write(Symbols.INSERT);

            VisitTop(insertStatement.Top, state);

            VisitInto(insertStatement.Into, state);

            VisitTokenSet(insertStatement.Columns, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis, true);

            VisitOutput(insertStatement.Output, insertStatement.OutputInto, state);

            if (insertStatement.DefaultValues)
            {
                state.Write(Symbols.DEFAULT);
                state.Write(Symbols.VALUES);

            }
            else if (insertStatement.Values.Count > 0)
            {
                var separator = Symbols.VALUES;
                foreach (var valuesSet in insertStatement.Values)
                {
                    state.Write(separator);
                    separator = Symbols.Comma;

                    VisitTokenSet(valuesSet, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
                }
            }
            else if (insertStatement.From != null)
            {
                VisitStatement(insertStatement.From, state);
            }
        }

        protected override void VisitBeginTransaction(BeginTransactionStatement statement, VisitorState state)
        {
            state.Write(Symbols.BEGIN);
            state.Write(Symbols.TRANSACTION);
            if (VisitTransactionName(statement, state) && !string.IsNullOrWhiteSpace(statement.Description))
            {
                state.Write(Symbols.WITH);
                state.Write(Symbols.MARK);
                state.Write(this.LiteralOpenQuote, statement.Description, this.LiteralCloseQuote);
            }
        }
        protected override void VisitCommitTransaction(CommitTransactionStatement statement, VisitorState state)
        {
            state.Write(Symbols.COMMIT);
            state.Write(Symbols.TRANSACTION);
            VisitTransactionName(statement, state);
        }
        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement, VisitorState state)
        {
            state.Write(Symbols.ROLLBACK);
            state.Write(Symbols.TRANSACTION);
            VisitTransactionName(statement, state);
        }
        protected override void VisitSaveTransaction(SaveTransactionStatement statement, VisitorState state)
        {
            state.Write(Symbols.SAVE);
            state.Write(Symbols.TRANSACTION);
            VisitTransactionName(statement, state);
        }

        protected override void VisitDeclareStatement(DeclareStatement statement, VisitorState state)
        {
            if (statement.Variable != null)
            {
                state.Variables.Add(statement.Variable);

                state.Write(Symbols.DECLARE);
                state.Write(statement.Variable.Name);

                VisitType(statement.Variable, state);

                if (statement.Initializer != null)
                {
                    state.Write(Symbols.AssignVal);
                    VisitToken(statement.Initializer, state);
                }
            }
        }

        // ReSharper disable once UnusedParameter.Local
        protected override void VisitBreakStatement(BreakStatement statement, VisitorState state)
        {
            state.Write(Symbols.BREAK);
        }

        // ReSharper disable once UnusedParameter.Local
        protected override void VisitContinueStatement(ContinueStatement statement, VisitorState state)
        {
            state.Write(Symbols.CONTINUE);
        }
        protected override void VisitGotoStatement(GotoStatement statement, VisitorState state)
        {
            state.Write(Symbols.GOTO);
            state.Write(statement.Label);
        }
        protected override void VisitReturnStatement(ReturnStatement statement, VisitorState state)
        {
            state.Write(Symbols.RETURN);
            if (statement.ReturnExpression != null)
            {
                VisitToken(statement.ReturnExpression, state);
            }

        }
        protected override void VisitThrowStatement(ThrowStatement statement, VisitorState state)
        {
            state.Write(Symbols.THROW);
            if (statement.ErrorNumber != null && statement.Message != null && statement.State != null)
            {
                VisitToken(statement.ErrorNumber, state);
                state.Write(Symbols.Comma);
                VisitToken(statement.Message, state);
                state.Write(Symbols.Comma);
                VisitToken(statement.State, state);
            }
        }
        protected override void VisitTryCatchStatement(TryCatchStatement stmt, VisitorState state)
        {
            state.Write(Symbols.BEGIN);
            state.Write(Symbols.TRY);
            state.WriteCRLF();
            VisitStatement(stmt.TryStatement, state);
            state.WriteStatementTerminator();
            state.Write(Symbols.END);
            state.Write(Symbols.TRY);
            state.WriteCRLF();
            state.Write(Symbols.BEGIN);
            state.Write(Symbols.CATCH);
            state.WriteCRLF();
            if (stmt.CatchStatement != null)
            {
                VisitStatement(stmt.CatchStatement, state);
                state.WriteStatementTerminator();
            }
            state.Write(Symbols.END);
            state.Write(Symbols.CATCH);
            state.WriteStatementTerminator();
        }
        protected override void VisitLabelStatement(LabelStatement stmt, VisitorState state)
        {
            state.Write(stmt.Label, Symbols.Colon);
        }
        protected override void VisitWaitforDelayStatement(WaitforDelayStatement stmt, VisitorState state)
        {
            state.Write(Symbols.WAITFOR);
            state.Write(Symbols.DELAY);
            state.Write(LiteralOpenQuote, stmt.Delay.ToString("HH:mm:ss"), LiteralCloseQuote);
        }
        protected override void VisitWaitforTimeStatement(WaitforTimeStatement stmt, VisitorState state)
        {
            state.Write(Symbols.WAITFOR);
            state.Write(Symbols.TIME);
            state.Write(LiteralOpenQuote, stmt.Time.ToString("yyyy-MM-ddTHH:mm:ss"), LiteralCloseQuote);
        }
        protected override void VisitWhileStatement(WhileStatement stmt, VisitorState state)
        {
            if (stmt.Condition != null)
            {
                state.Write(Symbols.WHILE);
                VisitToken(stmt.Condition, state);

                if (stmt.Do != null)
                {
                    state.WriteCRLF();
                    state.Write(Symbols.BEGIN);
                    state.WriteStatementTerminator();

                    VisitStatement(stmt.Do, state);
                    state.WriteStatementTerminator();

                    state.Write(Symbols.END);
                    state.WriteStatementTerminator();
                }
            }
        }
        protected override void VisitIfStatement(IfStatement ifs, VisitorState state)
        {
            if (ifs.Condition != null)
            {
                state.Write(Symbols.IF);
                VisitToken(ifs.Condition, state);

                if (ifs.Then != null)
                {
                    state.WriteCRLF();
                    state.Write(Symbols.BEGIN);
                    state.WriteStatementTerminator();

                    VisitStatement(ifs.Then, state);
                    state.WriteStatementTerminator();

                    state.Write(Symbols.END);
                    state.WriteStatementTerminator();

                    if (ifs.Else != null)
                    {
                        state.Write(Symbols.ELSE);
                        state.WriteCRLF();
                        state.Write(Symbols.BEGIN);
                        state.WriteStatementTerminator();

                        VisitStatement(ifs.Else, state);
                        state.WriteStatementTerminator();

                        state.Write(Symbols.END);
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
                state.Write(Symbols.IF);
                state.Write(Symbols.OBJECT_ID);
                state.Write(Symbols.OpenParenthesis);
                state.Write(LiteralOpenQuote, tableName, LiteralCloseQuote);
                state.Write(Symbols.Comma);
                state.Write(LiteralOpenQuote, "U", LiteralCloseQuote);
                state.Write(Symbols.CloseParenthesis);
                state.Write(Symbols.IS);
                state.Write(Symbols.NOT);
                state.Write(Symbols.NULL);
            }

            state.Write(Symbols.DROP);
            state.Write(Symbols.TABLE);
            state.Write(tableName);
        }
        protected override void VisitCreateTableStatement(CreateTableStatement createStatement, VisitorState state)
        {
            if (createStatement.IsTableVariable)
            {
                state.Write(Symbols.DECLARE);
                state.Write(GetTableVariableName(createStatement.Name));
                state.Write(Symbols.TABLE);
            }
            else
            {
                var tableName =
                    ResolveName((createStatement.IsTemporary) ? GetTempTableName(createStatement.Name) : createStatement.Name);

                if (createStatement.CheckIfNotExists)
                {
                    state.Write(Symbols.IF);
                    state.Write(Symbols.OBJECT_ID);
                    state.Write(Symbols.OpenParenthesis);
                    state.Write(LiteralOpenQuote, tableName, LiteralCloseQuote);
                    state.Write(Symbols.Comma);
                    state.Write(LiteralOpenQuote, "U", LiteralCloseQuote);
                    state.Write(Symbols.CloseParenthesis);
                    state.Write(Symbols.IS);
                    state.Write(Symbols.NULL);

                    state.WriteCRLF();
                    state.Write(Symbols.BEGIN);
                    state.WriteStatementTerminator();
                }

                state.Write(Symbols.CREATE);
                state.Write(Symbols.TABLE);
                state.Write(tableName);
                if (createStatement.AsFiletable)
                {
                    state.Write(Symbols.AS);
                    state.Write(Symbols.FILETABLE);
                }
            }


            var separator = Symbols.OpenParenthesis;
            foreach (var column in createStatement.Columns)
            {
                state.Write(separator);
                separator = Symbols.Comma;

                state.Write(this.IdentifierOpenQuote, column.Name, this.IdentifierCloseQuote);

                VisitType(column, state);

                if (column.Sparse)
                {
                    state.Write(Symbols.SPARSE);
                }
                if (column.Null.HasValue)
                {
                    if (!column.Null.Value)
                    {
                        state.Write(Symbols.NOT);
                    }
                    state.Write(Symbols.NULL);
                }
                if (column.Identity.On)
                {
                    state.Write(Symbols.IDENTITY);
                    state.Write(Symbols.OpenParenthesis);
                    state.Write(column.Identity.Seed.ToString(CultureInfo.InvariantCulture));
                    state.Write(Symbols.Comma);
                    state.Write(column.Identity.Increment.ToString(CultureInfo.InvariantCulture));
                    state.Write(Symbols.CloseParenthesis);
                }
                if (column.RowGuid)
                {
                    state.Write(Symbols.ROWGUIDCOL);
                }
                if (column.DefaultValue != null)
                {
                    state.Write(Symbols.DEFAULT);
                    state.Write(Symbols.OpenParenthesis);
                    VisitToken(column.DefaultValue, state);
                    state.Write(Symbols.CloseParenthesis);
                }
            }

            if (createStatement.PrimaryKey != null)
            {
                state.Write(Symbols.Comma);
                if (!createStatement.IsTableVariable)
                {
                    state.Write(Symbols.CONSTRAINT);
                    VisitNameToken(createStatement.PrimaryKey.Name, state);
                }

                state.Write(Symbols.PRIMARY);
                state.Write(Symbols.KEY);
                VisitTokenSet(createStatement.PrimaryKey.Columns, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
            }

            foreach (var unique in createStatement.UniqueConstrains)
            {
                state.Write(Symbols.Comma);
                if (!createStatement.IsTableVariable)
                {
                    state.Write(Symbols.CONSTRAINT);
                    VisitNameToken(unique.Name, state);
                }

                state.Write(Symbols.UNIQUE);
                if (unique.Clustered.HasValue)
                {
                    state.Write(unique.Clustered.Value ? Symbols.CLUSTERED : Symbols.NONCLUSTERED);
                }
                VisitTokenSet(unique.Columns, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
            }

            state.Write(Symbols.CloseParenthesis);
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
                state.Write(Symbols.END);
            }
        }

        protected override void VisitStringifyStatement(StringifyStatement statement, VisitorState state)
        {
            this.Stringify(statement.Content, state);
        }
        protected override void VisitExecuteStatement(ExecuteStatement statement, VisitorState state)
        {
            state.Write(Symbols.EXEC);
            state.Write(Symbols.OpenParenthesis);
            this.Stringify(statement.Target, state);
            state.Write(Symbols.CloseParenthesis);
        }
        protected override void VisitCreateIndexStatement(CreateIndexStatement createIndexStatement, VisitorState state)
        {
            state.Write(Symbols.CREATE);

            if (createIndexStatement.Unique)
            {
                state.Write(Symbols.UNIQUE);
            }

            if (createIndexStatement.Clustered.HasValue)
            {
                state.Write(createIndexStatement.Clustered.Value ? Symbols.CLUSTERED : Symbols.NONCLUSTERED);
            }
            state.Write(Symbols.INDEX);

            VisitToken(createIndexStatement.Name, state);

            state.Write(Symbols.ON);

            VisitToken(createIndexStatement.On, state);

            // columns
            VisitTokenSet(createIndexStatement.Columns, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);

            if (createIndexStatement.Include.Count > 0)
            {
                state.Write(Symbols.INCLUDE);
                VisitTokenSet(createIndexStatement.Include, state, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);
            }

            VisitWhereToken(createIndexStatement.Where, state);

            if (createIndexStatement.With.IsDefined)
            {
                state.Write(Symbols.WITH);
                state.Write(Symbols.OpenParenthesis);

                VisitWith(createIndexStatement.With.PadIndex, Symbols.PAD_INDEX, state);
                VisitWith(createIndexStatement.With.Fillfactor, Symbols.FILLFACTOR, state);
                VisitWith(createIndexStatement.With.SortInTempdb, Symbols.SORT_IN_TEMPDB, state);
                VisitWith(createIndexStatement.With.IgnoreDupKey, Symbols.IGNORE_DUP_KEY, state);
                VisitWith(createIndexStatement.With.StatisticsNorecompute, Symbols.STATISTICS_NORECOMPUTE, state);
                VisitWith(createIndexStatement.With.DropExisting, Symbols.DROP_EXISTING, state);
                VisitWith(createIndexStatement.With.Online, Symbols.ONLINE, state);
                VisitWith(createIndexStatement.With.AllowRowLocks, Symbols.ALLOW_ROW_LOCKS, state);
                VisitWith(createIndexStatement.With.AllowPageLocks, Symbols.ALLOW_PAGE_LOCKS, state);
                VisitWith(createIndexStatement.With.MaxDegreeOfParallelism, Symbols.MAXDOP, state);

                state.Write(Symbols.CloseParenthesis);
            }

            state.Write(Symbols.Semicolon);
        }
        protected override void VisitAlterIndexStatement(AlterIndexStatement alterStatement, VisitorState state)
        {
            state.Write(Symbols.ALTER);
            state.Write(Symbols.INDEX);

            if (alterStatement.Name == null)
            {
                state.Write(Symbols.ALL);
            }
            else
            {
                VisitToken(alterStatement.Name, state);
            }

            state.Write(Symbols.ON);

            VisitToken(alterStatement.On, state);

            if (alterStatement.Rebuild)
            {
                state.Write(Symbols.REBUILD);

                //TODO: [PARTITION = ALL]
                if (alterStatement.RebuildWith.IsDefined)
                {
                    state.Write(Symbols.WITH);
                    state.Write(Symbols.OpenParenthesis);

                    VisitWith(alterStatement.RebuildWith.PadIndex, Symbols.PAD_INDEX, state);
                    VisitWith(alterStatement.RebuildWith.Fillfactor, Symbols.FILLFACTOR, state);
                    VisitWith(alterStatement.RebuildWith.SortInTempdb, Symbols.SORT_IN_TEMPDB, state);
                    VisitWith(alterStatement.RebuildWith.IgnoreDupKey, Symbols.IGNORE_DUP_KEY, state);
                    VisitWith(alterStatement.RebuildWith.StatisticsNorecompute, Symbols.STATISTICS_NORECOMPUTE, state);
                    VisitWith(alterStatement.RebuildWith.DropExisting, Symbols.DROP_EXISTING, state);
                    VisitWith(alterStatement.RebuildWith.Online, Symbols.ONLINE, state);
                    VisitWith(alterStatement.RebuildWith.AllowRowLocks, Symbols.ALLOW_ROW_LOCKS, state);
                    VisitWith(alterStatement.RebuildWith.AllowPageLocks, Symbols.ALLOW_PAGE_LOCKS, state);
                    VisitWith(alterStatement.RebuildWith.MaxDegreeOfParallelism, Symbols.MAXDOP, state);

                    state.Write(Symbols.CloseParenthesis);
                }
            }
            else if (alterStatement.Disable)
            {
                state.Write(Symbols.DISABLE);
            }
            else if (alterStatement.Reorganize)
            {
                state.Write(Symbols.REORGANIZE);
            }
            else
            {
                VisitWith(alterStatement.Set.AllowRowLocks, Symbols.ALLOW_ROW_LOCKS, state);
                VisitWith(alterStatement.Set.AllowPageLocks, Symbols.ALLOW_PAGE_LOCKS, state);
                VisitWith(alterStatement.Set.IgnoreDupKey, Symbols.IGNORE_DUP_KEY, state);
                VisitWith(alterStatement.Set.StatisticsNorecompute, Symbols.STATISTICS_NORECOMPUTE, state);
            }
        }
        protected override void VisitDropIndexStatement(DropIndexStatement dropIndexStatement, VisitorState state)
        {
            state.Write(Symbols.DROP);
            state.Write(Symbols.INDEX);
            VisitToken(dropIndexStatement.Name, state);

            state.Write(Symbols.ON);

            VisitToken(dropIndexStatement.On, state);

            if (dropIndexStatement.With.IsDefined)
            {
                state.Write(Symbols.WITH);
                state.Write(Symbols.OpenParenthesis);

                if (dropIndexStatement.With.Online.HasValue)
                {
                    state.Write(Symbols.ONLINE);
                    state.Write(Symbols.AssignVal);
                    state.Write(dropIndexStatement.With.Online.Value ? Symbols.ON : Symbols.OFF);
                }
                if (dropIndexStatement.With.MaxDegreeOfParallelism.HasValue)
                {
                    state.Write(Symbols.MAXDOP);
                    state.Write(Symbols.AssignVal);
                    state.Write(dropIndexStatement.With.MaxDegreeOfParallelism.Value.ToString(CultureInfo.InvariantCulture));
                }

                state.Write(Symbols.CloseParenthesis);
            }
        }
        protected override void VisitCreateViewStatement(CreateViewStatement createStatement, VisitorState state)
        {
            var viewName = ResolveName(createStatement.Name);

            if (createStatement.CheckIfNotExists)
            {
                state.Write(Symbols.IF);
                state.Write(Symbols.OBJECT_ID);
                state.Write(Symbols.OpenParenthesis);
                state.Write(LiteralOpenQuote, viewName, LiteralCloseQuote);
                state.Write(Symbols.CloseParenthesis);
                state.Write(Symbols.IS);
                state.Write(Symbols.NULL);
                state.Write(Symbols.EXEC);
                state.Write(Symbols.OpenParenthesis);

                Stringify(s =>
                {
                    s.Write(Symbols.CREATE);
                    s.Write(Symbols.VIEW);
                    s.Write(viewName);
                    s.Write(Symbols.AS);
                    VisitStatement(createStatement.DefinitionStatement, state);
                }, state);
                state.Write(Symbols.CloseParenthesis);
            }
            else
            {
                state.Write(Symbols.CREATE);
                state.Write(Symbols.VIEW);
                state.Write(viewName);
                state.Write(Symbols.AS);
                VisitStatement(createStatement.DefinitionStatement, state);
            }
        }
        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement createStatement, VisitorState state)
        {
            var viewName = ResolveName(createStatement.Name);
            state.Write(Symbols.IF);
            state.Write(Symbols.OBJECT_ID);
            state.Write(Symbols.OpenParenthesis);
            state.Write(LiteralOpenQuote, viewName, LiteralCloseQuote);
            state.Write(Symbols.CloseParenthesis);
            state.Write(Symbols.IS);
            state.Write(Symbols.NULL);
            state.Write(Symbols.EXEC);
            state.Write(Symbols.OpenParenthesis);

            Stringify(s =>
            {
                s.Write(Symbols.CREATE);
                s.Write(Symbols.VIEW);
                s.Write(viewName);
                s.Write(Symbols.AS);
                VisitStatement(createStatement.DefinitionStatement, s);
            }, state);

            state.Write(Symbols.CloseParenthesis);
            state.WriteStatementTerminator();
            state.Write(Symbols.ELSE);
            state.Write(Symbols.EXEC);
            state.Write(Symbols.OpenParenthesis);

            Stringify(s =>
            {
                s.Write(Symbols.ALTER);
                s.Write(Symbols.VIEW);
                s.Write(viewName);
                s.Write(Symbols.AS);
                VisitStatement(createStatement.DefinitionStatement, s);
            }, state);

            state.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDropViewStatement(DropViewStatement statement, VisitorState state)
        {
            var viewName = ResolveName(statement.Name);

            if (statement.CheckExists)
            {
                state.Write(Symbols.IF);
                state.Write(Symbols.OBJECT_ID);
                state.Write(Symbols.OpenParenthesis);
                state.Write(LiteralOpenQuote, viewName, LiteralCloseQuote);
                state.Write(Symbols.CloseParenthesis);
                state.Write(Symbols.IS);
                state.Write(Symbols.NOT);
                state.Write(Symbols.NULL);
                state.Write(Symbols.EXEC);
                state.Write(Symbols.OpenParenthesis);

                Stringify(s =>
                {
                    s.Write(Symbols.DROP);
                    s.Write(Symbols.VIEW);
                    s.Write(viewName);
                    s.WriteStatementTerminator(false);
                }, state);

                state.Write(Symbols.CloseParenthesis);
            }
            else
            {
                state.Write(Symbols.DROP);
                state.Write(Symbols.VIEW);
                VisitNameToken(statement.Name, state);
            }
        }

        protected override void VisitAlterViewStatement(AlterViewStatement statement, VisitorState state)
        {
            state.Write(Symbols.ALTER);
            state.Write(Symbols.VIEW);
            VisitNameToken(statement.Name, state);
            state.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement, state);
        }


        #endregion Statements

        private void VisitOutput(IEnumerable<Token> columns, Name outputInto, VisitorState state)
        {
            VisitTokenSet(columns, state, Symbols.OUTPUT, Symbols.Comma, null, true);
            if (outputInto != null)
            {
                state.Write(Symbols.INTO);
                VisitNameToken(outputInto, state);
            }

        }

        private void VisitTop(Top top, VisitorState state)
        {
            if (top != null)
            {
                state.Write(Symbols.TOP);
                state.Write(Symbols.OpenParenthesis);
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
                state.Write(Symbols.CloseParenthesis);

                if (top.Percent)
                {
                    state.Write(Symbols.PERCENT);
                }
                if (top.WithTies)
                {
                    state.Write(Symbols.WITH);
                    state.Write(Symbols.TIES);
                }
            }
        }




        private void VisitWith(bool? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Write(name);
                state.Write(Symbols.AssignVal);
                state.Write(value.Value ? Symbols.ON : Symbols.OFF);
            }
        }

        private void VisitWith(int? value, string name, VisitorState state)
        {
            if (value.HasValue)
            {
                state.Write(name);
                state.Write(Symbols.AssignVal);
                state.Write(value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        protected override void VisitStringifyToken(StringifyToken token, VisitorState state)
        {
            Stringify(s => VisitToken(token.Content, s), state);
        }
    }
}
