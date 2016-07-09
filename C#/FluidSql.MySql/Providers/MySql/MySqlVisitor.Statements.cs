// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;

namespace TTRider.FluidSql.Providers.MySql
{
    internal partial class MySqlVisitor
    {
        //TODO: The DELETE syntax doesn't allow for returning values
        //TODO: Limit percent is not allowed in MySql
        //TODO: MySQL doesn't support the WITH clause
        protected override void VisitDelete(DeleteStatement statement)
        {
            bool isSingleDelete = (statement.UsingList.Count == 0);

            State.Write(Symbols.DELETE);

            if (isSingleDelete)
            {
                if ((statement.RecordsetSource.Source is ExpressionToken) && (!String.IsNullOrEmpty(((ExpressionToken)statement.RecordsetSource.Source).Alias)))
                {
                    VisitNameToken(Sql.Name(((ExpressionToken)statement.RecordsetSource.Source).Alias));
                }
                State.Write(Symbols.FROM);
                VisitFromToken(statement.RecordsetSource);
            }
            else
            {
                State.Write(Symbols.FROM);
                if ((statement.RecordsetSource.Source is ExpressionToken) && (!String.IsNullOrEmpty(((ExpressionToken)statement.RecordsetSource.Source).Alias)))
                {
                    VisitNameToken(Sql.Name(((ExpressionToken)statement.RecordsetSource.Source).Alias));
                }
                else
                {
                    VisitFromToken(statement.RecordsetSource);
                }
                VisitUsingToken(PrepareDeleteUsingList(statement.RecordsetSource, statement.UsingList));
            }

            VisitJoin(statement.Joins);

            VisitWhereToken(statement.Where);

            VisitTopToken(statement.Top);
        }

        protected override void VisitUpdate(UpdateStatement statement)
        {
            State.Write(Symbols.UPDATE);

            VisitToken(statement.Target, true);

            State.Write(Symbols.SET);

            VisitTokenSet(statement.Set);

            VisitJoin(statement.Joins);

            VisitWhereToken(statement.Where);

            VisitTopToken(statement.Top);

            //VisitFromToken(statement.RecordsetSource);
       }

        //TODO: The INSERT syntax doesn't allow for aliases
        //TODO: The INSERT syntax doesn't allow for returning values
        protected override void VisitInsert(InsertStatement statement)
        {
            State.Write(Symbols.INSERT);

            State.Write(Symbols.INTO);

            VisitToken(statement.Into, false);

            if (statement.DefaultValues)
            {
                State.Write(Symbols.VALUES);
                State.Write(Symbols.OpenParenthesis);
                State.Write(Symbols.CloseParenthesis);
            }
            else if (statement.Columns.Count > 0)
            {
                var separator = Symbols.OpenParenthesis;
                foreach (var valuesSet in statement.Columns)
                {
                    State.Write(separator);
                    VisitNameToken(valuesSet);
                    separator = Symbols.Comma;
                }
                State.Write(Symbols.CloseParenthesis);
            }

            if (statement.Values.Count > 0)
            {
                var separator = Symbols.VALUES;
                foreach (var valuesSet in statement.Values)
                {
                    State.Write(separator);
                    separator = Symbols.Comma;

                    VisitTokenSetInParenthesis(valuesSet);
                }
            }
            else if (statement.From != null)
            {
                VisitStatement(statement.From);
                if (statement.Top != null)
                {
                    State.Write(Symbols.LIMIT);
                    VisitToken(Sql.Scalar(statement.Top.IntValue));
                }
            }
        }

        //TODO: TOP PERCENT Statement should be implemented after PREPARE Statement
        //TODO: FULL OUTER JOIN Statement should be implemented after UNION Statement AS UNION LEFT and RIGHT Joins
        //TODO: MySQL Server doesn't support the SELECT ... INTO TABLE Sybase SQL extension
        //TODO: MySQL doesn't support the WITH clause
        protected override void VisitSelect(SelectStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions, true);

            State.Write(Symbols.SELECT);

