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

        protected override void VisitMerge(MergeStatement statement) { throw new NotImplementedException(); }
        protected override void VisitSet(SetStatement statement) { throw new NotImplementedException(); }
        protected override void VisitBeginTransaction(BeginTransactionStatement statement) { throw new NotImplementedException(); }
        protected override void VisitCommitTransaction(CommitTransactionStatement statement) { throw new NotImplementedException(); }
        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement) { throw new NotImplementedException(); }
        protected override void VisitSaveTransaction(SaveTransactionStatement statement) { throw new NotImplementedException(); }
        protected override void VisitDeclareStatement(DeclareStatement statement) { throw new NotImplementedException(); }
        protected override void VisitIfStatement(IfStatement statement) { throw new NotImplementedException(); }

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
            if(statement.IsCascade.HasValue)
            {
                if(statement.IsCascade.Value)
                {
                    State.Write(Symbols.CASCADE);
                }
                else
                {
                    State.Write(Symbols.RESTRICT);
                }
            }
        }

        protected override void VisitCreateIndexStatement(CreateIndexStatement createIndexStatement)
        {
            State.Write(Symbols.CREATE);
            
            State.Write(Symbols.INDEX);

            VisitToken(createIndexStatement.Name);

            State.Write(Symbols.ON);

            VisitToken(createIndexStatement.On);

            VisitTokenSetInParenthesis(createIndexStatement.Columns);

            State.Write(Symbols.Semicolon);
        }
        
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