// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            State.Write(TempName);
            State.WriteCRLF();
            State.Write(Symbols.BEGIN);
            State.WriteCRLF();

            bool isTop = false;
            string prefixToAlias = "alias_";
            IList<string> tempUpdateAliases = new List<string>();

            string targetTable = statement.Into.GetFullName();
            string targetAlias = (String.IsNullOrWhiteSpace(statement.Into.Alias)) ? prefixToAlias + statement.Into.LastPart : statement.Into.Alias;

            string sourceTable = ((Name)statement.Using).GetFullName();
            string sourceAlias = (String.IsNullOrWhiteSpace(((Name)statement.Using).Alias)) ? prefixToAlias + ((Name)statement.Using).LastPart : ((Name)statement.Using).Alias;

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
            State.Write(TempName);
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

        protected override void VisitDeclareStatement(DeclareStatement statement)
        {
            if (statement.Variable != null)
            {
                State.Variables.Add(statement.Variable);

                State.Write(Symbols.DECLARE);
                State.Write(statement.Variable.Name);

                VisitType(statement.Variable);

                if (statement.Initializer != null)
                {
                    State.Write(PostgrSQLSymbols.AssignValSign);
                    VisitToken(statement.Initializer);
                }
            }
        }

        protected override void VisitIfStatement(IfStatement statement)
        {
            //DO $do$ BEGIN
            State.Write(Symbols.DO);
            State.WriteCRLF();
            State.Write(TempName);
            State.WriteCRLF();
            State.Write(Symbols.BEGIN);

            OnlyIfStatement(statement);

            State.WriteStatementTerminator();

            State.Write(Symbols.END);
            State.WriteCRLF();
            State.Write(TempName);
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
                VisitTokenSetInParenthesis(statement.PrimaryKey.Columns.Select(c => c.Column));
            }
            foreach (var unique in statement.UniqueConstrains)
            {
                State.Write(Symbols.Comma);
                State.Write(Symbols.CONSTRAINT);
                VisitNameToken(unique.Name);
                State.Write(Symbols.UNIQUE);
                VisitTokenSetInParenthesis(unique.Columns.Select(c => c.Column));
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
            CreateIndex(statement);
        }

        protected override void VisitAlterIndexStatement(AlterIndexStatement statement)
        {
            VisitStatement(Sql.DropIndex(statement.Name, true));
            State.WriteStatementTerminator();
            CreateIndex(statement);
        }

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

        protected override void VisitStringifyStatement(StringifyStatement statement)
        {
            this.Stringify(statement.Content);
        }

        //There is no BREAK in PL/pgSQL.
        //EXIT terminates the loop.
        //CONTINUE continues at the next iteration of the loop.
        protected override void VisitBreakStatement(BreakStatement statement) { throw new NotImplementedException(); }

        protected override void VisitContinueStatement(ContinueStatement statement)
        {
            State.Write(Symbols.CONTINUE);
            if (!String.IsNullOrWhiteSpace(statement.Label))
            {
                State.Write(Symbols.OpenBracket);
                State.Write(statement.Label);
                State.Write(Symbols.CloseBracket);
            }
            if (statement.When != null)
            {
                State.Write(Symbols.WHEN);
                VisitToken(statement.When);
            }
        }

        protected override void VisitExitStatement(ExitStatement statement)
        {
            State.Write(Symbols.EXIT);
            if (!String.IsNullOrWhiteSpace(statement.Label))
            {
                State.Write(Symbols.OpenBracket);
                State.Write(statement.Label);
                State.Write(Symbols.CloseBracket);
            }
            if (statement.When != null)
            {
                State.Write(Symbols.WHEN);
                VisitToken(statement.When);
            }
        }

        //PL/PgSQL does not have GOTO operator.
        protected override void VisitGotoStatement(GotoStatement statement) { throw new NotImplementedException(); }

        //TODO: RETURN NEXT, RETURN QUERY
        protected override void VisitReturnStatement(ReturnStatement statement)
        {
            State.Write(Symbols.RETURN);
            if (statement.ReturnExpression != null)
            {
                VisitToken(statement.ReturnExpression);
            }
        }

        //TODO: RAISE LEVELS, CONDITION, FORMAT
        protected override void VisitThrowStatement(ThrowStatement statement)
        {
            State.Write(Symbols.RAISE);
            if (statement.Message != null)
            {
                VisitToken(statement.Message);
            }
        }

        //TODO: Add support for when / then cases
        protected override void VisitTryCatchStatement(TryCatchStatement statement)
        {
            State.Write(Symbols.BEGIN);
            State.WriteCRLF();
            VisitStatement(statement.TryStatement);
            State.WriteStatementTerminator();
            State.Write(Symbols.EXCEPTION);
            State.Write(Symbols.WHEN);
            State.Write(PostgrSQLSymbols.other);
            State.Write(PostgrSQLSymbols.THEN);
            if (statement.CatchStatement != null)
            {
                VisitStatement(statement.CatchStatement);
                State.WriteStatementTerminator();
            }
            State.Write(Symbols.END);
            State.WriteStatementTerminator();
        }

        protected override void VisitLabelStatement(LabelStatement statement)
        {
            State.Write(PostgrSQLSymbols.BEGIN_LABEL);
            State.Write(statement.Label);
            State.Write(PostgrSQLSymbols.END_LABEL);
            State.WriteCRLF();
        }

        //TODO: uncomment code
        protected override void VisitWaitforDelayStatement(WaitforDelayStatement statement)
        {
            //   string query = String.Format(PostgrSQLSymbols.DELAY_FORMAT, statement.Delay.TotalSeconds);
            //    VisitStatement(Sql.Perform(query));
        }

        //PL/PgSQL does not have Wait fo rTimeStatement operator
        protected override void VisitWaitforTimeStatement(WaitforTimeStatement statement) { throw new NotImplementedException(); }

        protected override void VisitWhileStatement(WhileStatement statement)
        {
            State.Write(Symbols.LOOP);
            State.WriteCRLF();
            if ((statement.Condition != null) && (statement.Do != null))
            {
                IfStatement condition = Sql.If(statement.Condition).Then(statement.Do).Else(Sql.Exit);
                OnlyIfStatement(condition);
            }
            State.Write(Symbols.END);
            State.Write(Symbols.LOOP);
        }

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

        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement)
        {
            AlterView(statement.Name, statement.DefinitionStatement);
        }

        protected override void VisitAlterViewStatement(AlterViewStatement statement)
        {
            AlterView(statement.Name, statement.DefinitionStatement);
        }

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

        protected override void VisitPrepareStatement(IExecutableStatement statement)
        {
            State.Write(Symbols.PREPARE);
            if (statement.Name == null)
            {
                statement.Name = Sql.Name(TempExecute);
            }
            VisitNameToken(statement.Name);
            string separator = Symbols.OpenParenthesis;

            foreach (var p in ((PrepareStatement)statement).Parameters)
            {
                State.Write(separator);
                State.Write(String.Empty);
                separator = Symbols.Comma;
                VisitType(p);
            }
            if (separator == Symbols.Comma)
            {
                State.Write(Symbols.CloseParenthesis);
            }
            State.Write(Symbols.AS);
            if (statement.Target.Target != null)
            {
                this.Stringify(statement.Target.Target, false);
            }
            State.WriteStatementTerminator();
        }

        protected override void VisitExecuteStatement(ExecuteStatement statement)
        {
            bool needDeallocate = false;

            if (statement.Name == null)
            {
                if (statement.Target.Target != null)
                {
                    PrepareStatement prepStatement = Sql.Prepare().Name(Sql.Name(TempExecute)).From(statement.Target.Target);
                    if(statement.Parameters.Count != 0)
                    {
                        foreach(Parameter p in statement.Parameters)
                        {
                            prepStatement.Parameters.Add(p);
                        }
                    }
                    VisitStatement(prepStatement);
                    State.WriteStatementTerminator();
                    statement.Name = Sql.Name(TempExecute);
                    needDeallocate = true;
                }
            }

            State.Write(Symbols.EXECUTE);
            VisitToken(statement.Name);
            if (statement.Parameters.Count != 0)
            {
                State.Write(Symbols.OpenParenthesis);
                VisitTokenSet(statement.Parameters, visitToken: parameter =>
                {
                    if (parameter.Value != null)
                    {
                        VisitValue(parameter.Value);
                    }
                    else
                    {
                        State.Write(parameter.Name);
                    }

                    State.Parameters.Add(parameter);
                });
                State.Write(Symbols.CloseParenthesis);
            }

            if (needDeallocate)
            {
                State.WriteStatementTerminator();
                VisitStatement(Sql.Deallocate(statement.Name));
            }
        }

        protected override void VisitDeallocateStatement(DeallocateStatement statement)
        {
            State.Write(Symbols.DEALLOCATE);
            VisitToken(statement.Name);
        }

        protected override void VisitPerformStatement(PerformStatement statement)
        {
            State.Write(Symbols.PERFORM);

            if (!string.IsNullOrWhiteSpace(statement.Query))
            {
                State.Write(statement.Query);
            }
            else if (statement.Target.Target != null)
            {
                VisitStatement(statement.Target.Target);
            }
        }

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

            if (!string.IsNullOrWhiteSpace(statement.Owner))
            {
                State.Write(Symbols.AUTHORIZATION);
                State.Write(this.IdentifierOpenQuote, statement.Owner, this.IdentifierCloseQuote);
            }
        }

        protected override void VisitAlterSchemaStatement(AlterSchemaStatement statement)
        {
            var schemaName = ResolveName(statement.Name);

            State.Write(Symbols.ALTER);
            State.Write(Symbols.SCHEMA);
            State.Write(schemaName);

            if (statement.NewName != null)
            {
                State.Write(Symbols.RENAME);
                State.Write(Symbols.TO);

                VisitNameToken(statement.NewName);
            }

            if (statement.NewOwner != null)
            {
                State.Write(Symbols.OWNER);
                State.Write(Symbols.TO);

                VisitNameToken(statement.NewOwner);
            }
        }

        //PL/PgSQL does not have Stored Procedure operator.
        protected override void VisitExecuteProcedureStatement(ExecuteProcedureStatement statement)
        {
            throw new NotImplementedException();
        }

        //PL/PgSQL does not have Stored Procedure operator.
        protected override void VisitCreateProcedureStatement(CreateProcedureStatement statement)
        {
            throw new NotImplementedException();
        }

        //PL/PgSQL does not have Stored Procedure operator.
        protected override void VisitAlterProcedureStatement(AlterProcedureStatement statement)
        {
            throw new NotImplementedException();
        }

        //PL/PgSQL does not have Stored Procedure operator.
        protected override void VisitDropProcedureStatement(DropProcedureStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitCreateFunctionStatement(CreateFunctionStatement statement)
        {
            CreateOrReplaceFunction(statement);
        }

        //not like in postgresql
        protected override void VisitAlterFunctionStatement(AlterFunctionStatement statement)
        {
            CreateOrReplaceFunction(statement);
        }


        protected override void VisitDropFunctionStatement(DropFunctionStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.FUNCTION);

            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name);

            State.Write(Symbols.OpenParenthesis);
            if (statement.ReturnValue != null)
            {
                VisitType(statement.ReturnValue);
            }
            State.Write(Symbols.CloseParenthesis);
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

        protected override void VisitExecuteFunctionStatement(ExecuteFunctionStatement statement)
        {
            var retVal = statement.Parameters.FirstOrDefault(p => p.Direction == ParameterDirection.ReturnValue);
            if (retVal != null)
            {
                State.Write(retVal.Name);
                State.Write(Symbols.AssignVal);
            }

            State.Write(statement.Name.GetFullNameWithoutQuotes());

            State.Write(Symbols.OpenParenthesis);

            VisitTokenSet(statement.Parameters.Where(p => p.Direction != ParameterDirection.ReturnValue),
                visitToken: parameter =>
                {
                    if (parameter.Value != null)
                    {
                        VisitValue(parameter.Value);
                    }
                    else
                    {
                        State.Write(parameter.Name);
                    }

                    State.Parameters.Add(parameter);
                });

            State.Write(Symbols.CloseParenthesis);
        }

        private void AlterView(Name tokenName, IStatement definitionStatement)
        {
            State.Write(Symbols.DO);
            State.WriteCRLF();
            State.Write(TempName);
            State.WriteCRLF();
            State.Write(Symbols.BEGIN);

            DropViewStatement dropViewStatement = Sql.DropView(tokenName, true);
            VisitStatement(dropViewStatement);
            State.WriteStatementTerminator();

            CreateViewStatement createViewStatement = Sql.CreateView(tokenName, definitionStatement);
            VisitStatement(createViewStatement);
            State.WriteStatementTerminator();

            State.Write(Symbols.END);
            State.WriteCRLF();
            State.Write(TempName);
        }

        private void OnlyIfStatement(IfStatement statement)
        {
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
            State.Write(Symbols.END);
            State.Write(Symbols.IF);
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

        private void CreateOrReplaceFunction(IProcedureStatement statement)
        {
            State.Write(Symbols.CREATE);
            State.Write(Symbols.OR);
            State.Write(Symbols.REPLACE);
            State.Write(Symbols.FUNCTION);

            VisitNameToken(statement.Name);
            var separator = String.Empty;
            string returnValueName = TempName;

            var retVal = statement.Parameters.FirstOrDefault(p => p.Direction == ParameterDirection.ReturnValue);

            State.Write(Symbols.OpenParenthesis);
            foreach (var p in statement.Parameters
                .Where(p => p.Direction != ParameterDirection.ReturnValue))
            {
                State.Write(separator);
                separator = Symbols.Comma;

                VisitNameToken(p.Name);
                VisitType(p);

                if (p.DefaultValue != null)
                {
                    State.Write(Symbols.AssignVal);
                    VisitValue(p.DefaultValue);
                }
                if ((p.Direction != 0) && (p.Direction != ParameterDirection.Input))
                {
                    State.Write(Symbols.OUTPUT);
                }
                if (p.ReadOnly)
                {
                    State.Write(Symbols.READONLY);
                }
            }

            State.Write(Symbols.CloseParenthesis);
            State.WriteCRLF();

            State.Write(Symbols.RETURNS);
            if (retVal == null)
            {
                State.Write(Symbols.VOID);
            }
            else
            {
                VisitType(retVal);
                returnValueName = retVal.Name;
            }
            State.Write(Symbols.AS);
            State.Write(returnValueName);
            State.WriteCRLF();

            if (statement.Declarations.Count != 0)
            {
                State.Write(Symbols.DECLARE);
                State.WriteCRLF();

                foreach (Parameter p in statement.Declarations)
                {
                    VisitNameToken(p.Name);
                    VisitType(p);
                    State.WriteStatementTerminator();
                }
            }

            State.Write(Symbols.BEGIN);
            State.WriteCRLF();
            VisitStatement(statement.Body);
            State.WriteStatementTerminator();
            State.Write(Symbols.END);
            State.WriteStatementTerminator();
            State.Write(String.Format(EndFunction, returnValueName));
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

        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        private string TempName = "$do$";
        private string EndFunction = "{0} LANGUAGE plpgsql;";
        private string TopAlias = "top_alias";
        private string TempExecute = "temp_execute";
    }
}