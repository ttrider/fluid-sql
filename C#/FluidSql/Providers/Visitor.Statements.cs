using System;
using System.Collections.Generic;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {


        protected virtual void VisitStatementsStatement(StatementsStatement statement, VisitorState state)
        {
            foreach (var subStatement in statement.Statements)
            {
                VisitStatement(subStatement, state);
                state.WriteStatementTerminator();
            }
        }
        protected virtual void VisitCommentStatement(CommentStatement statement, VisitorState state)
        {
            state.Write(this.CommentOpenQuote);
            VisitStatement(statement.Content, state);
            state.Write(this.CommentCloseQuote);
        }
        protected virtual void VisitSnippetStatement(SnippetStatement statement, VisitorState state)
        {
            state.Write(statement.Value);
        }
        protected virtual void VisitUnionStatement(UnionStatement statement, VisitorState state)
        {
            VisitStatement(statement.First, state);

            state.Write(Symbols.UNION);
            if (statement.All)
            {
                state.Write(Symbols.ALL);
            }

            VisitStatement(statement.Second, state);
        }
        protected virtual void VisitExceptStatement(ExceptStatement statement, VisitorState state)
        {
            VisitStatement(statement.First, state);

            state.Write(Symbols.EXCEPT);

            VisitStatement(statement.Second, state);
        }
        protected virtual void VisitIntersectStatement(IntersectStatement statement, VisitorState state)
        {
            VisitStatement(statement.First, state);
            state.Write(Symbols.INTERSECT);
            VisitStatement(statement.Second, state);
        }




        protected virtual void VisitDelete(DeleteStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitUpdate(UpdateStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitInsert(InsertStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitSelect(SelectStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitMerge(MergeStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitSet(SetStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitBeginTransaction(BeginTransactionStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitCommitTransaction(CommitTransactionStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitRollbackTransaction(RollbackTransactionStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitSaveTransaction(SaveTransactionStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitDeclareStatement(DeclareStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitIfStatement(IfStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitCreateTableStatement(CreateTableStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitDropTableStatement(DropTableStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitCreateIndexStatement(CreateIndexStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitAlterIndexStatement(AlterIndexStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitDropIndexStatement(DropIndexStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitStringifyStatement(StringifyStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitBreakStatement(BreakStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitContinueStatement(ContinueStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitGotoStatement(GotoStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitReturnStatement(ReturnStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitThrowStatement(ThrowStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitTryCatchStatement(TryCatchStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitLabelStatement(LabelStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitWaitforDelayStatement(WaitforDelayStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitWaitforTimeStatement(WaitforTimeStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitWhileStatement(WhileStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitCreateViewStatement(CreateViewStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitAlterViewStatement(AlterViewStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitDropViewStatement(DropViewStatement statement, VisitorState state){ throw new NotImplementedException();}
        protected virtual void VisitExecuteStatement(ExecuteStatement statement, VisitorState state){ throw new NotImplementedException();}



    }
}