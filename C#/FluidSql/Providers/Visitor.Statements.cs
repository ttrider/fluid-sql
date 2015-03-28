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
            State.Write(statement.Value);
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




        protected virtual void VisitDelete(DeleteStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitUpdate(UpdateStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitInsert(InsertStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitSelect(SelectStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitMerge(MergeStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitSet(SetStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitBeginTransaction(BeginTransactionStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitCommitTransaction(CommitTransactionStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitRollbackTransaction(RollbackTransactionStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitSaveTransaction(SaveTransactionStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitDeclareStatement(DeclareStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitIfStatement(IfStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitCreateTableStatement(CreateTableStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitDropTableStatement(DropTableStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitCreateIndexStatement(CreateIndexStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitAlterIndexStatement(AlterIndexStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitDropIndexStatement(DropIndexStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitStringifyStatement(StringifyStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitBreakStatement(BreakStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitContinueStatement(ContinueStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitGotoStatement(GotoStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitReturnStatement(ReturnStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitThrowStatement(ThrowStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitTryCatchStatement(TryCatchStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitLabelStatement(LabelStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitWaitforDelayStatement(WaitforDelayStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitWaitforTimeStatement(WaitforTimeStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitWhileStatement(WhileStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitCreateViewStatement(CreateViewStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitAlterViewStatement(AlterViewStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitDropViewStatement(DropViewStatement statement){ throw new NotImplementedException();}
        protected virtual void VisitExecuteStatement(ExecuteStatement statement){ throw new NotImplementedException();}



    }
}