using System;
using System.Collections.Generic;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {
        protected string IdentifierOpenQuote = "\"";
        protected string IdentifierCloseQuote = "\"";
        protected string LiteralOpenQuote = "'";
        protected string LiteralCloseQuote = "'";
        protected string CommentOpenQuote = "/*";
        protected string CommentCloseQuote = "*/";

        protected abstract void VisitJoinType(Joins join, VisitorState state);

        protected virtual string ResolveName(Name name)
        {
            return name.GetFullName(this.IdentifierOpenQuote, this.IdentifierCloseQuote);
        }

        public VisitorState Compile(IStatement statement, VisitorState state)
        {
            this.VisitStatement(statement, state);
            state.WriteStatementTerminator();
            return state;
        }

        protected virtual void VisitToken(Token token, VisitorState state, bool includeAlias = false)
        {
            if (token is IStatement)
            {
                VisitStatementToken((IStatement)token, state);
            }
            else
            {
                if (!TokenVisitors.ContainsKey(token.GetType()))
                {
                    throw new NotImplementedException("Token " + token.GetType().Name + " is not implemented");
                }
                // todo check for statement
                TokenVisitors[token.GetType()](this, token, state);
            }
            if (includeAlias)
            {
                if (!string.IsNullOrWhiteSpace(token.Alias))
                {
                    state.Write(Symbols.AS);
                    state.Write(this.IdentifierOpenQuote, token.Alias, this.IdentifierCloseQuote);
                }
            }

            state.Parameters.AddRange(token.Parameters);
            state.ParameterValues.AddRange(token.ParameterValues);
        }

        protected virtual void VisitStatement(IStatement statement, VisitorState state)
        {
            if (!StatementVisitors.ContainsKey(statement.GetType()))
            {
                throw new NotImplementedException("Statement " + statement.GetType().Name + " is not implemented");
            }

            StatementVisitors[statement.GetType()](this, statement, state);
        }


        protected virtual void VisitConflict(OnConflict? conflict, VisitorState state)
        {
            if (conflict.HasValue)
            {
                state.Write(Symbols.ON);
                state.Write(Symbols.CONFLICT);
                switch (conflict.Value)
                {
                    case OnConflict.Abort:
                        state.Write(Symbols.ABORT);
                        break;
                    case OnConflict.Fail:
                        state.Write(Symbols.FAIL);
                        break;
                    case OnConflict.Ignore:
                        state.Write(Symbols.IGNORE);
                        break;
                    case OnConflict.Replace:
                        state.Write(Symbols.REPLACE);
                        break;
                    case OnConflict.Rollback:
                        state.Write(Symbols.ROLLBACK);
                        break;
                }

            }

        }

        protected virtual bool VisitTransactionName(TransactionStatement statement, VisitorState state)
        {
            if (statement.Name != null)
            {
                VisitNameToken(statement.Name, state);
                return true;
            }
            if (statement.Parameter != null)
            {
                state.Write(statement.Parameter.Name);
                state.Parameters.Add(statement.Parameter);
                return true;
            }
            return false;
        }

        protected virtual void VisitTokenSet(IEnumerable<Token> tokens, VisitorState state, string prefix, string separator, string suffix, bool includeAlias = false)
        {
            var enumerator = tokens.GetEnumerator();
            if (enumerator.MoveNext())
            {
                state.Write(prefix);
                VisitToken(enumerator.Current, state, includeAlias);

                while (enumerator.MoveNext())
                {
                    state.Write(separator);
                    VisitToken(enumerator.Current, state, includeAlias);
                }
                state.Write(suffix);
            }
        }




        private static readonly Dictionary<Type, Action<Visitor, Token, VisitorState>> TokenVisitors =
                new Dictionary<Type, Action<Visitor, Token, VisitorState>>
            {
                {typeof (Scalar),(v,t,s)=>v.VisitScalarToken((Scalar)t,s)},
                {typeof (Name),(v,t,s)=>v.VisitNameToken((Name)t,s)},
                {typeof (Parameter),(v,t,s)=>v.VisitParameterToken((Parameter)t,s)},
                {typeof (Snippet),(v,t,s)=>v.VisitSnippetToken((Snippet)t,s)},
                {typeof (Function),(v,t,s)=>v.VisitFunctionToken((Function)t,s)},
                {typeof (IsEqualsToken),(v,t,s)=>v.VisitIsEqualsToken((IsEqualsToken)t,s)},
                {typeof (NotEqualToken),(v,t,s)=>v.VisitNotEqualToken((NotEqualToken)t,s)},
                {typeof (LessToken),(v,t,s)=>v.VisitLessToken((LessToken)t,s)},
                {typeof (NotLessToken),(v,t,s)=>v.VisitNotLessToken((NotLessToken)t,s)},
                {typeof (LessOrEqualToken),(v,t,s)=>v.VisitLessOrEqualToken((LessOrEqualToken)t,s)},
                {typeof (GreaterToken),(v,t,s)=>v.VisitGreaterToken((GreaterToken)t,s)},
                {typeof (NotGreaterToken),(v,t,s)=>v.VisitNotGreaterToken((NotGreaterToken)t,s)},
                {typeof (GreaterOrEqualToken),(v,t,s)=>v.VisitGreaterOrEqualToken((GreaterOrEqualToken)t,s)},
                {typeof (AndToken),(v,t,s)=>v.VisitAndToken((AndToken)t,s)},
                {typeof (OrToken),(v,t,s)=>v.VisitOrToken((OrToken)t,s)},
                {typeof (PlusToken),(v,t,s)=>v.VisitPlusToken((PlusToken)t,s)},
                {typeof (MinusToken),(v,t,s)=>v.VisitMinusToken((MinusToken)t,s)},
                {typeof (DivideToken),(v,t,s)=>v.VisitDivideToken((DivideToken)t,s)},
                {typeof (ModuloToken),(v,t,s)=>v.VisitModuloToken((ModuloToken)t,s)},
                {typeof (MultiplyToken),(v,t,s)=>v.VisitMultiplyToken((MultiplyToken)t,s)},
                {typeof (BitwiseAndToken),(v,t,s)=>v.VisitBitwiseAndToken((BitwiseAndToken)t,s)},
                {typeof (BitwiseOrToken),(v,t,s)=>v.VisitBitwiseOrToken((BitwiseOrToken)t,s)},
                {typeof (BitwiseXorToken),(v,t,s)=>v.VisitBitwiseXorToken((BitwiseXorToken)t,s)},
                {typeof (BitwiseNotToken),(v,t,s)=>v.VisitBitwiseNotToken((BitwiseNotToken)t,s)},
                {typeof (ContainsToken),(v,t,s)=>v.VisitContainsToken((ContainsToken)t,s)},
                {typeof (StartsWithToken),(v,t,s)=>v.VisitStartsWithToken((StartsWithToken)t,s)},
                {typeof (EndsWithToken),(v,t,s)=>v.VisitEndsWithToken((EndsWithToken)t,s)},
                {typeof (LikeToken),(v,t,s)=>v.VisitLikeToken((LikeToken)t,s)},
                {typeof (GroupToken),(v,t,s)=>v.VisitGroupToken((GroupToken)t,s)},
                {typeof (NotToken),(v,t,s)=>v.VisitNotToken((NotToken)t,s)},
                {typeof (IsNullToken),(v,t,s)=>v.VisitIsNullToken((IsNullToken)t,s)},
                {typeof (IsNotNullToken),(v,t,s)=>v.VisitIsNotNullToken((IsNotNullToken)t,s)},
                {typeof (ExistsToken),(v,t,s)=>v.VisitExistsToken((ExistsToken)t,s)},
                {typeof (AllToken),(v,t,s)=>v.VisitAllToken((AllToken)t,s)},
                {typeof (AnyToken),(v,t,s)=>v.VisitAnyToken((AnyToken)t,s)},
                {typeof (AssignToken),(v,t,s)=>v.VisitAssignToken((AssignToken)t,s)},
                {typeof (BetweenToken),(v,t,s)=>v.VisitBetweenToken((BetweenToken)t,s)},
                {typeof (InToken),(v,t,s)=>v.VisitInToken((InToken)t,s)},
                {typeof (NotInToken),(v,t,s)=>v.VisitNotInToken((NotInToken)t,s)},
                {typeof (CommentToken),(v,t,s)=>v.VisitCommentToken((CommentToken)t,s)},
                {typeof (StringifyToken),(v,t,s)=>v.VisitStringifyToken((StringifyToken)t,s)},
                {typeof (WhenMatchedThenDelete),(v,t,s)=>v.VisitWhenMatchedThenDelete((WhenMatchedThenDelete)t,s)},
                {typeof (WhenMatchedThenUpdateSet),(v,t,s)=>v.VisitWhenMatchedThenUpdateSet((WhenMatchedThenUpdateSet)t,s)},
                {typeof (WhenNotMatchedThenInsert),(v,t,s)=>v.VisitWhenNotMatchedThenInsert((WhenNotMatchedThenInsert)t,s)},
                {typeof (Order),(v,t,s)=>v.VisitOrderToken((Order)t,s)},

                
            };

        private static readonly Dictionary<Type, Action<Visitor, IStatement, VisitorState>> StatementVisitors =
            new Dictionary<Type, Action<Visitor, IStatement, VisitorState>>
            {
                {typeof (DeleteStatement), (v,stm,s)=>v.VisitDelete((DeleteStatement)stm, s)},
                {typeof (UpdateStatement), (v,stm,s)=>v.VisitUpdate((UpdateStatement)stm, s)},
                {typeof (InsertStatement), (v,stm,s)=>v.VisitInsert((InsertStatement)stm,s)},
                {typeof (SelectStatement), (v,stm,s)=>v.VisitSelect((SelectStatement)stm,s)},
                {typeof (MergeStatement),(v,stm,s)=>v. VisitMerge((MergeStatement)stm,s)},
                {typeof (SetStatement), (v,stm,s)=>v.VisitSet((SetStatement)stm,s)},
                {typeof (UnionStatement), (v,stm,s)=>v.VisitUnionStatement((UnionStatement)stm,s)},
                {typeof (IntersectStatement), (v,stm,s)=>v.VisitIntersectStatement((IntersectStatement)stm,s)},
                {typeof (ExceptStatement), (v,stm,s)=>v.VisitExceptStatement((ExceptStatement)stm,s)},
                {typeof (BeginTransactionStatement), (v,stm,s)=>v.VisitBeginTransaction((BeginTransactionStatement)stm,s)},
                {typeof (CommitTransactionStatement), (v,stm,s)=>v.VisitCommitTransaction((CommitTransactionStatement)stm,s)},
                {typeof (RollbackTransactionStatement), (v,stm,s)=>v.VisitRollbackTransaction((RollbackTransactionStatement)stm,s)},
                {typeof (StatementsStatement), (v,stm,s)=>v.VisitStatementsStatement((StatementsStatement)stm,s)},
                {typeof (SaveTransactionStatement), (v,stm,s)=>v.VisitSaveTransaction((SaveTransactionStatement)stm,s)},
                {typeof (DeclareStatement), (v,stm,s)=>v.VisitDeclareStatement((DeclareStatement)stm,s)},
                {typeof (IfStatement), (v,stm,s)=>v.VisitIfStatement((IfStatement)stm,s)},
                {typeof (CreateTableStatement), (v,stm,s)=>v.VisitCreateTableStatement((CreateTableStatement)stm,s)},
                {typeof (DropTableStatement), (v,stm,s)=>v.VisitDropTableStatement((DropTableStatement)stm,s)},
                {typeof (CreateIndexStatement), (v,stm,s)=>v.VisitCreateIndexStatement((CreateIndexStatement)stm,s)},
                {typeof (AlterIndexStatement), (v,stm,s)=>v.VisitAlterIndexStatement((AlterIndexStatement)stm,s)},
                {typeof (DropIndexStatement), (v,stm,s)=>v.VisitDropIndexStatement((DropIndexStatement)stm,s)},
                {typeof (CommentStatement), (v,stm,s)=>v.VisitCommentStatement((CommentStatement)stm,s)},
                {typeof (StringifyStatement), (v,stm,s)=>v.VisitStringifyStatement((StringifyStatement)stm,s)},
                {typeof (SnippetStatement), (v,stm,s)=>v.VisitSnippetStatement((SnippetStatement)stm,s)},
                {typeof (BreakStatement), (v,stm,s)=>v.VisitBreakStatement((BreakStatement)stm,s)},
                {typeof (ContinueStatement), (v,stm,s)=>v.VisitContinueStatement((ContinueStatement)stm,s)},
                {typeof (GotoStatement), (v,stm,s)=>v.VisitGotoStatement((GotoStatement)stm,s)},
                {typeof (ReturnStatement), (v,stm,s)=>v.VisitReturnStatement((ReturnStatement)stm,s)},
                {typeof (ThrowStatement), (v,stm,s)=>v.VisitThrowStatement((ThrowStatement)stm,s)},
                {typeof (TryCatchStatement), (v,stm,s)=>v.VisitTryCatchStatement((TryCatchStatement)stm,s)},
                {typeof (LabelStatement), (v,stm,s)=>v.VisitLabelStatement((LabelStatement)stm,s)},
                {typeof (WaitforDelayStatement), (v,stm,s)=>v.VisitWaitforDelayStatement((WaitforDelayStatement)stm,s)},
                {typeof (WaitforTimeStatement), (v,stm,s)=>v.VisitWaitforTimeStatement((WaitforTimeStatement)stm,s)},
                {typeof (WhileStatement), (v,stm,s)=>v.VisitWhileStatement((WhileStatement)stm,s)},
                {typeof (CreateViewStatement), (v,stm,s)=>v.VisitCreateViewStatement((CreateViewStatement)stm,s)},
                {typeof (CreateOrAlterViewStatement), (v,stm,s)=>v.VisitCreateOrAlterViewStatement((CreateOrAlterViewStatement)stm,s)},
                {typeof (AlterViewStatement), (v,stm,s)=>v.VisitAlterViewStatement((AlterViewStatement)stm,s)},
                {typeof (DropViewStatement), (v,stm,s)=>v.VisitDropViewStatement((DropViewStatement)stm,s)},
                {typeof (ExecuteStatement), (v,stm,s)=>v.VisitExecuteStatement((ExecuteStatement)stm,s)},
            };

    
    
    }
}





