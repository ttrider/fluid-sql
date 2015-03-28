// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Linq;
using System.Collections.Generic;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {
        protected VisitorState State = new VisitorState();
        protected string IdentifierOpenQuote = "\"";
        protected string IdentifierCloseQuote = "\"";
        protected string LiteralOpenQuote = "'";
        protected string LiteralCloseQuote = "'";
        protected string CommentOpenQuote = "/*";
        protected string CommentCloseQuote = "*/";

        protected abstract void VisitJoinType(Joins join);

        protected virtual string ResolveName(Name name)
        {
            return name.GetFullName(this.IdentifierOpenQuote, this.IdentifierCloseQuote);
        }

        public VisitorState Compile(IStatement statement)
        {
            this.VisitStatement(statement);
            State.WriteStatementTerminator();
            return State;
        }

        protected virtual void VisitToken(Token token)
        {
            VisitToken(token, false);
        }
        protected virtual void VisitToken(Token token, bool includeAlias)
        {
            if (token is IStatement)
            {
                VisitStatementToken((IStatement)token);
            }
            else
            {
                if (!TokenVisitors.ContainsKey(token.GetType()))
                {
                    throw new NotImplementedException("Token " + token.GetType().Name + " is not implemented");
                }
                // todo check for statement
                TokenVisitors[token.GetType()](this, token);
            }
            if (includeAlias && token is AliasedToken)
            {
                if (!string.IsNullOrWhiteSpace(((AliasedToken)token).Alias))
                {
                    State.Write(Symbols.AS);
                    State.Write(this.IdentifierOpenQuote, ((AliasedToken)token).Alias, this.IdentifierCloseQuote);
                }
            }

            State.Parameters.AddRange(token.Parameters);
            State.ParameterValues.AddRange(token.ParameterValues);
        }

        protected virtual void VisitStatement(IStatement statement)
        {
            if (!StatementVisitors.ContainsKey(statement.GetType()))
            {
                throw new NotImplementedException("Statement " + statement.GetType().Name + " is not implemented");
            }

            StatementVisitors[statement.GetType()](this, statement);
        }


        protected virtual void VisitConflict(OnConflict? conflict)
        {
            if (conflict.HasValue)
            {
                State.Write(Symbols.ON);
                State.Write(Symbols.CONFLICT);
                switch (conflict.Value)
                {
                    case OnConflict.Abort:
                        State.Write(Symbols.ABORT);
                        break;
                    case OnConflict.Fail:
                        State.Write(Symbols.FAIL);
                        break;
                    case OnConflict.Ignore:
                        State.Write(Symbols.IGNORE);
                        break;
                    case OnConflict.Replace:
                        State.Write(Symbols.REPLACE);
                        break;
                    case OnConflict.Rollback:
                        State.Write(Symbols.ROLLBACK);
                        break;
                }

            }

        }

        protected virtual bool VisitTransactionName(TransactionStatement statement)
        {
            if (statement.Name != null)
            {
                VisitNameToken(statement.Name);
                return true;
            }
            if (statement.Parameter != null)
            {
                State.Write(statement.Parameter.Name);
                State.Parameters.Add(statement.Parameter);
                return true;
            }
            return false;
        }

        protected virtual void VisitAliasedTokenSet(IEnumerable<AliasedToken> tokens, string prefix,
            string separator, string suffix)
        {
            if (tokens != null)
            {
                var enumerator = tokens.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    State.Write(prefix);
                    VisitToken(enumerator.Current, true);

                    while (enumerator.MoveNext())
                    {
                        State.Write(separator);
                        VisitToken(enumerator.Current, true);
                    }
                    State.Write(suffix);
                }
            }
        }

        protected virtual void VisitTokenSet(IEnumerable<Token> tokens, Action prefix = null,
            string separator = Symbols.Comma, Action suffix = null)
        {
            if (tokens != null)
            {
                var enumerator = tokens.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    if (prefix != null)
                    {
                        prefix();
                    }
                    VisitToken(enumerator.Current);

                    while (enumerator.MoveNext())
                    {
                        State.Write(separator);
                        VisitToken(enumerator.Current);
                    }
                    if (suffix != null)
                    {
                        suffix();
                    }
                }
            }
        }

        protected virtual void VisitTokenSetInParenthesis(IEnumerable<Token> tokens, Action prefix = null, bool includeAlias = false)
        {
            if (tokens != null)
            {
                var enumerator = tokens.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    if (prefix != null)
                    {
                        prefix();
                    }
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(enumerator.Current, includeAlias);

                    while (enumerator.MoveNext())
                    {
                        State.Write(Symbols.Comma);
                        VisitToken(enumerator.Current, includeAlias);
                    }
                    State.Write(Symbols.CloseParenthesis);
                }
            }
        }


        protected virtual void VisitCommonTableExpressions(IList<CTEDefinition> definitions,
            bool emitRECURSIVE = false)
        {
            if (definitions != null)
            {
                this.VisitTokenSet(definitions, () =>
                {
                    State.Write(Symbols.WITH);
                    if (emitRECURSIVE && definitions.Any(d => d.Declaration.Recursive))
                    {
                        State.Write(Symbols.RECURSIVE);
                    }
                });
            }
        }

        protected virtual void VisitCommonTableExpression(CTEDefinition definition)
        {
            VisitNameToken(definition.Declaration.Name);
            this.VisitTokenSetInParenthesis(definition.Declaration.Columns);
            State.Write(Symbols.AS);
            State.Write(Symbols.OpenParenthesis);
            this.VisitStatement(definition.Definition);
            State.Write(Symbols.CloseParenthesis);

        }

        private static readonly Dictionary<Type, Action<Visitor, Token>> TokenVisitors =
                new Dictionary<Type, Action<Visitor, Token>>
            {
                {typeof (Scalar),(v,t)=>v.VisitScalarToken((Scalar)t)},
                {typeof (Name),(v,t)=>v.VisitNameToken((Name)t)},
                {typeof (Parameter),(v,t)=>v.VisitParameterToken((Parameter)t)},
                {typeof (Snippet),(v,t)=>v.VisitSnippetToken((Snippet)t)},
                {typeof (Function),(v,t)=>v.VisitFunctionToken((Function)t)},
                {typeof (IsEqualsToken),(v,t)=>v.VisitIsEqualsToken((IsEqualsToken)t)},
                {typeof (NotEqualToken),(v,t)=>v.VisitNotEqualToken((NotEqualToken)t)},
                {typeof (LessToken),(v,t)=>v.VisitLessToken((LessToken)t)},
                {typeof (NotLessToken),(v,t)=>v.VisitNotLessToken((NotLessToken)t)},
                {typeof (LessOrEqualToken),(v,t)=>v.VisitLessOrEqualToken((LessOrEqualToken)t)},
                {typeof (GreaterToken),(v,t)=>v.VisitGreaterToken((GreaterToken)t)},
                {typeof (NotGreaterToken),(v,t)=>v.VisitNotGreaterToken((NotGreaterToken)t)},
                {typeof (GreaterOrEqualToken),(v,t)=>v.VisitGreaterOrEqualToken((GreaterOrEqualToken)t)},
                {typeof (AndToken),(v,t)=>v.VisitAndToken((AndToken)t)},
                {typeof (OrToken),(v,t)=>v.VisitOrToken((OrToken)t)},
                {typeof (PlusToken),(v,t)=>v.VisitPlusToken((PlusToken)t)},
                {typeof (MinusToken),(v,t)=>v.VisitMinusToken((MinusToken)t)},
                {typeof (DivideToken),(v,t)=>v.VisitDivideToken((DivideToken)t)},
                {typeof (ModuloToken),(v,t)=>v.VisitModuloToken((ModuloToken)t)},
                {typeof (MultiplyToken),(v,t)=>v.VisitMultiplyToken((MultiplyToken)t)},
                {typeof (BitwiseAndToken),(v,t)=>v.VisitBitwiseAndToken((BitwiseAndToken)t)},
                {typeof (BitwiseOrToken),(v,t)=>v.VisitBitwiseOrToken((BitwiseOrToken)t)},
                {typeof (BitwiseXorToken),(v,t)=>v.VisitBitwiseXorToken((BitwiseXorToken)t)},
                {typeof (BitwiseNotToken),(v,t)=>v.VisitBitwiseNotToken((BitwiseNotToken)t)},
                {typeof (ContainsToken),(v,t)=>v.VisitContainsToken((ContainsToken)t)},
                {typeof (StartsWithToken),(v,t)=>v.VisitStartsWithToken((StartsWithToken)t)},
                {typeof (EndsWithToken),(v,t)=>v.VisitEndsWithToken((EndsWithToken)t)},
                {typeof (LikeToken),(v,t)=>v.VisitLikeToken((LikeToken)t)},
                {typeof (GroupToken),(v,t)=>v.VisitGroupToken((GroupToken)t)},
                {typeof (NotToken),(v,t)=>v.VisitNotToken((NotToken)t)},
                {typeof (IsNullToken),(v,t)=>v.VisitIsNullToken((IsNullToken)t)},
                {typeof (IsNotNullToken),(v,t)=>v.VisitIsNotNullToken((IsNotNullToken)t)},
                {typeof (ExistsToken),(v,t)=>v.VisitExistsToken((ExistsToken)t)},
                {typeof (AllToken),(v,t)=>v.VisitAllToken((AllToken)t)},
                {typeof (AnyToken),(v,t)=>v.VisitAnyToken((AnyToken)t)},
                {typeof (AssignToken),(v,t)=>v.VisitAssignToken((AssignToken)t)},
                {typeof (BetweenToken),(v,t)=>v.VisitBetweenToken((BetweenToken)t)},
                {typeof (InToken),(v,t)=>v.VisitInToken((InToken)t)},
                {typeof (NotInToken),(v,t)=>v.VisitNotInToken((NotInToken)t)},
                {typeof (CommentToken),(v,t)=>v.VisitCommentToken((CommentToken)t)},
                {typeof (StringifyToken),(v,t)=>v.VisitStringifyToken((StringifyToken)t)},
                {typeof (WhenMatchedTokenThenDeleteToken),(v,t)=>v.VisitWhenMatchedThenDelete((WhenMatchedTokenThenDeleteToken)t)},
                {typeof (WhenMatchedTokenThenUpdateSetToken),(v,t)=>v.VisitWhenMatchedThenUpdateSet((WhenMatchedTokenThenUpdateSetToken)t)},
                {typeof (WhenNotMatchedTokenThenInsertToken),(v,t)=>v.VisitWhenNotMatchedThenInsert((WhenNotMatchedTokenThenInsertToken)t)},
                {typeof (Order),(v,t)=>v.VisitOrderToken((Order)t)},
                {typeof (CTEDefinition),(v,t)=>v.VisitCommonTableExpression((CTEDefinition)t)},
            };

        private static readonly Dictionary<Type, Action<Visitor, IStatement>> StatementVisitors =
            new Dictionary<Type, Action<Visitor, IStatement>>
            {
                {typeof (DeleteStatement), (v, stm)=>v.VisitDelete((DeleteStatement)stm)},
                {typeof (UpdateStatement), (v, stm)=>v.VisitUpdate((UpdateStatement)stm)},
                {typeof (InsertStatement), (v, stm)=>v.VisitInsert((InsertStatement)stm)},
                {typeof (SelectStatement), (v, stm)=>v.VisitSelect((SelectStatement)stm)},
                {typeof (MergeStatement),(v, stm)=>v. VisitMerge((MergeStatement)stm)},
                {typeof (SetStatement), (v, stm)=>v.VisitSet((SetStatement)stm)},
                {typeof (UnionStatement), (v, stm)=>v.VisitUnionStatement((UnionStatement)stm)},
                {typeof (IntersectStatement), (v, stm)=>v.VisitIntersectStatement((IntersectStatement)stm)},
                {typeof (ExceptStatement), (v, stm)=>v.VisitExceptStatement((ExceptStatement)stm)},
                {typeof (BeginTransactionStatement), (v, stm)=>v.VisitBeginTransaction((BeginTransactionStatement)stm)},
                {typeof (CommitTransactionStatement), (v, stm)=>v.VisitCommitTransaction((CommitTransactionStatement)stm)},
                {typeof (RollbackTransactionStatement), (v, stm)=>v.VisitRollbackTransaction((RollbackTransactionStatement)stm)},
                {typeof (StatementsStatement), (v, stm)=>v.VisitStatementsStatement((StatementsStatement)stm)},
                {typeof (SaveTransactionStatement), (v, stm)=>v.VisitSaveTransaction((SaveTransactionStatement)stm)},
                {typeof (DeclareStatement), (v, stm)=>v.VisitDeclareStatement((DeclareStatement)stm)},
                {typeof (IfStatement), (v, stm)=>v.VisitIfStatement((IfStatement)stm)},
                {typeof (CreateTableStatement), (v, stm)=>v.VisitCreateTableStatement((CreateTableStatement)stm)},
                {typeof (DropTableStatement), (v, stm)=>v.VisitDropTableStatement((DropTableStatement)stm)},
                {typeof (CreateIndexStatement), (v, stm)=>v.VisitCreateIndexStatement((CreateIndexStatement)stm)},
                {typeof (AlterIndexStatement), (v, stm)=>v.VisitAlterIndexStatement((AlterIndexStatement)stm)},
                {typeof (DropIndexStatement), (v, stm)=>v.VisitDropIndexStatement((DropIndexStatement)stm)},
                {typeof (CommentStatement), (v, stm)=>v.VisitCommentStatement((CommentStatement)stm)},
                {typeof (StringifyStatement), (v, stm)=>v.VisitStringifyStatement((StringifyStatement)stm)},
                {typeof (SnippetStatement), (v, stm)=>v.VisitSnippetStatement((SnippetStatement)stm)},
                {typeof (BreakStatement), (v, stm)=>v.VisitBreakStatement((BreakStatement)stm)},
                {typeof (ContinueStatement), (v, stm)=>v.VisitContinueStatement((ContinueStatement)stm)},
                {typeof (GotoStatement), (v, stm)=>v.VisitGotoStatement((GotoStatement)stm)},
                {typeof (ReturnStatement), (v, stm)=>v.VisitReturnStatement((ReturnStatement)stm)},
                {typeof (ThrowStatement), (v, stm)=>v.VisitThrowStatement((ThrowStatement)stm)},
                {typeof (TryCatchStatement), (v, stm)=>v.VisitTryCatchStatement((TryCatchStatement)stm)},
                {typeof (LabelStatement), (v, stm)=>v.VisitLabelStatement((LabelStatement)stm)},
                {typeof (WaitforDelayStatement), (v, stm)=>v.VisitWaitforDelayStatement((WaitforDelayStatement)stm)},
                {typeof (WaitforTimeStatement), (v, stm)=>v.VisitWaitforTimeStatement((WaitforTimeStatement)stm)},
                {typeof (WhileStatement), (v, stm)=>v.VisitWhileStatement((WhileStatement)stm)},
                {typeof (CreateViewStatement), (v, stm)=>v.VisitCreateViewStatement((CreateViewStatement)stm)},
                {typeof (CreateOrAlterViewStatement), (v, stm)=>v.VisitCreateOrAlterViewStatement((CreateOrAlterViewStatement)stm)},
                {typeof (AlterViewStatement), (v, stm)=>v.VisitAlterViewStatement((AlterViewStatement)stm)},
                {typeof (DropViewStatement), (v, stm)=>v.VisitDropViewStatement((DropViewStatement)stm)},
                {typeof (ExecuteStatement), (v, stm)=>v.VisitExecuteStatement((ExecuteStatement)stm)},
            };



    }
}





