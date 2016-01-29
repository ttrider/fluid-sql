// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;

namespace TTRider.FluidSql.Providers.PostgreSQL
{
    internal partial class PostgreSQLVisitor
    {
        protected override void VisitDelete(DeleteStatement statement) { throw new NotImplementedException(); }
        protected override void VisitUpdate(UpdateStatement statement) { throw new NotImplementedException(); }
        protected override void VisitInsert(InsertStatement statement) { throw new NotImplementedException(); }
        protected override void VisitSelect(SelectStatement statement) { throw new NotImplementedException(); }
        protected override void VisitMerge(MergeStatement statement) { throw new NotImplementedException(); }
        protected override void VisitSet(SetStatement statement) { throw new NotImplementedException(); }
        protected override void VisitUnionStatement(UnionStatement statement) { throw new NotImplementedException(); }
        protected override void VisitIntersectStatement(IntersectStatement statement) { throw new NotImplementedException(); }
        protected override void VisitExceptStatement(ExceptStatement statement) { throw new NotImplementedException(); }
        protected override void VisitBeginTransaction(BeginTransactionStatement statement) { throw new NotImplementedException(); }
        protected override void VisitCommitTransaction(CommitTransactionStatement statement) { throw new NotImplementedException(); }
        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement) { throw new NotImplementedException(); }
//        protected override void VisitStatementsStatement(StatementsStatement statement) { throw new NotImplementedException(); }
        protected override void VisitSaveTransaction(SaveTransactionStatement statement) { throw new NotImplementedException(); }
        protected override void VisitDeclareStatement(DeclareStatement statement) { throw new NotImplementedException(); }
        protected override void VisitIfStatement(IfStatement statement) { throw new NotImplementedException(); }
        protected override void VisitCreateTableStatement(CreateTableStatement statement) { throw new NotImplementedException(); }

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
        }
        protected override void VisitCreateIndexStatement(CreateIndexStatement statement) { throw new NotImplementedException(); }
        protected override void VisitAlterIndexStatement(AlterIndexStatement statement) { throw new NotImplementedException(); }
        protected override void VisitDropIndexStatement(DropIndexStatement statement) { throw new NotImplementedException(); }
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
        protected override void VisitCreateViewStatement(CreateViewStatement statement) { throw new NotImplementedException(); }
        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement statement) { throw new NotImplementedException(); }
        protected override void VisitAlterViewStatement(AlterViewStatement statement) { throw new NotImplementedException(); }
        protected override void VisitDropViewStatement(DropViewStatement statement) { throw new NotImplementedException(); }
        protected override void VisitExecuteStatement(ExecuteStatement statement) { throw new NotImplementedException(); }
        protected override void VisitDropSchemaStatement(DropSchemaStatement statement) { throw new NotImplementedException(); }
        protected override void VisitCreateSchemaStatement(CreateSchemaStatement statement) { throw new NotImplementedException(); }

    }
}