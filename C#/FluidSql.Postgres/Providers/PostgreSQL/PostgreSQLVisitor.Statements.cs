﻿// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;

namespace TTRider.FluidSql.Providers.PostgreSQL
{
    internal partial class PostgreSQLVisitor
    {
        protected override void VisitDelete(DeleteStatement statement)
        {
            State.Write(Symbols.DELETE);

            State.Write(Symbols.FROM);

            if (statement.Only)
            {
                State.Write(Symbols.ONLY);
            }

            VisitFromToken(statement.RecordsetSource);

            VisitCRUDJoinToken(statement.Joins);

            VisitUsingToken(statement.UsingList);

            VisitWhereToken(statement.Where);

            VisitCRUDJoinOnToken(statement.Joins, (statement.Where != null));

            if (statement.Top != null)
            {
                VisitTopToken(statement.RecordsetSource, statement.Top, (statement.Where != null));
            }

            VisitWhereCurrentOfToken(statement.CursorName);

            VisitReturningToken(statement.Output);
        }

        protected override void VisitUpdate(UpdateStatement statement)
        {
            State.Write(Symbols.UPDATE);

            if (statement.Only)
            {
                State.Write(Symbols.ONLY);
            }

            VisitToken(statement.Target, true);

            State.Write(Symbols.SET);

            VisitTokenSet(statement.Set);

            if (statement.RecordsetSource != null)
            {
                State.Write(Symbols.FROM);
                VisitFromToken(statement.RecordsetSource);
            }

            VisitCRUDJoinToken(statement.Joins, true);

            VisitWhereToken(statement.Where);

            VisitCRUDJoinOnToken(statement.Joins, (statement.Where != null));

            VisitWhereCurrentOfToken(statement.CursorName);

            VisitReturningToken(statement.Output, statement.OutputInto);
        }

        protected override void VisitInsert(InsertStatement statement)
        {
            State.Write(Symbols.INSERT);

            State.Write(Symbols.INTO);

            VisitToken(statement.Into, true);

            if (statement.DefaultValues)
            {
                State.Write(Symbols.DEFAULT);
                State.Write(Symbols.VALUES);
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

            if (statement.Conflict.HasValue)
            {
                State.Write(Symbols.ON);
                State.Write(Symbols.CONFLICT);
                switch (statement.Conflict.Value)
                {
                    case OnConflict.Ignore:
                        State.Write(Symbols.DO);
                        State.Write(Symbols.NOTHING);
                        break;
                }
            }

            VisitReturningToken(statement.Output, statement.OutputInto);
        }

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

            VisitIntoToken(statement.Into);

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

            VisitTopToken(statement);

            if (statement.Offset != null)
            {
                State.Write(Symbols.OFFSET);
                VisitToken(statement.Offset);
            }
        }

        protected override void VisitExceptStatement(ExceptStatement statement)
        {
            VisitStatement(statement.First);

            State.Write(Symbols.EXCEPT);

            if (statement.All)
            {
                State.Write(Symbols.ALL);
            }

            VisitStatement(statement.Second);
        }

        protected override void VisitIntersectStatement(IntersectStatement statement)
        {
            VisitStatement(statement.First);
            State.Write(Symbols.INTERSECT);

            if (statement.All)
            {
                State.Write(Symbols.ALL);
            }

            VisitStatement(statement.Second);
        }

