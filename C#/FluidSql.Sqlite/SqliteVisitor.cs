using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor : Visitor
    {
        private readonly string[] DbTypeStrings =
        {
            "BIGINT", //BigInt = 0,
            "BINARY", //Binary = 1,      ()
            "BIT", //Bit = 2,
            "CHAR", //Char = 3,          ()
            "DATETIME", //DateTime = 4,
            "DECIMAL", //Decimal = 5,    (,)
            "FLOAT", //Float = 6,
            "IMAGE", //Image = 7,
            "INTEGER", //Int = 8,
            "MONEY", //Money = 9,
            "NCHAR", //NChar = 10,      ()
            "NTEXT", //NText = 11,
            "NVARCHAR", //NVarChar = 12, ()
            "REAL", //Real = 13,
            "UNIQUEIDENTIFIER", //UniqueIdentifier = 14,
            "SMALLDATETIME", //SmallDateTime = 15,
            "SMALLINT", //SmallInt = 16,
            "SMALLMONEY", //SmallMoney = 17,
            "TEXT", //Text = 18,
            "TIMESTAMP", //Timestamp = 19,
            "TINYINT", //TinyInt = 20,
            "VARBINARY", //VarBinary = 21,   ()
            "VARCHAR", //VarChar = 22,       ()
            "SQL_VARIANT", //Variant = 23,
            "Xml", //Xml = 25,
            "", //Udt = 29,
            "", //Structured = 30,
            "DATE", //Date = 31,
            "TIME", //Time = 32,            ()
            "DATETIME2", //DateTime2 = 33,  ()
            "DateTimeOffset" //DateTimeOffset = 34, ()
        };

        private static readonly Dictionary<Type, Action<SqliteVisitor, Token, VisitorState>> TokenVisitors =
    new Dictionary<Type, Action<SqliteVisitor, Token, VisitorState>>
            {
                {typeof (SelectStatement), (v,t,s)=>v.VisitStatementToken((IStatement)t, s)},
                {typeof (Union),(v,t,s)=>v.VisitStatementToken((IStatement)t,s)},
                {typeof (Intersect),(v,t,s)=>v.VisitStatementToken((IStatement)t,s)},
                {typeof (Except),(v,t,s)=>v.VisitStatementToken((IStatement)t,s)},
                {typeof (Scalar),(v,t,s)=>v.VisitScalarToken((Scalar)t,s,"'","'")},
                {typeof (Name),(v,t,s)=>v.VisitNameToken((Name)t,s)},
                {typeof (Parameter),(v,t,s)=>v.VisitParameterToken((Parameter)t,s)},
                {typeof (Snippet),(v,t,s)=>v.VisitSnippetToken((Snippet)t,s)},
                {typeof (Function),(v,t,s)=>v.VisitFunctionToken((Function)t,s)},
                {typeof (IsEqualsToken),(v,t,s)=>v.VisitIsEqualsToken((IsEqualsToken)t,s)},
                {typeof (NotEqualToken),(v,t,s)=>v.VisitNotEqualToken((NotEqualToken)t,s)},
                {typeof (LessToken),(v,t,s)=>v.VisitLessToken((LessToken)t,s)},
                //{typeof (NotLessToken),(v,t,s)=>v.VisitNotLessToken((NotLessToken)t,s)},
                {typeof (LessOrEqualToken),(v,t,s)=>v.VisitLessOrEqualToken((LessOrEqualToken)t,s)},
                {typeof (GreaterToken),(v,t,s)=>v.VisitGreaterToken((GreaterToken)t,s)},
                //{typeof (NotGreaterToken),(v,t,s)=>v.VisitNotGreaterToken((NotGreaterToken)t,s)},
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
                //{typeof (AllToken),(v,t,s)=>v.VisitAllToken((AllToken)t,s)},
                //{typeof (AnyToken),(v,t,s)=>v.VisitAnyToken((AnyToken)t,s)},
                //{typeof (AssignToken),(v,t,s)=>v.VisitAssignToken((AssignToken)t,s)},
                {typeof (BetweenToken),(v,t,s)=>v.VisitBetweenToken((BetweenToken)t,s)},
                {typeof (InToken),(v,t,s)=>v.VisitInToken((InToken)t,s)},
                {typeof (NotInToken),(v,t,s)=>v.VisitNotInToken((NotInToken)t,s)},
                {typeof (CommentToken),(v,t,s)=>v.VisitCommentToken((CommentToken)t,s)},
                //{typeof (StringifyToken),(v,t,s)=>v.VisitStringifyToken((StringifyToken)t,s)},
                //{typeof (WhenMatchedThenDelete),(v,t,s)=>v.VisitWhenMatchedThenDelete((WhenMatchedThenDelete)t,s)},
                //{typeof (WhenMatchedThenUpdateSet),(v,t,s)=>v.VisitWhenMatchedThenUpdateSet((WhenMatchedThenUpdateSet)t,s)},
                //{typeof (WhenNotMatchedThenInsert),(v,t,s)=>v.VisitWhenNotMatchedThenInsert((WhenNotMatchedThenInsert)t,s)},
            };

        private static readonly Dictionary<Type, Action<SqliteVisitor, IStatement, VisitorState>> StatementVisitors =
            new Dictionary<Type, Action<SqliteVisitor, IStatement, VisitorState>>
            {
                //{typeof (DeleteStatement), (v,stm,s)=>v.VisitDelete((DeleteStatement)stm, s)},
                //{typeof (UpdateStatement), (v,stm,s)=>v.VisitUpdate((UpdateStatement)stm, s)},
                //{typeof (InsertStatement), (v,stm,s)=>v.VisitInsert((InsertStatement)stm,s)},
                {typeof (SelectStatement), (v,stm,s)=>v.VisitSelect((SelectStatement)stm,s)},
                //{typeof (MergeStatement),(v,stm,s)=>v. VisitMerge((MergeStatement)stm,s)},
                //{typeof (SetStatement), (v,stm,s)=>v.VisitSet((SetStatement)stm,s)},
                {typeof (Union), (v,stm,s)=>v.VisitUnion((Union)stm,s)},
                {typeof (Intersect), (v,stm,s)=>v.VisitIntersect((Intersect)stm,s)},
                {typeof (Except), (v,stm,s)=>v.VisitExcept((Except)stm,s)},
                {typeof (BeginTransactionStatement), (v,stm,s)=>v.VisitBeginTransaction((BeginTransactionStatement)stm,s)},
                //{typeof (CommitTransactionStatement), (v,stm,s)=>v.VisitCommitTransaction((CommitTransactionStatement)stm,s)},
                //{typeof (RollbackTransactionStatement), (v,stm,s)=>v.VisitRollbackTransaction((RollbackTransactionStatement)stm,s)},
                //{typeof (StatementsStatement), (v,stm,s)=>v.VisitStatementsStatement((StatementsStatement)stm,s)},
                //{typeof (SaveTransactionStatement), (v,stm,s)=>v.VisitSaveTransaction((SaveTransactionStatement)stm,s)},
                //{typeof (DeclareStatement), (v,stm,s)=>v.VisitDeclareStatement((DeclareStatement)stm,s)},
                //{typeof (IfStatement), (v,stm,s)=>v.VisitIfStatement((IfStatement)stm,s)},
                {typeof (CreateTableStatement), (v,stm,s)=>v.VisitCreateTableStatement((CreateTableStatement)stm,s)},
                {typeof (DropTableStatement), (v,stm,s)=>v.VisitDropTableStatement((DropTableStatement)stm,s)},
                {typeof (CreateIndexStatement), (v,stm,s)=>v.VisitCreateIndexStatement((CreateIndexStatement)stm,s)},
                {typeof (AlterIndexStatement), (v,stm,s)=>v.VisitAlterIndexStatement((AlterIndexStatement)stm,s)},
                {typeof (DropIndexStatement), (v,stm,s)=>v.VisitDropIndexStatement((DropIndexStatement)stm,s)},
                {typeof (CommentStatement), (v,stm,s)=>v.VisitCommentStatement((CommentStatement)stm,s)},
                //{typeof (StringifyStatement), (v,stm,s)=>v.VisitStringifyStatement((StringifyStatement)stm,s)},
                {typeof (SnippetStatement), (v,stm,s)=>v.VisitSnippetStatement((SnippetStatement)stm,s)},
                //{typeof (BreakStatement), (v,stm,s)=>v.VisitBreakStatement((BreakStatement)stm,s)},
                //{typeof (ContinueStatement), (v,stm,s)=>v.VisitContinueStatement((ContinueStatement)stm,s)},
                //{typeof (GotoStatement), (v,stm,s)=>v.VisitGotoStatement((GotoStatement)stm,s)},
                //{typeof (ReturnStatement), (v,stm,s)=>v.VisitReturnStatement((ReturnStatement)stm,s)},
                //{typeof (ThrowStatement), (v,stm,s)=>v.VisitThrowStatement((ThrowStatement)stm,s)},
                //{typeof (TryCatchStatement), (v,stm,s)=>v.VisitTryCatchStatement((TryCatchStatement)stm,s)},
                //{typeof (LabelStatement), (v,stm,s)=>v.VisitLabelStatement((LabelStatement)stm,s)},
                //{typeof (WaitforDelayStatement), (v,stm,s)=>v.VisitWaitforDelayStatement((WaitforDelayStatement)stm,s)},
                //{typeof (WaitforTimeStatement), (v,stm,s)=>v.VisitWaitforTimeStatement((WaitforTimeStatement)stm,s)},
                //{typeof (WhileStatement), (v,stm,s)=>v.VisitWhileStatement((WhileStatement)stm,s)},
                {typeof (CreateViewStatement), (v,stm,s)=>v.VisitCreateViewStatement((CreateViewStatement)stm,s)},
                {typeof (CreateOrAlterViewStatement), (v,stm,s)=>v.VisitCreateOrAlterViewStatement((CreateOrAlterViewStatement)stm,s)},
                {typeof (AlterViewStatement), (v,stm,s)=>v.VisitAlterViewStatement((AlterViewStatement)stm,s)},
                {typeof (DropViewStatement), (v,stm,s)=>v.VisitDropViewStatement((DropViewStatement)stm,s)},
                //{typeof (ExecuteStatement), (v,stm,s)=>v.VisitExecuteStatement((ExecuteStatement)stm,s)},
            };


        public static VisitorState Compile(IStatement statement)
        {
            var state = new VisitorState();

            var visitor = new SqliteVisitor();

            visitor.VisitStatement(statement, state);
            visitor.EnsureSemicolumn(state);

            return state;
        }

        protected override string CloseQuote
        {
            get { return "\""; }
        }
        protected override string OpenQuote
        {
            get { return "\""; }
        }

        protected override void VisitJoinType(Joins join, VisitorState state)
        {
            if (join == Joins.RightOuter || join == Joins.FullOuter)
            {
                throw new NotImplementedException("join "+join+" is not implemented on SQLite");
            }
            state.Append(JoinStrings[(int)join]);
        }


        private void VisitType(TypedToken typedToken, VisitorState state)
        {
            if (typedToken.DbType.HasValue)
            {
                state.Append(Sym.SPACE);
                state.Append(DbTypeStrings[(int)typedToken.DbType]);
            }
        }

        protected override void VisitToken(Token token, VisitorState state, bool includeAlias = false)
        {
            // todo check for statement
            if (!TokenVisitors.ContainsKey(token.GetType()))
            {
                throw new NotImplementedException("Token " + token.GetType().Name+" is not implemented");
            }
            
            TokenVisitors[token.GetType()](this, token, state);

            if (includeAlias)
            {
                VisitAlias(token, state);
            }

            state.Parameters.AddRange(token.Parameters);
            state.ParameterValues.AddRange(token.ParameterValues);
        }

        protected override void VisitStatement(IStatement statement, VisitorState state)
        {
            if (!StatementVisitors.ContainsKey(statement.GetType()))
            {
                throw new NotImplementedException("Statement " + statement.GetType().Name + " is not implemented");
            }

            StatementVisitors[statement.GetType()](this, statement, state);
        }
    }
}