            if (statement.Distinct)
            {
                State.Write(Symbols.DISTINCT);
            }

            // output columns
            if (statement.Output.Count == 0)
            {
                State.Write(Symbols.Asterisk);
            }
            else
            {
                VisitAliasedTokenSet(statement.Output, (string)null, Symbols.Comma, null);
            }

            //VisitIntoToken(statement.Into);

            if (statement.From.Count > 0)
            {
                State.Write(Symbols.FROM);
                VisitFromToken(statement.From);
            }

            VisitJoin(statement.Joins);

            VisitWhereToken(statement.Where);

            VisitGroupByToken(statement.GroupBy);

            VisitHavingToken(statement.Having);

            VisitOrderByToken(statement.OrderBy);

            VisitTopToken(statement.Top);

            if (statement.Offset != null)
            {
                State.Write(Symbols.OFFSET);
                VisitToken(statement.Offset);
            }
        }

        protected override void VisitMerge(MergeStatement statement) { throw new NotImplementedException(); }

        protected override void VisitSet(SetStatement statement)
        {
            State.Write(Symbols.SET);
            if (statement.Assign != null)
            {
                VisitToken(statement.Assign);
            }
        }

        //TODO: MySQL Server doesn't support the INTERSECT statement
        protected override void VisitIntersectStatement(IntersectStatement statement) { throw new NotImplementedException(); }

        //TODO: MySQL Server doesn't support the EXCEPT statement
        protected override void VisitExceptStatement(ExceptStatement statement) { throw new NotImplementedException(); }

        //TODO: Add transaction characteristic
        // no tested on db
        protected override void VisitBeginTransaction(BeginTransactionStatement statement)
        {
            State.Write(Symbols.START);
            State.Write(Symbols.TRANSACTION);
        }

        //TODO: Add [WORK] [AND [NO] CHAIN] [[NO] RELEASE]
        // no tested on db
        protected override void VisitCommitTransaction(CommitTransactionStatement statement)
        {
            State.Write(Symbols.COMMIT);
        }