        protected override void VisitMerge(MergeStatement statement)
        {
            State.Write(Symbols.DO);
            State.WriteCRLF();
            State.Write(TempFunctionName);
            State.WriteCRLF();
            State.Write(Symbols.BEGIN);
            State.WriteCRLF();

            bool isTop = false;
            string prefixToAlias = "alias_";
            IList<string> tempUpdateAliases = new List<string>();

            string targetTable = statement.Into.GetFullName();
            string targetAlias = (String.IsNullOrEmpty(statement.Into.Alias)) ? prefixToAlias + statement.Into.LastPart : statement.Into.Alias;

            string sourceTable = ((Name)statement.Using).GetFullName();
            string sourceAlias = (String.IsNullOrEmpty(((Name)statement.Using).Alias)) ? prefixToAlias + ((Name)statement.Using).LastPart : ((Name)statement.Using).Alias;

            string targetColumnOn = string.Empty;
            string sourceColumnOn = string.Empty;

            string firstOn = ((Name)((BinaryToken)statement.On).First).FirstPart;

            if ((firstOn).Equals(targetTable) || (firstOn).Equals(targetAlias))
            {
                targetColumnOn = ((Name)((BinaryToken)statement.On).First).LastPart;
                sourceColumnOn = ((Name)((BinaryToken)statement.On).Second).LastPart;
            }
            else
            {
                sourceColumnOn = ((Name)((BinaryToken)statement.On).First).LastPart;
                targetColumnOn = ((Name)((BinaryToken)statement.On).Second).LastPart;
            }

            ((BinaryToken)statement.On).First = Sql.Name(targetAlias, targetColumnOn);
            ((BinaryToken)statement.On).Second = Sql.Name(sourceAlias, sourceColumnOn);

            if (statement.Top != null)
            {
                CreateTableStatement createTable = Sql.CreateTemporaryTable(Sql.Name(TopAlias), true).As(Sql.Select.Output(Sql.Name(targetColumnOn)).From(Sql.Name(targetTable)).Top(((int)((Scalar)statement.Top.Value).Value), statement.Top.Percent));
                isTop = true;
                VisitStatement(createTable);
                State.WriteStatementTerminator();
            }

            VisitWhenMatchedUpdateToken(statement
                , tempUpdateAliases
                , targetAlias
                , targetTable
                , targetColumnOn
                , sourceAlias
                , sourceTable
                , sourceColumnOn
                , isTop);

            VisitWhenMatchedDeleteToken(statement
                , targetAlias
                , targetTable
                , targetColumnOn
                , sourceAlias
                , sourceTable
                , sourceColumnOn
                , isTop);

            VisitWhenNotMatchedBySourceToken(statement
                 , targetAlias
                 , targetTable
                 , targetColumnOn
                 , sourceAlias
                 , sourceTable
                 , sourceColumnOn
                 , isTop);

            VisitWhenNotMatchedThenInsertToken(statement
                 , tempUpdateAliases
                 , targetAlias
                 , targetColumnOn
                 , targetTable
                 , sourceAlias
                 , sourceTable
                 , sourceColumnOn);

            if (tempUpdateAliases.Count != 0)
            {
                foreach (string tempTable in tempUpdateAliases)
                {
                    DropTableStatement dropTable = Sql.DropTemporaryTable(tempTable, true);
                    VisitStatement(dropTable);
                    State.WriteStatementTerminator();
                }
            }
            if (isTop)
            {
                DropTableStatement dropTable = Sql.DropTemporaryTable(TopAlias, true);
                VisitStatement(dropTable);
                State.WriteStatementTerminator();
            }

            State.Write(Symbols.END);
            State.WriteStatementTerminator();
            State.Write(TempFunctionName);
        }

        protected override void VisitSet(SetStatement statement) { throw new NotImplementedException(); }

        protected override void VisitBeginTransaction(BeginTransactionStatement statement)
        {
            {
                State.Write(Symbols.BEGIN);
                State.Write(Symbols.TRANSACTION);
                VisitTransactionName(statement);

                if (statement.IsolationLevel != null)
                {
                    State.Write(Symbols.ISOLATION);
                    State.Write(Symbols.LEVEL);
                    switch (statement.IsolationLevel.Value)
                    {
                        case IsolationLevelType.Serializable:
                            State.Write(Symbols.SERIALIZABLE);
                            break;
                        case IsolationLevelType.RepeatableRead:
                            State.Write(Symbols.REPEATABLE);
                            State.Write(Symbols.READ);
                            break;
                        case IsolationLevelType.ReadCommited:
                            State.Write(Symbols.READ);
                            State.Write(Symbols.COMMITED);
                            break;
                        case IsolationLevelType.ReadUnCommited:
                            State.Write(Symbols.READ);
                            State.Write(Symbols.UNCOMMITED);
                            break;
                    }
                }
                if (statement.AccessType != null)
                {
                    switch (statement.AccessType.Value)
                    {
                        case TransactionAccessType.ReadOnly:
                            State.Write(Symbols.READ);
                            State.Write(Symbols.ONLY);
                            break;
                        case TransactionAccessType.ReadWrite:
                            State.Write(Symbols.READ);
                            State.Write(Symbols.WRITE);
                            break;
                    }
                }

            }
        }

        protected override void VisitCommitTransaction(CommitTransactionStatement statement)
        {
            State.Write(Symbols.COMMIT);
            State.Write(Symbols.TRANSACTION);
            VisitTransactionName(statement);
        }

        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement)
        {
            State.Write(Symbols.ROLLBACK);
            State.Write(Symbols.TRANSACTION);

            if (statement.Name != null || statement.Parameter != null)
            {
                State.Write(Symbols.TO);
                State.Write(Symbols.SAVEPOINT);
                VisitTransactionName(statement);
            }
        }

        protected override void VisitSaveTransaction(SaveTransactionStatement statement)
        {
            State.Write(Symbols.SAVEPOINT);
            VisitTransactionName(statement);
        }

        protected override void VisitDeclareStatement(DeclareStatement statement) { throw new NotImplementedException(); }

