using System;
using System.Collections.Generic;

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


        public static VisitorState Compile(IStatement statement)
        {
            var state = new VisitorState();

            var visitor = new SqliteVisitor();

            visitor.VisitStatement(statement, state);
            state.WriteStatementTerminator();
            return state;
        }

        protected override string IdentifierCloseQuote { get { return "\""; } }
        protected override void VisitDelete(DeleteStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitUpdate(UpdateStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitInsert(InsertStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitMerge(MergeStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitSet(SetStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDeclareStatement(DeclareStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitIfStatement(IfStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitStringifyStatement(StringifyStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitBreakStatement(BreakStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitContinueStatement(ContinueStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitGotoStatement(GotoStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitReturnStatement(ReturnStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitThrowStatement(ThrowStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitTryCatchStatement(TryCatchStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitLabelStatement(LabelStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitWaitforDelayStatement(WaitforDelayStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitWaitforTimeStatement(WaitforTimeStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitWhileStatement(WhileStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitExecuteStatement(ExecuteStatement statement, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitStringifyToken(StringifyToken token, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitWhenMatchedThenDelete(WhenMatchedThenDelete token, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitWhenMatchedThenUpdateSet(WhenMatchedThenUpdateSet token, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override void VisitWhenNotMatchedThenInsert(WhenNotMatchedThenInsert token, VisitorState state)
        {
            throw new NotImplementedException();
        }

        protected override string IdentifierOpenQuote { get { return "\""; } }
        protected override string LiteralOpenQuote { get { return "'"; } }
        protected override string LiteralCloseQuote { get { return "'"; } }
        protected override string CommentOpenQuote { get { return "/*"; } }
        protected override string CommentCloseQuote { get { return "*/"; } }



        protected override void VisitJoinType(Joins join, VisitorState state)
        {
            if (join == Joins.RightOuter || join == Joins.FullOuter)
            {
                throw new NotImplementedException("join "+join+" is not implemented on SQLite");
            }
            state.Write(JoinStrings[(int)join]);
        }


         void VisitType(TypedToken typedToken, VisitorState state)
        {
            if (typedToken.DbType.HasValue)
            {
                state.Write(DbTypeStrings[(int)typedToken.DbType]);
            }
        }
    }
}
