// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TTRider.FluidSql.Providers
{
    public abstract partial class Visitor
    {
        static readonly string[] supportedDialects = { "", "ansi" };


        private static readonly Dictionary<Type, Action<Visitor, Token>> TokenVisitors =
            new Dictionary<Type, Action<Visitor, Token>>
            {
                {
                    typeof (PlaceholderExpressionToken),
                    (v, t) => v.VisitPlaceholderExpressionToken((PlaceholderExpressionToken) t)
                },
                { typeof (Scalar), (v, t) => v.VisitScalarToken((Scalar) t) },
                { typeof (Name), (v, t) => v.VisitNameToken((Name) t) },
                { typeof (Parameter), (v, t) => v.VisitParameterToken((Parameter) t) },
                { typeof (Snippet), (v, t) => v.VisitSnippetToken((Snippet) t) },
                { typeof (Function), (v, t) => v.VisitFunctionToken((Function) t) },
                { typeof (IsEqualsToken), (v, t) => v.VisitIsEqualsToken((IsEqualsToken) t) },
                { typeof (NotEqualToken), (v, t) => v.VisitNotEqualToken((NotEqualToken) t) },
                { typeof (LessToken), (v, t) => v.VisitLessToken((LessToken) t) },
                { typeof (NotLessToken), (v, t) => v.VisitNotLessToken((NotLessToken) t) },
                { typeof (LessOrEqualToken), (v, t) => v.VisitLessOrEqualToken((LessOrEqualToken) t) },
                { typeof (GreaterToken), (v, t) => v.VisitGreaterToken((GreaterToken) t) },
                { typeof (NotGreaterToken), (v, t) => v.VisitNotGreaterToken((NotGreaterToken) t) },
                { typeof (GreaterOrEqualToken), (v, t) => v.VisitGreaterOrEqualToken((GreaterOrEqualToken) t) },
                { typeof (AndToken), (v, t) => v.VisitAndToken((AndToken) t) },
                { typeof (OrToken), (v, t) => v.VisitOrToken((OrToken) t) },
                { typeof (PlusToken), (v, t) => v.VisitPlusToken((PlusToken) t) },
                { typeof (MinusToken), (v, t) => v.VisitMinusToken((MinusToken) t) },
                { typeof (DivideToken), (v, t) => v.VisitDivideToken((DivideToken) t) },
                { typeof (ModuloToken), (v, t) => v.VisitModuloToken((ModuloToken) t) },
                { typeof (MultiplyToken), (v, t) => v.VisitMultiplyToken((MultiplyToken) t) },
                { typeof (BitwiseAndToken), (v, t) => v.VisitBitwiseAndToken((BitwiseAndToken) t) },
                { typeof (BitwiseOrToken), (v, t) => v.VisitBitwiseOrToken((BitwiseOrToken) t) },
                { typeof (BitwiseXorToken), (v, t) => v.VisitBitwiseXorToken((BitwiseXorToken) t) },
                { typeof (BitwiseNotToken), (v, t) => v.VisitBitwiseNotToken((BitwiseNotToken) t) },
                { typeof (ContainsToken), (v, t) => v.VisitContainsToken((ContainsToken) t) },
                { typeof (StartsWithToken), (v, t) => v.VisitStartsWithToken((StartsWithToken) t) },
                { typeof (EndsWithToken), (v, t) => v.VisitEndsWithToken((EndsWithToken) t) },
                { typeof (LikeToken), (v, t) => v.VisitLikeToken((LikeToken) t) },
                { typeof (GroupToken), (v, t) => v.VisitGroupToken((GroupToken) t) },
                { typeof (UnaryMinusToken), (v, t) => v.VisitUnaryMinusToken((UnaryMinusToken) t) },
                { typeof (NotToken), (v, t) => v.VisitNotToken((NotToken) t) },
                { typeof (IsNullToken), (v, t) => v.VisitIsNullToken((IsNullToken) t) },
                { typeof (IsNotNullToken), (v, t) => v.VisitIsNotNullToken((IsNotNullToken) t) },
                { typeof (ExistsToken), (v, t) => v.VisitExistsToken((ExistsToken) t) },
                { typeof (AllToken), (v, t) => v.VisitAllToken((AllToken) t) },
                { typeof (AnyToken), (v, t) => v.VisitAnyToken((AnyToken) t) },
                { typeof (AssignToken), (v, t) => v.VisitAssignToken((AssignToken) t) },
                { typeof (BetweenToken), (v, t) => v.VisitBetweenToken((BetweenToken) t) },
                { typeof (InToken), (v, t) => v.VisitInToken((InToken) t) },
                { typeof (NotInToken), (v, t) => v.VisitNotInToken((NotInToken) t) },
                { typeof (CommentToken), (v, t) => v.VisitCommentToken((CommentToken) t) },
                { typeof (StringifyToken), (v, t) => v.VisitStringifyToken((StringifyToken) t) },
                {
                    typeof (WhenMatchedTokenThenDeleteToken),
                    (v, t) => v.VisitWhenMatchedThenDelete((WhenMatchedTokenThenDeleteToken) t)
                },
                {
                    typeof (WhenMatchedTokenThenUpdateSetToken),
                    (v, t) => v.VisitWhenMatchedThenUpdateSet((WhenMatchedTokenThenUpdateSetToken) t)
                },
                {
                    typeof (WhenNotMatchedTokenThenInsertToken),
                    (v, t) => v.VisitWhenNotMatchedThenInsert((WhenNotMatchedTokenThenInsertToken) t)
                },
                { typeof (Order), (v, t) => v.VisitOrderToken((Order) t) },
                { typeof (CTEDefinition), (v, t) => v.VisitCommonTableExpression((CTEDefinition) t) },
                { typeof (RecordsetSourceToken), (v, t) => v.VisitFromToken((RecordsetSourceToken) t) },
                { typeof (CaseToken), (v, t) => v.VisitCaseToken((CaseToken) t) },
                { typeof (CastToken), (v, t) => v.VisitCastToken((CastToken) t) },
                /*functions*/
                { typeof (NowFunctionToken), (v, t) => v.VisitNowFunctionToken((NowFunctionToken) t) },
                { typeof (UuidFunctionToken), (v, t) => v.VisitUuidFunctionToken((UuidFunctionToken) t) },
                { typeof (IifFunctionToken), (v, t) => v.VisitIIFFunctionToken((IifFunctionToken) t) },
                { typeof (DatePartFunctionToken), (v, t) => v.VisitDatePartFunctionToken((DatePartFunctionToken) t) },
                { typeof (DateAddFunctionToken), (v, t) => v.VisitDateAddFunctionToken((DateAddFunctionToken) t) },
                { typeof (DurationFunctionToken), (v, t) => v.VisitDurationFunctionToken((DurationFunctionToken) t) },
                { typeof (MakeDateFunctionToken), (v, t) => v.VisitMakeDateFunctionToken((MakeDateFunctionToken) t) },
                { typeof (MakeTimeFunctionToken), (v, t) => v.VisitMakeTimeFunctionToken((MakeTimeFunctionToken) t) },
                { typeof (CoalesceFunctionToken), (v, t) => v.VisitCoalesceFunctionToken((CoalesceFunctionToken) t) },
                { typeof (NullIfFunctionToken), (v, t) => v.VisitNullIfFunctionToken((NullIfFunctionToken) t) }
            };

        private static readonly Dictionary<Type, Action<Visitor, IStatement>> StatementVisitors =
            new Dictionary<Type, Action<Visitor, IStatement>>
            {
                { typeof (DeleteStatement), (v, stm) => v.VisitDelete((DeleteStatement) stm) },
                { typeof (UpdateStatement), (v, stm) => v.VisitUpdate((UpdateStatement) stm) },
                { typeof (InsertStatement), (v, stm) => v.VisitInsert((InsertStatement) stm) },
                { typeof (SelectStatement), (v, stm) => v.VisitSelect((SelectStatement) stm) },
                { typeof (MergeStatement), (v, stm) => v.VisitMerge((MergeStatement) stm) },
                { typeof (SetStatement), (v, stm) => v.VisitSet((SetStatement) stm) },
                { typeof (UnionStatement), (v, stm) => v.VisitUnionStatement((UnionStatement) stm) },
                { typeof (IntersectStatement), (v, stm) => v.VisitIntersectStatement((IntersectStatement) stm) },
                { typeof (ExceptStatement), (v, stm) => v.VisitExceptStatement((ExceptStatement) stm) },
                {
                    typeof (BeginTransactionStatement),
                    (v, stm) => v.VisitBeginTransaction((BeginTransactionStatement) stm)
                },
                {
                    typeof (CommitTransactionStatement),
                    (v, stm) => v.VisitCommitTransaction((CommitTransactionStatement) stm)
                },
                {
                    typeof (RollbackTransactionStatement),
                    (v, stm) => v.VisitRollbackTransaction((RollbackTransactionStatement) stm)
                },
                { typeof (StatementsStatement), (v, stm) => v.VisitStatementsStatement((StatementsStatement) stm) },
                {
                    typeof (SaveTransactionStatement), (v, stm) => v.VisitSaveTransaction((SaveTransactionStatement) stm)
                },
                { typeof (DeclareStatement), (v, stm) => v.VisitDeclareStatement((DeclareStatement) stm) },
                { typeof (IfStatement), (v, stm) => v.VisitIfStatement((IfStatement) stm) },
                { typeof (CreateTableStatement), (v, stm) => v.VisitCreateTableStatement((CreateTableStatement) stm) },
                { typeof (DropTableStatement), (v, stm) => v.VisitDropTableStatement((DropTableStatement) stm) },
                { typeof (CreateIndexStatement), (v, stm) => v.VisitCreateIndexStatement((CreateIndexStatement) stm) },
                { typeof (AlterIndexStatement), (v, stm) => v.VisitAlterIndexStatement((AlterIndexStatement) stm) },
                { typeof (DropIndexStatement), (v, stm) => v.VisitDropIndexStatement((DropIndexStatement) stm) },
                { typeof (CommentStatement), (v, stm) => v.VisitCommentStatement((CommentStatement) stm) },
                { typeof (StringifyStatement), (v, stm) => v.VisitStringifyStatement((StringifyStatement) stm) },
                { typeof (SnippetStatement), (v, stm) => v.VisitSnippetStatement((SnippetStatement) stm) },
                { typeof (BreakStatement), (v, stm) => v.VisitBreakStatement((BreakStatement) stm) },
                { typeof (ContinueStatement), (v, stm) => v.VisitContinueStatement((ContinueStatement) stm) },
                {typeof (ExitStatement), (v, stm)=>v.VisitExitStatement((ExitStatement)stm)},
                { typeof (GotoStatement), (v, stm) => v.VisitGotoStatement((GotoStatement) stm) },
                { typeof (ReturnStatement), (v, stm) => v.VisitReturnStatement((ReturnStatement) stm) },
                { typeof (ThrowStatement), (v, stm) => v.VisitThrowStatement((ThrowStatement) stm) },
                { typeof (TryCatchStatement), (v, stm) => v.VisitTryCatchStatement((TryCatchStatement) stm) },
                { typeof (LabelStatement), (v, stm) => v.VisitLabelStatement((LabelStatement) stm) },
                {
                    typeof (WaitforDelayStatement), (v, stm) => v.VisitWaitforDelayStatement((WaitforDelayStatement) stm)
                },
                { typeof (WaitforTimeStatement), (v, stm) => v.VisitWaitforTimeStatement((WaitforTimeStatement) stm) },
                { typeof (WhileStatement), (v, stm) => v.VisitWhileStatement((WhileStatement) stm) },
                { typeof (CreateViewStatement), (v, stm) => v.VisitCreateViewStatement((CreateViewStatement) stm) },
                {
                    typeof (CreateOrAlterViewStatement),
                    (v, stm) => v.VisitCreateOrAlterViewStatement((CreateOrAlterViewStatement) stm)
                },
                { typeof (AlterViewStatement), (v, stm) => v.VisitAlterViewStatement((AlterViewStatement) stm) },
                { typeof (DropViewStatement), (v, stm) => v.VisitDropViewStatement((DropViewStatement) stm) },
                { typeof (ExecuteStatement), (v, stm) => v.VisitExecuteStatement((ExecuteStatement) stm) },
                { typeof (PrepareStatement), (v, stm) => v.VisitPrepareStatement((IExecutableStatement) stm) },
                { typeof (PerformStatement), (v, stm)=>v.VisitPerformStatement((PerformStatement)stm)},
                { typeof (DeallocateStatement), (v, stm)=>v.VisitDeallocateStatement((DeallocateStatement)stm)},
                { typeof (AlterSchemaStatement), (v, stm) => v.VisitAlterSchemaStatement((AlterSchemaStatement) stm) },
                { typeof (DropSchemaStatement), (v, stm) => v.VisitDropSchemaStatement((DropSchemaStatement) stm) },
                {
                    typeof (CreateSchemaStatement), (v, stm) => v.VisitCreateSchemaStatement((CreateSchemaStatement) stm)
                },
                {
                    typeof (CreateProcedureStatement),
                    (v, stm) => v.VisitCreateProcedureStatement((CreateProcedureStatement) stm)
                },
                {
                    typeof (AlterProcedureStatement),
                    (v, stm) => v.VisitAlterProcedureStatement((AlterProcedureStatement) stm)
                },
                {
                    typeof (DropProcedureStatement),
                    (v, stm) => v.VisitDropProcedureStatement((DropProcedureStatement) stm)
                },
                {
                    typeof (ExecuteProcedureStatement),
                    (v, stm) => v.VisitExecuteProcedureStatement((ExecuteProcedureStatement) stm)
                },
                {
                    typeof (CreateFunctionStatement),
                    (v, stm) => v.VisitCreateFunctionStatement((CreateFunctionStatement) stm)
                },
                {
                    typeof (AlterFunctionStatement),
                    (v, stm) => v.VisitAlterFunctionStatement((AlterFunctionStatement) stm)
                },
                {
                    typeof (DropFunctionStatement),
                    (v, stm) => v.VisitDropFunctionStatement((DropFunctionStatement) stm)
                },
                {
                    typeof (ExecuteFunctionStatement),
                    (v, stm) => v.VisitExecuteFunctionStatement((ExecuteFunctionStatement) stm)
                }
            };

        protected string CommentCloseQuote = "*/";
        protected string CommentOpenQuote = "/*";
        protected string IdentifierCloseQuote = "\"";
        protected string IdentifierOpenQuote = "\"";
        protected string LiteralCloseQuote = "'";
        protected string LiteralOpenQuote = "'";

        protected VisitorState State = new VisitorState();

        protected virtual string[] SupportedDialects => supportedDialects;

        protected abstract void VisitJoinType(Joins join);
        protected abstract void VisitType(ITyped typedToken);

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
            if (includeAlias && token is IAliasToken)
            {
                VisitAlias(((IAliasToken)token).Alias);
            }

            State.Parameters.AddRange(token.Parameters);
            State.ParameterValues.AddRange(token.ParameterValues);
        }

        protected virtual void VisitAlias(string alias)
        {
            if (!string.IsNullOrWhiteSpace(alias))
            {
                State.Write(Symbols.AS);
                State.Write(this.IdentifierOpenQuote, alias, this.IdentifierCloseQuote);
            }
        }

        protected virtual void VisitStatement(IStatement statement)
        {
            if (!StatementVisitors.ContainsKey(statement.GetType()))
            {
                throw new NotImplementedException("Statement " + statement.GetType().Name + " is not implemented");
            }

            StatementVisitors[statement.GetType()](this, statement);

            if (statement is Token)
            {
                var token = (Token)statement;
                State.Parameters.AddRange(token.Parameters);
                State.ParameterValues.AddRange(token.ParameterValues);
            }
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

        protected virtual void VisitAliasedTokenSet(IEnumerable<Token> tokens, string prefix,
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

        protected virtual void VisitTokenSet<T>(IEnumerable<T> tokens, Action prefix = null,
            string separator = Symbols.Comma, Action suffix = null, Action<T> visitToken = null)
            where T : Token
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
                    (visitToken ?? VisitToken)(enumerator.Current);

                    while (enumerator.MoveNext())
                    {
                        State.Write(separator);
                        (visitToken ?? VisitToken)(enumerator.Current);
                    }
                    if (suffix != null)
                    {
                        suffix();
                    }
                }
            }
        }

        protected virtual void VisitTokenSetInParenthesis<T>(IEnumerable<T> tokens, Action prefix = null,
            bool includeAlias = false, Action<T, bool> visitToken = null)
            where T : Token
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
                    (visitToken ?? VisitToken)(enumerator.Current, includeAlias);

                    while (enumerator.MoveNext())
                    {
                        State.Write(Symbols.Comma);
                        (visitToken ?? VisitToken)(enumerator.Current, includeAlias);
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


        protected virtual void VisitCaseToken(CaseToken token)
        {
            State.Write(Symbols.CASE);
            if (token.CaseValueToken != null)
            {
                VisitToken(token.CaseValueToken);
            }

            foreach (var whenCondition in token.WhenConditions)
            {
                State.Write(Symbols.WHEN);
                VisitToken(whenCondition.WhenToken);
                State.Write(Symbols.THEN);
                VisitToken(whenCondition.ThenToken);
            }

            if (token.ElseToken != null)
            {
                State.Write(Symbols.ELSE);
                VisitToken(token.ElseToken);
            }

            State.Write(Symbols.END);
        }

        protected virtual void VisitCastToken(CastToken token)
        {
            State.Write(Symbols.CAST);
            State.Write(Symbols.OpenParenthesis);
            VisitToken(token.Token);
            State.Write(Symbols.AS);
            VisitType(token);
            State.Write(Symbols.CloseParenthesis);
        }


        protected virtual void VisitPlaceholderExpressionToken(PlaceholderExpressionToken token)
        {
            VisitToken(token.Content);
        }


        protected virtual void VisitValue(object value)
        {
            if (value == null)
            {
                State.Write(Symbols.NULL);
            }
            else if (value is byte[])
            {
            }
            else if (value is DBNull)
            {
                State.Write(Symbols.NULL);
            }
            else if (value is bool)
            {
                State.Write((bool)value ? "1" : "0");
            }
            else if (value is char || value is string)
            {
                State.Write(this.LiteralOpenQuote, value.ToString(), this.LiteralCloseQuote);
            }
            else if (value is DateTime)
            {
                State.Write(this.LiteralOpenQuote, ((DateTime)value).ToString("s"), this.LiteralCloseQuote);
            }
            else if (value is DateTimeOffset)
            {
                State.Write(this.LiteralOpenQuote, ((DateTimeOffset)value).ToString("O"), this.LiteralCloseQuote);
            }
            else if (value is TimeSpan)
            {
                State.Write(this.LiteralOpenQuote, ((TimeSpan)value).ToString("G"), this.LiteralCloseQuote);
            }
            else if (value is XNode)
            {
                State.Write(this.LiteralOpenQuote, ((XNode)value).ToString(SaveOptions.DisableFormatting),
                    this.LiteralCloseQuote);
            }
            else
            {
                State.Write(value.ToString());
            }
        }
    }
}