        //TODO: Add [WORK] [AND [NO] CHAIN] [[NO] RELEASE]
        // no tested on db
        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement)
        {
            State.Write(Symbols.ROLLBACK);
            if (statement.Name != null || statement.Parameter != null)
            {
                State.Write(Symbols.TO);
                State.Write(Symbols.SAVEPOINT);
                VisitTransactionName(statement);
            }
        }

        // no tested on db
        protected override void VisitSaveTransaction(SaveTransactionStatement statement)
        {
            State.Write(Symbols.SAVEPOINT);
            VisitTransactionName(statement);
        }

        protected override void VisitDeclareStatement(DeclareStatement statement) { throw new NotImplementedException(); }

        //IF control block cannot be OUTSIDE of functions
        protected override void VisitIfStatement(IfStatement statement)
        {
            if (statement.Condition != null)
            {
                State.Write(Symbols.IF);
                VisitToken(statement.Condition);

                if (statement.Then != null)
                {
                    State.WriteCRLF();
                    State.Write(Symbols.THEN);
                    State.WriteCRLF();

                    VisitStatement(statement.Then);
                    State.WriteStatementTerminator();

                    if (statement.Else != null)
                    {
                        State.Write(Symbols.ELSE);
                        State.WriteCRLF();

                        VisitStatement(statement.Else);
                        State.WriteStatementTerminator();
                    }
                }
                State.Write(Symbols.END);
                State.Write(Symbols.IF);
            }
        }

        protected override void VisitCreateTableStatement(CreateTableStatement statement)
        {
            State.Write(Symbols.CREATE);
            if (statement.IsTableVariable || statement.IsTemporary)
            {
                State.Write(Symbols.TEMPORARY);
            }
            State.Write(Symbols.TABLE);

            if (statement.CheckIfNotExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.NOT);
                State.Write(Symbols.EXISTS);
            }

            VisitNameToken(statement.Name);

            var separator = "(";
            foreach (var column in statement.Columns)
            {
                State.Write(separator);
                separator = ",";

                State.Write("`", column.Name, "`");

                VisitType(column);

                if (column.Null.HasValue)
                {
                    if (!column.Null.Value)
                    {
                        State.Write(Symbols.NOT);
                    }
                    State.Write(Symbols.NULL);
                    VisitConflict(column.NullConflict);
                }

                if (column.DefaultValue != null)
                {
                    State.Write(Symbols.DEFAULT);
                    VisitToken(column.DefaultValue);
                }

                if (column.Identity.On)
                {
                    State.Write(MySqlSymbols.AUTO_INCREMENT);
                }

                if (column.PrimaryKeyDirection.HasValue)
                {
                    State.Write(Symbols.PRIMARY);
                    State.Write(Symbols.KEY);
                }
            }

            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDropTableStatement(DropTableStatement statement)
        {
            State.Write(Symbols.DROP);
            if (statement.IsTemporary)
            {
                State.Write(Symbols.TEMPORARY);
            }
            State.Write(Symbols.TABLE);
            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name);
        }

        // TODO: Add Unique / Fulltext / Spatial option
        // TODO: Add index_type  BTREE | HASH
        // TODO: Add index_option
        // TODO: Add Algorithm option and lock option
        // TODO: Doesn't support Where causes
        protected override void VisitCreateIndexStatement(CreateIndexStatement statement)
        {
            CreateIndex(statement);
        }

        //There is no ALTER INDEX command in MySQL
        protected override void VisitAlterIndexStatement(AlterIndexStatement statement)
        {
            VisitStatement(Sql.DropIndex(statement.Name, statement.On, false));
            State.WriteStatementTerminator();
            CreateIndex(statement);
        }

        //TODO: Add algorithm option and lock option
        //TODO: There are no Check If exists option
        protected override void VisitDropIndexStatement(DropIndexStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.INDEX);

            VisitToken(statement.Name);

            if (statement.On != null)
            {
                State.Write(Symbols.ON);
                VisitNameToken(statement.On);
            }
        }

        protected override void VisitStringifyStatement(StringifyStatement statement)
        {
            this.Stringify(statement.Content);
        }

        protected override void VisitBreakStatement(BreakStatement statement)
        {
            State.Write(MySqlSymbols.LEAVE);
            if (!String.IsNullOrEmpty(statement.Label))
            {
                State.Write(statement.Label);
            }
        }

        protected override void VisitContinueStatement(ContinueStatement statement)
        {
            State.Write(MySqlSymbols.ITERATE);
            if (!String.IsNullOrEmpty(statement.Label))
            {
                State.Write(statement.Label);
            }
        }

        //There is no GOTO statement in MySQL
        protected override void VisitGotoStatement(GotoStatement statement) { throw new NotImplementedException(); }

        //TODO: check retirn a variable
        protected override void VisitReturnStatement(ReturnStatement statement)
        {
            State.Write(Symbols.RETURN);
            if (statement.ReturnExpression != null)
            {
                VisitToken(statement.ReturnExpression);
            }
        }

        //TODO: Implement SIGNAL Statement
        protected override void VisitThrowStatement(ThrowStatement statement) { throw new NotImplementedException(); }
        //There is no Try/Catch command in MySQL
        protected override void VisitTryCatchStatement(TryCatchStatement statement) { throw new NotImplementedException(); }

        //Label is used like a block statement (with begin and end)
        protected override void VisitLabelStatement(LabelStatement statement) { throw new NotImplementedException(); }

        //There are no any appropriate function in SQL.cs
        protected override void VisitWaitforDelayStatement(WaitforDelayStatement statement)
        {
            State.Write(Symbols.WAITFOR);
            State.Write(Symbols.DELAY);
            State.Write(LiteralOpenQuote, statement.Delay.ToString("HH:mm:ss"), LiteralCloseQuote);
        }

        protected override void VisitWaitforTimeStatement(WaitforTimeStatement statement) { throw new NotImplementedException(); }
        protected override void VisitWhileStatement(WhileStatement statement) { throw new NotImplementedException(); }

        //TODO: Create Or Replace statement
        //TODO: Check If Exist
        //TODO: Add [ALGORITHM = {UNDEFINED | MERGE | TEMPTABLE}]
        //TODO: Add [DEFINER = { user | CURRENT_USER }]
        //TODO: add[SQL SECURITY { DEFINER | INVOKER }]
        //TODO: Add [WITH[CASCADED | LOCAL] CHECK OPTION]
        protected override void VisitCreateViewStatement(CreateViewStatement statement)
        {
            State.Write(Symbols.CREATE);

            State.Write(Symbols.VIEW);

            if (statement.CheckIfNotExists)
            {
                throw new NotImplementedException();
            }
            VisitNameToken(statement.Name);
            State.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement);
        }

        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement)
        {
            State.Write(Symbols.CREATE);
            State.Write(Symbols.OR);
            State.Write(Symbols.REPLACE);

            State.Write(Symbols.VIEW);

            VisitNameToken(statement.Name);
            State.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement);
        }

        //TODO: Create Or Replace statement
        //TODO: Check If Exist
        //TODO: Add [ALGORITHM = {UNDEFINED | MERGE | TEMPTABLE}]
        //TODO: Add [DEFINER = { user | CURRENT_USER }]
        //TODO: add[SQL SECURITY { DEFINER | INVOKER }]
        //TODO: Add [WITH[CASCADED | LOCAL] CHECK OPTION]
        protected override void VisitAlterViewStatement(AlterViewStatement statement)
        {
            State.Write(Symbols.ALTER);

            State.Write(Symbols.VIEW);

            VisitNameToken(statement.Name);
            State.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement);
        }

        //TODO: Add Cascade or Restrict
        protected override void VisitDropViewStatement(DropViewStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.VIEW);

            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name);
        }

        protected override void VisitExecuteStatement(ExecuteStatement statement) { throw new NotImplementedException(); }

        protected override void VisitDropSchemaStatement(DropSchemaStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.SCHEMA);
            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }
            State.Write(statement.Name.GetFullName(this.IdentifierOpenQuote, this.IdentifierCloseQuote));
        }

        protected override void VisitCreateSchemaStatement(CreateSchemaStatement statement)
        {
            var schemaName = ResolveName(statement.Name);

            State.Write(Symbols.CREATE);
            State.Write(Symbols.SCHEMA);

            if (statement.CheckIfNotExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.NOT);
                State.Write(Symbols.EXISTS);
            }

            State.Write(schemaName);
        }

        private void CreateIndex(IIndex statement)
        {
            State.Write(Symbols.CREATE);

            if (statement.Unique)
            {
                State.Write(Symbols.UNIQUE);
            }

            State.Write(Symbols.INDEX);

            if (statement.CheckIfNotExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.NOT);
                State.Write(Symbols.EXISTS);
            }

            VisitToken(statement.Name);

            State.Write(Symbols.ON);

            VisitToken(statement.On);

            // columns
            VisitTokenSetInParenthesis(statement.Columns);
            VisitWhereToken(statement.Where);
        }

        private void Stringify(IStatement statement, bool needQuote = true)
        {
            if (needQuote)
            {
                Stringify(() => VisitStatement(statement));
            }
            else
            {
                StringifyWithoutQuotes(() => VisitStatement(statement));
            }
        }

        private void Stringify(Action fragment)
        {
            State.WriteBeginStringify(Symbols.SingleQuote, Symbols.SingleQuote);
            fragment();
            State.WriteEndStringify();
        }

        private void StringifyWithoutQuotes(Action fragment)
        {
            fragment();
        }

        private List<Name> PrepareDeleteUsingList(RecordsetSourceToken RecordsetSource, List<Name> usingList)
        {
            if (RecordsetSource.Source is Name)
            {
                if (!usingList.Contains((Name)RecordsetSource.Source))
                {
                    usingList.Insert(0, (Name)RecordsetSource.Source);
                }
            }
            return usingList;
        }

    }
}