// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {
        protected virtual void VisitStatementsStatement(StatementsStatement statement)
        {
            foreach (var subStatement in statement.Statements)
            {
                VisitStatement(subStatement);
                State.WriteStatementTerminator();
            }
        }

        protected virtual void VisitCommentStatement(CommentStatement statement)
        {
            State.Write(this.CommentOpenQuote);
            VisitStatement(statement.Content);
            State.Write(this.CommentCloseQuote);
        }

        protected virtual void VisitSnippetStatement(SnippetStatement statement)
        {
            var value = statement.GetValue(this.SupportedDialects);

            if (statement.Arguments.Count > 0)
            {
                var index = 0;
                SnippetArgumentRegex.Replace(value, match =>
                {
                    if (match.Index > index)
                    {
                        State.Write(value.Substring(index, match.Index - index));
                    }
                    var argIndex = int.Parse(match.Groups["index"].Value);
                    VisitToken(statement.Arguments[argIndex]);
                    index = match.Index + match.Length;
                    return "";
                });
                State.Write(value.Substring(index));
            }
            else
            {
                State.Write(value);
            }
        }

        protected virtual void VisitUnionStatement(UnionStatement statement)
        {
            VisitStatement(statement.First);

            State.Write(Symbols.UNION);
            if (statement.All)
            {
                State.Write(Symbols.ALL);
            }

            VisitStatement(statement.Second);
        }

        protected virtual void VisitExceptStatement(ExceptStatement statement)
        {
            VisitStatement(statement.First);

            State.Write(Symbols.EXCEPT);

            VisitStatement(statement.Second);
        }

        protected virtual void VisitIntersectStatement(IntersectStatement statement)
        {
            VisitStatement(statement.First);
            State.Write(Symbols.INTERSECT);
            VisitStatement(statement.Second);
        }


        protected abstract void VisitDelete(DeleteStatement statement);
        protected abstract void VisitUpdate(UpdateStatement statement);
        protected abstract void VisitInsert(InsertStatement statement);
        protected abstract void VisitSelect(SelectStatement statement);
        protected abstract void VisitMerge(MergeStatement statement);
        protected abstract void VisitSet(SetStatement statement);
        protected abstract void VisitBeginTransaction(BeginTransactionStatement statement);
        protected abstract void VisitCommitTransaction(CommitTransactionStatement statement);
        protected abstract void VisitRollbackTransaction(RollbackTransactionStatement statement);
        protected abstract void VisitSaveTransaction(SaveTransactionStatement statement);
        protected abstract void VisitDeclareStatement(DeclareStatement statement);
        protected abstract void VisitIfStatement(IfStatement statement);
        protected abstract void VisitCreateTableStatement(CreateTableStatement statement);
        protected abstract void VisitDropTableStatement(DropTableStatement statement);
        protected abstract void VisitCreateIndexStatement(CreateIndexStatement statement);
        protected abstract void VisitAlterIndexStatement(AlterIndexStatement statement);
        protected abstract void VisitDropIndexStatement(DropIndexStatement statement);
        protected abstract void VisitStringifyStatement(StringifyStatement statement);
        protected abstract void VisitBreakStatement(BreakStatement statement);
        protected abstract void VisitContinueStatement(ContinueStatement statement);
        protected abstract void VisitExitStatement(ExitStatement statement);
        protected abstract void VisitGotoStatement(GotoStatement statement);
        protected abstract void VisitReturnStatement(ReturnStatement statement);
        protected abstract void VisitThrowStatement(ThrowStatement statement);
        protected abstract void VisitTryCatchStatement(TryCatchStatement statement);
        protected abstract void VisitLabelStatement(LabelStatement statement);
        protected abstract void VisitWaitforDelayStatement(WaitforDelayStatement statement);
        protected abstract void VisitWaitforTimeStatement(WaitforTimeStatement statement);
        protected abstract void VisitWhileStatement(WhileStatement statement);
        protected abstract void VisitCreateViewStatement(CreateViewStatement statement);
        protected abstract void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement);
        protected abstract void VisitAlterViewStatement(AlterViewStatement statement);
        protected abstract void VisitDropViewStatement(DropViewStatement statement);
        protected abstract void VisitPerformStatement(PerformStatement statement);
        protected abstract void VisitDeallocateStatement(DeallocateStatement statement);
        protected abstract void VisitExecuteStatement(ExecuteStatement statement);
        protected abstract void VisitPrepareStatement(IExecutableStatement statement);
        protected abstract void VisitExecuteProcedureStatement(ExecuteProcedureStatement statement);
        protected abstract void VisitCreateSchemaStatement(CreateSchemaStatement statement);
        protected abstract void VisitDropSchemaStatement(DropSchemaStatement statement);
        protected abstract void VisitAlterSchemaStatement(AlterSchemaStatement statement);
        protected abstract void VisitCreateProcedureStatement(CreateProcedureStatement statement);
        protected abstract void VisitDropProcedureStatement(DropProcedureStatement statement);
        protected abstract void VisitAlterProcedureStatement(AlterProcedureStatement statement);
        protected abstract void VisitCreateFunctionStatement(CreateFunctionStatement statement);
        protected abstract void VisitAlterFunctionStatement(AlterFunctionStatement statement);
        protected abstract void VisitDropFunctionStatement(DropFunctionStatement statement);
        protected abstract void VisitExecuteFunctionStatement(ExecuteFunctionStatement statement);
    }
}