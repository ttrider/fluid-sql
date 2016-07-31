// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System;
using TTRider.FluidSql.Statements;

namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor : Visitor
    {
        private static readonly string[] supportedDialects = { "sqlite", "ansi" };

        private readonly string[] DbTypeStrings =
        {
            "BIGINT", // BigInt = 0,
            "BINARY", // Binary = 1,
            "BIT", // Bit = 2,
            "CHAR", // Char = 3,
            "DATETIME", // DateTime = 4,
            "DECIMAL", // Decimal = 5,
            "FLOAT", // Float = 6,
            "IMAGE", // Image = 7,
            "INTEGER", // Int = 8,
            "MONEY", // Money = 9,
            "NCHAR", // NChar = 10,
            "NTEXT", // NText = 11,
            "NVARCHAR", // NVarChar = 12,
            "REAL", // Real = 13,
            "UNIQUEIDENTIFIER", // UniqueIdentifier = 14,
            "SMALLDATETIME", // SmallDateTime = 15,
            "SMALLINT", // SmallInt = 16,
            "SMALLMONEY", // SmallMoney = 17,
            "TEXT", // Text = 18,
            "TIMESTAMP", // Timestamp = 19,
            "TINYINT", // TinyInt = 20,
            "VARBINARY", // VarBinary = 21,
            "VARCHAR", // VarChar = 22,
            "SQL_VARIANT", // Variant = 23,
            "Xml", // Xml = 24,
            "DATE", // Date = 25,
            "TIME", // Time = 26,
            "DATETIME2", // DateTime2 = 27,
            "DateTimeOffset" // DateTimeOffset = 28,
        };

        public SqliteVisitor()
        {
            this.IdentifierOpenQuote = "\"";
            this.IdentifierCloseQuote = "\"";
            this.LiteralOpenQuote = "'";
            this.LiteralCloseQuote = "'";
            this.CommentOpenQuote = "/*";
            this.CommentCloseQuote = "*/";
        }

        protected override string[] SupportedDialects => supportedDialects;


        protected override void VisitJoinType(Joins join)
        {
            if (join == Joins.RightOuter || join == Joins.FullOuter)
            {
                throw new NotImplementedException("join " + join + " is not implemented on SQLite");
            }
            State.Write(JoinStrings[(int) join]);
        }


        protected override void VisitType(ITyped typedToken)
        {
            if (typedToken.DbType.HasValue)
            {
                State.Write(DbTypeStrings[(int) typedToken.DbType]);
            }
        }

        protected override void VisitStatement(IStatement statement)
        {
            if (statement is VacuumStatement)
            {
                VisitVacuumStatement((VacuumStatement) statement);
                return;
            }
            base.VisitStatement(statement);
        }

        protected override void VisitSet(SetStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDeclareStatement(DeclareStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitIfStatement(IfStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitStringifyStatement(StringifyStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitBreakStatement(BreakStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitContinueStatement(ContinueStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitExitStatement(ExitStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitGotoStatement(GotoStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitReturnStatement(ReturnStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitThrowStatement(ThrowStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitTryCatchStatement(TryCatchStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitLabelStatement(LabelStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitWaitforDelayStatement(WaitforDelayStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitWaitforTimeStatement(WaitforTimeStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitWhileStatement(WhileStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitPerformStatement(PerformStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDeallocateStatement(DeallocateStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitExecuteStatement(ExecuteStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitPrepareStatement(IExecutableStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitExecuteProcedureStatement(ExecuteProcedureStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitCreateSchemaStatement(CreateSchemaStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDropSchemaStatement(DropSchemaStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitAlterSchemaStatement(AlterSchemaStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitCreateProcedureStatement(CreateProcedureStatement statement)
        {
            throw new NotImplementedException();
        }


        protected override void VisitDropProcedureStatement(DropProcedureStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitAlterProcedureStatement(AlterProcedureStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitCreateFunctionStatement(CreateFunctionStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitAlterFunctionStatement(AlterFunctionStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitDropFunctionStatement(DropFunctionStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitExecuteFunctionStatement(ExecuteFunctionStatement statement)
        {
            throw new NotImplementedException();
        }


        void VisitVacuumStatement(VacuumStatement statement)
        {
            State.Write(SqliteSymbols.VACUUM);
        }


        protected class SqliteSymbols : Symbols
        {
            public const string hex = "hex";
            public const string localtime = "localtime";
            public const string now = "now";
            public const string randomblob = "randomblob";
            public const string utc = "utc";
            public const string current_timestamp = "current_timestamp";

            public const string strftime = "strftime";
            public const string floor = "floor";
            public const string datetime = "datetime";
            public const string printf = "printf";
            



            public const string VACUUM = "VACUUM";
            public const string INTEGER = "INTEGER";
            public const string REAL = "REAL";
            public const string TEXT = "TEXT";

            public const string IFNULL = "IFNULL";
            

            public const string d = "d";
            public const string m = "m";
            public const string H = "H";
            public const string Y = "Y";
            public const string M = "M";
            public const string S = "S";
            public const string f = "f";
            public const string W = "W";

            public const string DotVal = ".";
            public const string days = "days";
            public const string years = "years";
            public const string months = "months";
            public const string hours = "hours";
            public const string minutes = "minutes";
            public const string seconds = "seconds";


            public const string Concat = "||";

        }
    }
}