        protected override void VisitIfStatement(IfStatement statement)
        {
            //DO $do$ BEGIN
            State.Write(Symbols.DO);
            State.WriteCRLF();
            State.Write(TempFunctionName);
            State.WriteCRLF();
            State.Write(Symbols.BEGIN);

            State.Write(Symbols.IF);
            VisitToken(statement.Condition);

            if (statement.Then != null)
            {
                State.Write(Symbols.THEN);
                VisitStatementsStatement(statement.Then);
            }
            if (statement.Else != null)
            {
                State.Write(Symbols.ELSE);
                VisitStatementsStatement(statement.Else);
            }
            //END IF; END $do$
            State.Write(Symbols.END);
            State.Write(Symbols.IF);
            State.WriteStatementTerminator();

            State.Write(Symbols.END);
            State.WriteCRLF();
            State.Write(TempFunctionName);
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

            if (statement.AsSelectStatement != null)
            {
                State.Write(Symbols.AS);
                State.Write(Symbols.OpenParenthesis);
                VisitStatement(statement.AsSelectStatement);
            }
            else
            {
                var separator = Symbols.OpenParenthesis;
                if (statement.Columns.Count == 0)
                {
                    State.Write(separator);
                }
                else
                {
                    foreach (var column in statement.Columns)
                    {
                        State.Write(separator);
                        separator = Symbols.Comma;
                        VisitNameToken(column.Name);

                        if (column.Identity.On)
                        {
                            column.DbType = CommonDbType.Serial;
                        }

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
                    }
                }
            }
            if (statement.PrimaryKey != null)
            {
                if (statement.PrimaryKey.Name != null)
                {
                    State.Write(Symbols.Comma);
                    State.Write(Symbols.CONSTRAINT);
                    VisitNameToken(statement.PrimaryKey.Name);
                }
                State.Write(Symbols.PRIMARY);
                State.Write(Symbols.KEY);
                VisitTokenSetInParenthesis(statement.PrimaryKey.ColumnsName);
            }
            foreach (var unique in statement.UniqueConstrains)
            {
                State.Write(Symbols.Comma);
                State.Write(Symbols.CONSTRAINT);
                VisitNameToken(unique.Name);
                State.Write(Symbols.UNIQUE);
                VisitTokenSetInParenthesis(unique.ColumnsName);
            }

            State.Write(Symbols.CloseParenthesis);

            if (statement.Indicies.Count > 0)
            {
                foreach (var createIndexStatement in statement.Indicies)
                {
                    State.WriteStatementTerminator();
                    VisitCreateIndexStatement(createIndexStatement);
                }
            }
        }

        protected override void VisitDropTableStatement(DropTableStatement statement)
        {
            //DROP TABLE [ IF EXISTS ] name [, ...] [ CASCADE | RESTRICT ]
            State.Write(Symbols.DROP);
            State.Write(Symbols.TABLE);
            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }
            State.Write(statement.Name.GetFullName(this.IdentifierOpenQuote, this.IdentifierCloseQuote));
            if (statement.IsCascade.HasValue)
            {
                if (statement.IsCascade.Value)
                {
                    State.Write(Symbols.CASCADE);
                }
                else
                {
                    State.Write(Symbols.RESTRICT);
                }
            }
        }

        protected override void VisitCreateIndexStatement(CreateIndexStatement statement)
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

        protected override void VisitAlterIndexStatement(AlterIndexStatement statement) { throw new NotImplementedException(); }

        protected override void VisitDropIndexStatement(DropIndexStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.INDEX);

            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }

            if (statement.On != null)
            {
                var name = Sql.Name(statement.On.FirstPart, statement.Name.LastPart);
                VisitToken(name);
            }
            else
            {
                VisitToken(statement.Name);
            }
        }

        protected override void VisitCommentStatement(CommentStatement statement) { throw new NotImplementedException(); }
        protected override void VisitStringifyStatement(StringifyStatement statement) { throw new NotImplementedException(); }
        protected override void VisitSnippetStatement(SnippetStatement statement) { throw new NotImplementedException(); }
        protected override void VisitBreakStatement(BreakStatement statement) { throw new NotImplementedException(); }
        protected override void VisitContinueStatement(ContinueStatement statement) { throw new NotImplementedException(); }
        protected override void VisitGotoStatement(GotoStatement statement) { throw new NotImplementedException(); }
        protected override void VisitReturnStatement(ReturnStatement statement) { throw new NotImplementedException(); }
        protected override void VisitThrowStatement(ThrowStatement statement) { throw new NotImplementedException(); }
        protected override void VisitTryCatchStatement(TryCatchStatement statement) { throw new NotImplementedException(); }
        protected override void VisitLabelStatement(LabelStatement statement) { throw new NotImplementedException(); }
        protected override void VisitWaitforDelayStatement(WaitforDelayStatement statement) { throw new NotImplementedException(); }
        protected override void VisitWaitforTimeStatement(WaitforTimeStatement statement) { throw new NotImplementedException(); }
        protected override void VisitWhileStatement(WhileStatement statement) { throw new NotImplementedException(); }

        protected override void VisitCreateViewStatement(CreateViewStatement statement)
        {
            State.Write(Symbols.CREATE);
            if (statement.IsTemporary)
            {
                State.Write(Symbols.TEMPORARY);
            }
            State.Write(Symbols.VIEW);

            if (statement.CheckIfNotExists)
            {
                throw new NotImplementedException();
            }
            VisitNameToken(statement.Name);
            State.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement);
        }

        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement) { throw new NotImplementedException(); }
        protected override void VisitAlterViewStatement(AlterViewStatement statement) { throw new NotImplementedException(); }
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
        protected override void VisitDropSchemaStatement(DropSchemaStatement statement) { throw new NotImplementedException(); }
        protected override void VisitCreateSchemaStatement(CreateSchemaStatement statement) { throw new NotImplementedException(); }

        private string TempFunctionName = "$do$";
        private string TopAlias = "top_alias";
    }
}