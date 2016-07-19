// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
                if ((statement.RecordsetSource.Source is ExpressionToken) && (!String.IsNullOrWhiteSpace(((ExpressionToken)statement.RecordsetSource.Source).Alias)))
                {
                    VisitNameToken(Sql.Name(((ExpressionToken)statement.RecordsetSource.Source).Alias));
                }
                State.Write(Symbols.FROM);
                VisitFromToken(statement.RecordsetSource);
            }
            else
            {
                State.Write(Symbols.FROM);
                if ((statement.RecordsetSource.Source is ExpressionToken) && (!String.IsNullOrWhiteSpace(((ExpressionToken)statement.RecordsetSource.Source).Alias)))
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

            if (statement.RecordsetSource != null)
            {
                if (statement.RecordsetSource.Source is Name)
                {
                    State.Write(Symbols.Comma);
                    VisitToken(statement.RecordsetSource);
                }
            }

            VisitJoin(statement.Joins);

            State.Write(Symbols.SET);

            VisitTokenSet(statement.Set);

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
        //TODO: MySQL Server doesn't support the SELECT ... INTO TABLE Sybase SQL extension
        //TODO: MySQL doesn't support the WITH clause
        protected override void VisitSelect(SelectStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions, true);

            if (statement.Joins.Count == 1)
            {
                if ((statement.Joins[0]).Type == Joins.FullOuter)
                {
                    statement.Joins[0].Type = Joins.LeftOuter;
                    VisitStatement(statement);

                    State.Write(Symbols.UNION);
                    State.Write(Symbols.ALL);

                    statement.Joins[0].Type = Joins.RightOuter;
                    VisitStatement(statement);

                    return;
                }
            }
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

            VisitTopToken(statement.Top);

            if (statement.Offset != null)
            {
                State.Write(Symbols.OFFSET);
                VisitToken(statement.Offset);
            }
        }

        protected override void VisitMerge(MergeStatement statement)
        {
            VisitStatement(Sql.BeginTransaction());
            State.WriteStatementTerminator();

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

            VisitStatement(Sql.CommitTransaction());
        }

        protected override void VisitSet(SetStatement statement)
        {
            State.Write(Symbols.SET);
            if (statement.Assign != null)
            {
                VisitToken(statement.Assign);
            }
        }

        protected override void VisitIntersectStatement(IntersectStatement statement)
        {
            VisitCorrelationStatement(statement, "EXISTS");
        }

        protected override void VisitExceptStatement(ExceptStatement statement)
        {
            VisitCorrelationStatement(statement, "NOT EXISTS");
        }

        //TODO: Add transaction characteristic
        protected override void VisitBeginTransaction(BeginTransactionStatement statement)
        {
            State.Write(Symbols.START);
            State.Write(Symbols.TRANSACTION);
        }

        //TODO: Add [WORK] [AND [NO] CHAIN] [[NO] RELEASE]
        protected override void VisitCommitTransaction(CommitTransactionStatement statement)
        {
            State.Write(Symbols.COMMIT);
        }

        //TODO: Add [WORK] [AND [NO] CHAIN] [[NO] RELEASE]
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
            if (statement.AsSelectStatement != null)
            {
                State.Write(Symbols.AS);
                State.Write(Symbols.OpenParenthesis);
                VisitStatement(statement.AsSelectStatement);
            }
            else
            {
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

                if (statement.PrimaryKey != null)
                {
                    State.Write(Symbols.Comma);
                    if (!statement.IsTableVariable)
                    {
                        State.Write(Symbols.CONSTRAINT);
                        VisitNameToken(statement.PrimaryKey.Name);
                    }

                    State.Write(Symbols.PRIMARY);
                    State.Write(Symbols.KEY);
                    VisitTokenSetInParenthesis(statement.PrimaryKey.Columns);
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
            if (!String.IsNullOrWhiteSpace(statement.Label))
            {
                State.Write(statement.Label);
            }
        }

        protected override void VisitContinueStatement(ContinueStatement statement)
        {
            State.Write(MySqlSymbols.ITERATE);
            if (!String.IsNullOrWhiteSpace(statement.Label))
            {
                State.Write(statement.Label);
            }
        }

        //There is no GOTO statement in MySQL
        protected override void VisitGotoStatement(GotoStatement statement) { throw new NotImplementedException(); }

        //TODO: check return a variable
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

        protected override void VisitWaitforDelayStatement(WaitforDelayStatement statement)
        {
            State.Write(Symbols.WAITFOR);
            State.Write(Symbols.DELAY);
            State.Write(LiteralOpenQuote, statement.Delay.ToString("HH:mm:ss"), LiteralCloseQuote);
        }

        //There are no any appropriate function in SQL.cs
        protected override void VisitWaitforTimeStatement(WaitforTimeStatement statement) { throw new NotImplementedException(); }

        protected override void VisitWhileStatement(WhileStatement statement)
        {
            if (statement.Condition != null)
            {
                State.Write(Symbols.WHILE);
                VisitToken(statement.Condition);

                if (statement.Do != null)
                {
                    State.WriteCRLF();
                    State.Write(Symbols.DO);
                    State.WriteCRLF();

                    VisitStatement(statement.Do);
                    State.WriteStatementTerminator();

                    State.Write(Symbols.END);
                    State.Write(Symbols.WHILE);
                    State.WriteStatementTerminator();
                }
            }
        }

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

        protected override void VisitExecuteStatement(ExecuteStatement statement)
        {
            bool needDeallocate = false;

            if (statement.Name == null)
            {
                if (statement.Target.Target != null)
                {
                    PrepareStatement prepStatement = Sql.Prepare().Name(Sql.Name(TempExecute)).From(statement.Target.Target);
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
                State.Write(Symbols.USING);
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
            State.Write(Symbols.PREPARE);
            VisitToken(statement.Name);
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

        protected override void VisitCreateProcedureStatement(CreateProcedureStatement statement)
        {
            VisitProcedureAndBodyParameters(statement, true, statement.CheckIfNotExists);
        }

        protected override void VisitDropProcedureStatement(DropProcedureStatement statement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.PROCEDURE);
            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
            }
            VisitNameToken(statement.Name);
        }

        protected override void VisitAlterProcedureStatement(AlterProcedureStatement statement)
        {
            VisitProcedureAndBodyParameters(statement, true, true);
        }

        protected override void VisitExecuteProcedureStatement(ExecuteProcedureStatement statement)
        {
            State.Write(Symbols.CALL);

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

        protected override void VisitCreateFunctionStatement(CreateFunctionStatement statement)
        {
            VisitProcedureAndBodyParameters(statement, false, statement.CheckIfNotExists);
        }

        protected override void VisitAlterFunctionStatement(AlterFunctionStatement statement)
        {
            VisitProcedureAndBodyParameters(statement, false, true);
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
        }

        protected override void VisitExecuteFunctionStatement(ExecuteFunctionStatement statement)
        {
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

        protected override void VisitPrepareStatement(IExecutableStatement statement)
        {
            State.Write(Symbols.PREPARE);
            VisitNameToken(statement.Name);
            State.Write(Symbols.FROM);

            if (statement.Target.Name != null)
            {
                VisitNameToken(statement.Target.Name);
            }
            else if (statement.Target.Target != null)
            {
                State.Write(String.Empty);
                this.Stringify(statement.Target.Target, true);
            }
            State.WriteStatementTerminator();
        }

        private void VisitProcedureAndBodyParameters(IProcedureStatement statement, bool isProcedure, bool checkIfNotExists)
        {
            if (checkIfNotExists)
            {
                if (isProcedure)
                {
                    VisitStatement(Sql.DropProcedure(statement.Name, true));
                }
                else
                {
                    VisitStatement(Sql.DropFunction(statement.Name, true));
                }
                State.WriteStatementTerminator();
            }
            State.Write(MySqlSymbols.DELIMITER);
            State.Write(DelimiterSymbol);
            State.WriteCRLF();
            State.Write(Symbols.CREATE);

            if (isProcedure)
            {
                State.Write(Symbols.PROCEDURE);
            }
            else
            {
                State.Write(Symbols.FUNCTION);
            }
            VisitNameToken(statement.Name);
            State.WriteCRLF();


            if (!isProcedure)
            {
                VisitProcedureAndFunctionParameters(statement, false);
                VisitFunctionReturnParameters(statement);
            }
            else
            {
                VisitProcedureAndFunctionParameters(statement);
            }

            State.Write(Symbols.BEGIN);
            State.WriteCRLF();
            VisitStatement(statement.Body);
            State.WriteStatementTerminator();
            State.Write(Symbols.END);

            State.Write(String.Empty);
            State.Write(DelimiterSymbol);
            State.WriteStatementTerminator();
            State.Write(MySqlSymbols.DELIMITER);
            State.Write(String.Empty);
            State.WriteStatementTerminator();
        }

        private void VisitProcedureAndFunctionParameters(IProcedureStatement s, bool IsProcedure = true)
        {
            var separator = Symbols.OpenParenthesis;
            State.Write(separator);
            foreach (var p in s.Parameters
                .Where(p => p.Direction != ParameterDirection.ReturnValue))
            {
                if (separator == Symbols.Comma)
                {
                    State.Write(separator);
                }
                State.WriteCRLF();
                separator = Symbols.Comma;

                if (IsProcedure)
                {
                    switch (p.Direction)
                    {
                        case ParameterDirection.Input:
                            State.Write(Symbols.IN);
                            break;
                        case ParameterDirection.InputOutput:
                            State.Write(Symbols.INOUT);
                            break;
                        case ParameterDirection.Output:
                            State.Write(Symbols.OUT);
                            break;
                        default:
                            State.Write(Symbols.IN);
                            break;

                    }
                }
                VisitNameToken(p.Name);
                VisitType(p);
            }
            if (separator == Symbols.Comma)
            {
                State.WriteCRLF();
            }
            State.Write(Symbols.CloseParenthesis);
            State.WriteCRLF();
        }

        private void VisitFunctionReturnParameters(IProcedureStatement s)
        {
            var retVal = s.Parameters.FirstOrDefault(p => p.Direction == ParameterDirection.ReturnValue);

            State.Write(Symbols.RETURNS);
            if (retVal == null)
            {
                State.Write(Symbols.VOID);
            }
            else
            {
                VisitType(retVal);
            }
            State.WriteCRLF();
        }

        private void VisitFunctionParametersAndBody(IProcedureStatement s)
        {
            State.Write(Symbols.FUNCTION);

            VisitNameToken(s.Name);
            State.WriteCRLF();

            var separator = Symbols.OpenParenthesis;
            Parameter returnParam = s.Parameters
                .Where(p => p.Direction == ParameterDirection.ReturnValue).FirstOrDefault();

            foreach (var p in s.Parameters
                .Where(p => p.Direction != ParameterDirection.ReturnValue))
            {
                State.Write(separator);
                State.WriteCRLF();
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
            if (separator == Symbols.Comma)
            {
                State.WriteCRLF();
                State.Write(Symbols.CloseParenthesis);
                State.WriteCRLF();
            }
            if (returnParam != null)
            {
                State.Write(Symbols.RETURNS);
                VisitNameToken(returnParam.Name);
                VisitType(returnParam);
                State.WriteCRLF();
            }

            if (s.Recompile)
            {
                State.Write(Symbols.WITH, Symbols.RECOMPILE);
                State.WriteCRLF();
            }

            State.Write(Symbols.AS);
            State.WriteCRLF();
            State.Write(Symbols.BEGIN);
            State.WriteStatementTerminator();
            VisitStatement(s.Body);
            State.Write(Symbols.END);
            State.WriteStatementTerminator();
        }

        private void VisitConditional(bool doCheck, Name name, string objectType, bool inverse, Action isNullAction,
            Action isNotNullAction = null)
        {
            if (doCheck)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.OBJECT_ID);
                State.Write(Symbols.OpenParenthesis);
                State.Write(LiteralOpenQuote, ResolveName(name), LiteralCloseQuote);
                State.Write(Symbols.Comma);
                State.Write(LiteralOpenQuote, objectType, LiteralCloseQuote);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.IS);
                if (inverse)
                {
                    State.Write(Symbols.NOT);
                }
                State.Write(Symbols.NULL);
                State.WriteCRLF();
                State.Write(Symbols.BEGIN);
                State.WriteStatementTerminator();

                State.Write(Symbols.EXEC);
                State.Write(Symbols.OpenParenthesis);
                State.WriteBeginStringify(LiteralOpenQuote, LiteralCloseQuote);

                isNullAction();

                State.WriteEndStringify();
                State.Write(Symbols.CloseParenthesis);
                State.WriteStatementTerminator();

                State.Write(Symbols.END);
                State.WriteStatementTerminator();

                if (isNotNullAction != null)
                {
                    State.Write(Symbols.ELSE);
                    State.Write(Symbols.BEGIN);
                    State.WriteStatementTerminator();

                    State.Write(Symbols.EXEC);
                    State.Write(Symbols.OpenParenthesis);
                    State.WriteBeginStringify(LiteralOpenQuote, LiteralCloseQuote);

                    isNotNullAction();

                    State.WriteEndStringify();
                    State.Write(Symbols.CloseParenthesis);
                    State.WriteStatementTerminator();


                    State.Write(Symbols.END);
                    State.WriteStatementTerminator();
                }
            }
            else
            {
                isNullAction();
            }
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

        private void VisitCorrelationStatement(CorrelationStatement statement, string functionName)
        {
            SelectStatement statement1 = (SelectStatement)statement.First;
            SelectStatement statement2 = (SelectStatement)statement.Second;

            if (((statement1.From.Count != 1) && (!(statement1.From[0].Source is Name))) ||
                ((statement2.From.Count != 1) && (!(statement2.From[0].Source is Name))))
            {
                throw new NotImplementedException();
            }
            else if ((statement1.Output.Count != statement2.Output.Count) && (statement1.Output.Count == 0))
            {
                throw new NotImplementedException();
            }

            string table1 = ((Name)(statement1.From[0].Source)).LastPart;
            string table2 = ((Name)(statement2.From[0].Source)).LastPart;

            ExpressionToken whereExpression = null;
            List<ExpressionToken> expressions1 = new List<ExpressionToken>();
            List<ExpressionToken> expressions2 = new List<ExpressionToken>();

            for (int i = 0; i < statement1.Output.Count; i++)
            {
                if ((!(statement1.Output[i] is Name)) && (!(statement2.Output[i] is Name)))
                {
                    throw new NotImplementedException();
                }
                expressions1.Add(Sql.Name(table1, ((Name)(statement1.Output[i])).LastPart));
                expressions2.Add(Sql.Name(table2, ((Name)(statement2.Output[i])).LastPart));
                if (whereExpression == null)
                {
                    whereExpression = expressions1[i].IsEqual(expressions2[i]);
                }
                else
                {
                    whereExpression = whereExpression.And(expressions1[i].IsEqual(expressions2[i]));
                }
            }
            if (statement2.Where != null)
            {
                whereExpression = whereExpression.And(AddPrefixToExpressionToken(((ExpressionToken)statement2.Where), table2));
            }

            ExpressionToken tempExpression = Sql.Function(functionName, Sql.Select.From(table2).Where(whereExpression));
            if (statement1.Where != null)
            {
                tempExpression = tempExpression.And(AddPrefixToExpressionToken(((ExpressionToken)statement1.Where), table1));
            }            

            SelectStatement correlationStatement =
                Sql.Select.Output(expressions1).From(table1)
                .Where(tempExpression);
            VisitStatement(correlationStatement);
        }

        private string TopAlias = "top_alias";
        private string TempExecute = "temp_execute";
        private string DelimiterSymbol = "$$";

    }
}
