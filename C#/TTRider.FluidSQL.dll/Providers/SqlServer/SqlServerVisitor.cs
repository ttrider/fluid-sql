// <license>
// The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
// Copyright (c) 2014-2015 All Rights Reserved
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace TTRider.FluidSql.Providers.SqlServer
{
    internal class SqlServerVisitor : Visitor
    {
        private static readonly string[] SqlSupportedDialects = { "t-sql", "ansi" };

        private readonly string[] dbTypeStrings =
        {
            "BIGINT", // BigInt = 0,
            "BINARY", // Binary = 1,
            "BIT", // Bit = 2,
            "CHAR", // Char = 3,
            "DATETIME", // DateTime = 4,
            "DECIMAL", // Decimal = 5,
            "FLOAT", // Float = 6,
            "IMAGE", // Image = 7,
            "INT", // Int = 8,
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

        public SqlServerVisitor()
        {
            this.IdentifierOpenQuote = "[";
            this.IdentifierCloseQuote = "]";
            this.LiteralOpenQuote = "N'";
            this.LiteralCloseQuote = "'";
            this.CommentOpenQuote = "/*";
            this.CommentCloseQuote = "*/";
        }

        protected override string[] SupportedDialects => SqlSupportedDialects;


        internal static VisitorState Compile(Token token)
        {
            var visitor = new SqlServerVisitor();
            var statement = token as IStatement;
            if (statement != null)
            {
                return visitor.Compile(statement);
            }
            visitor.VisitToken(token);
            return visitor.State;
        }

        internal static Name GetTempTableName(Name name)
        {
            var namePart = name.LastPart;

            if (!namePart.StartsWith(Symbols.Pound))
            {
                namePart = Symbols.Pound + namePart;
            }
            return Sql.Name("tempdb", "", namePart);
        }

        internal static string GetTableVariableName(Name name)
        {
            var namePart = name.LastPart;

            if (!namePart.StartsWith(Symbols.At))
            {
                namePart = Symbols.At + namePart;
            }
            return namePart;
        }

        private void Stringify(IStatement statement)
        {
            Stringify(() => VisitStatement(statement));
        }

        private void Stringify(RecordsetSourceToken statement)
        {
            Stringify(() => VisitToken(statement));
        }
        private void Stringify(Action fragment)
        {
            State.WriteBeginStringify(LiteralOpenQuote, LiteralCloseQuote);
            fragment();
            State.WriteEndStringify();
        }


        protected override void VisitType(ITyped typedToken)
        {
            if (typedToken.DbType.HasValue)
            {
                State.Write(dbTypeStrings[(int)typedToken.DbType]);
            }

            if (typedToken.Length.HasValue || typedToken.Precision.HasValue || typedToken.Scale.HasValue)
            {
                State.Write(Symbols.OpenParenthesis);
                if (typedToken.Length.HasValue)
                {
                    State.Write(typedToken.Length.Value == -1
                        ? Symbols.MAX
                        : typedToken.Length.Value.ToString(CultureInfo.InvariantCulture));
                }
                else if (typedToken.Precision.HasValue)
                {
                    State.Write(typedToken.Precision.Value.ToString(CultureInfo.InvariantCulture));

                    if (typedToken.Scale.HasValue)
                    {
                        State.Write(Symbols.Comma);
                        State.Write(typedToken.Scale.Value.ToString(CultureInfo.InvariantCulture));
                    }
                }

                State.Write(Symbols.CloseParenthesis);
            }
        }

        private void VisitOutput(IEnumerable<ExpressionToken> columns, Name outputInto)
        {
            VisitAliasedTokenSet(columns, Symbols.OUTPUT, Symbols.Comma, null);
            if (outputInto != null)
            {
                State.Write(Symbols.INTO);
                VisitNameToken(outputInto);
            }
        }

        private void VisitTop(Top top)
        {
            if (top != null)
            {
                State.Write(Symbols.TOP);
                State.Write(Symbols.OpenParenthesis);
                if (top.Value != null)
                {
                    VisitToken(top.Value);
                }
                State.Write(Symbols.CloseParenthesis);

                if (top.Percent)
                {
                    State.Write(Symbols.PERCENT);
                }
                if (top.WithTies)
                {
                    State.Write(Symbols.WITH);
                    State.Write(Symbols.TIES);
                }
            }
        }


        private void VisitWith(bool? value, string name)
        {
            if (value.HasValue)
            {
                State.Write(name);
                State.Write(Symbols.AssignVal);
                State.Write(value.Value ? Symbols.ON : Symbols.OFF);
            }
        }

        private void VisitWith(int? value, string name)
        {
            if (value.HasValue)
            {
                State.Write(name);
                State.Write(Symbols.AssignVal);
                State.Write(value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }

        protected override void VisitStringifyToken(StringifyToken token)
        {
            Stringify(() => VisitToken(token.Content));
        }


        protected override void VisitCreateSchemaStatement(CreateSchemaStatement statement)
        {
            var schemaName = ResolveName(statement.Name);

            if (statement.CheckIfNotExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.NOT);
                State.Write(Symbols.EXISTS);
                State.Write(Symbols.OpenParenthesis);
                State.Write(Symbols.SELECT);
                State.Write(Symbols.Asterisk);
                State.Write(Symbols.FROM);
                State.Write("INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME =");
                State.Write(LiteralOpenQuote, statement.Name.LastPart, LiteralCloseQuote);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.BEGIN);
            }
            State.Write(Symbols.EXEC);
            State.Write(Symbols.OpenParenthesis);

            Stringify(() =>
            {
                State.Write(Symbols.CREATE);
                State.Write(Symbols.SCHEMA);
                State.Write(schemaName);
                if (!string.IsNullOrWhiteSpace(statement.Owner))
                {
                    State.Write(Symbols.AUTHORIZATION);
                    State.Write(this.IdentifierOpenQuote, statement.Owner, this.IdentifierCloseQuote);
                }
            });

            //AUTHORIZATION joe;

            State.Write(Symbols.CloseParenthesis);
            if (statement.CheckIfNotExists)
            {
                State.Write(Symbols.END);
            }

            State.WriteStatementTerminator();
        }

        protected override void VisitDropSchemaStatement(DropSchemaStatement statement)
        {
            var schemaName = ResolveName(statement.Name);

            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.EXISTS);
                State.Write(Symbols.OpenParenthesis);
                State.Write(Symbols.SELECT);
                State.Write(Symbols.Asterisk);
                State.Write(Symbols.FROM);
                State.Write("INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME =");
                State.Write(LiteralOpenQuote, statement.Name.LastPart, LiteralCloseQuote);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.BEGIN);
            }
            State.Write(Symbols.EXEC);
            State.Write(Symbols.OpenParenthesis);

            Stringify(() =>
            {
                State.Write(Symbols.DROP);
                State.Write(Symbols.SCHEMA);
                State.Write(schemaName);
            });
            State.Write(Symbols.CloseParenthesis);
            if (statement.CheckExists)
            {
                State.Write(Symbols.END);
            }

            State.WriteStatementTerminator();
        }

        protected override void VisitAlterSchemaStatement(AlterSchemaStatement statement)
        {
            throw new NotImplementedException();
        }


        /* FUNCTIONS */

        protected override void VisitNowFunctionToken(NowFunctionToken token)
        {
            if (token.Utc)
            {
                State.Write(SqlSymbols.GETUTCDATE);
                State.Write(Symbols.OpenParenthesis);
                State.Write(Symbols.CloseParenthesis);
            }
            else
            {
                State.Write(SqlSymbols.GETDATE);
                State.Write(Symbols.OpenParenthesis);
                State.Write(Symbols.CloseParenthesis);
            }
        }

        protected override void VisitUuidFunctionToken(UuidFunctionToken token)
        {
            State.Write(SqlSymbols.NEWID);
            State.Write(Symbols.OpenParenthesis);
            State.Write(Symbols.CloseParenthesis);
        }


        protected override void VisitIIFFunctionToken(IifFunctionToken token)
        {
            State.Write(Symbols.IIF);
            State.Write(Symbols.OpenParenthesis);
            VisitToken(token.ConditionToken);
            State.Write(Symbols.Comma);
            VisitToken(token.ThenToken);
            State.Write(Symbols.Comma);
            VisitToken(token.ElseToken);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDatePartFunctionToken(DatePartFunctionToken token)
        {
            State.Write(SqlSymbols.DATEPART);
            State.Write(Symbols.OpenParenthesis);

            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(SqlSymbols.d);
                    break;
                case DatePart.Year:
                    State.Write(SqlSymbols.yy);
                    break;
                case DatePart.Month:
                    State.Write(SqlSymbols.m);
                    break;
                case DatePart.Week:
                    State.Write(SqlSymbols.ww);
                    break;
                case DatePart.Hour:
                    State.Write(SqlSymbols.hh);
                    break;
                case DatePart.Minute:
                    State.Write(SqlSymbols.mi);
                    break;
                case DatePart.Second:
                    State.Write(SqlSymbols.ss);
                    break;
                case DatePart.Millisecond:
                    State.Write(SqlSymbols.ms);
                    break;
            }
            State.Write(Symbols.Comma);
            VisitToken(token.Token);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDateAddFunctionToken(DateAddFunctionToken token)
        {
            State.Write(SqlSymbols.DATEADD);
            State.Write(Symbols.OpenParenthesis);

            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(SqlSymbols.d);
                    break;
                case DatePart.Year:
                    State.Write(SqlSymbols.yy);
                    break;
                case DatePart.Month:
                    State.Write(SqlSymbols.m);
                    break;
                case DatePart.Week:
                    State.Write(SqlSymbols.ww);
                    break;
                case DatePart.Hour:
                    State.Write(SqlSymbols.hh);
                    break;
                case DatePart.Minute:
                    State.Write(SqlSymbols.mi);
                    break;
                case DatePart.Second:
                    State.Write(SqlSymbols.ss);
                    break;
                case DatePart.Millisecond:
                    State.Write(SqlSymbols.ms);
                    break;
            }
            State.Write(Symbols.Comma);
            VisitToken(token.Subtract ? new UnaryMinusToken { Token = token.Number } : token.Number);
            State.Write(Symbols.Comma);
            VisitToken(token.Token);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDurationFunctionToken(DurationFunctionToken token)
        {
            State.Write(SqlSymbols.DATEDIFF);
            State.Write(Symbols.OpenParenthesis);

            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(SqlSymbols.d);
                    break;
                case DatePart.Year:
                    State.Write(SqlSymbols.yy);
                    break;
                case DatePart.Month:
                    State.Write(SqlSymbols.m);
                    break;
                case DatePart.Week:
                    State.Write(SqlSymbols.ww);
                    break;
                case DatePart.Hour:
                    State.Write(SqlSymbols.hh);
                    break;
                case DatePart.Minute:
                    State.Write(SqlSymbols.mi);
                    break;
                case DatePart.Second:
                    State.Write(SqlSymbols.ss);
                    break;
                case DatePart.Millisecond:
                    State.Write(SqlSymbols.ms);
                    break;
            }
            State.Write(Symbols.Comma);
            VisitToken(token.Start);
            State.Write(Symbols.Comma);
            VisitToken(token.End);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitMakeDateFunctionToken(MakeDateFunctionToken token)
        {
            State.Write(SqlSymbols.DATETIMEFROMPARTS);
            State.Write(Symbols.OpenParenthesis);

            VisitToken(token.Year);
            State.Write(Symbols.Comma);
            VisitToken(token.Month);
            State.Write(Symbols.Comma);
            VisitToken(token.Day);
            State.Write(Symbols.Comma);

            if (token.Hour != null)
            {
                VisitToken(token.Hour);
            }
            else
            {
                State.Write("0");
            }
            State.Write(Symbols.Comma);

            if (token.Minute != null)
            {
                VisitToken(token.Minute);
            }
            else
            {
                State.Write("0");
            }
            State.Write(Symbols.Comma);

            if (token.Second != null)
            {
                VisitToken(token.Second);
            }
            else
            {
                State.Write("0");
            }
            State.Write(Symbols.Comma);
            if (token.Millisecond != null)
            {
                VisitToken(token.Millisecond);
            }
            else
            {
                State.Write("0");
            }
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitMakeTimeFunctionToken(MakeTimeFunctionToken token)
        {
            State.Write(SqlSymbols.TIMEFROMPARTS);
            State.Write(Symbols.OpenParenthesis);

            VisitToken(token.Hour);
            State.Write(Symbols.Comma);
            VisitToken(token.Minute);
            State.Write(Symbols.Comma);
            if (token.Second != null)
            {
                VisitToken(token.Second);
            }
            else
            {
                State.Write("0");
            }
            State.Write(Symbols.Comma);
            State.Write("0");
            State.Write(Symbols.Comma);
            State.Write("0");
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitCreateProcedureStatement(CreateProcedureStatement statement)
        {
            VisitConditional(
                statement.CheckIfNotExists,
                statement.Name,
                "P",
                false,
                () =>
                {
                    State.Write(Symbols.CREATE);
                    VisitProcedureParametersAndBody(statement);
                }
                );
        }


        protected override void VisitDropProcedureStatement(DropProcedureStatement statement)
        {
            VisitConditional(
                statement.CheckExists,
                statement.Name,
                "P",
                true,
                () =>
                {
                    State.Write(Symbols.DROP);
                    State.Write(Symbols.PROCEDURE);
                    VisitNameToken(statement.Name);
                }
                );
        }

        protected override void VisitAlterProcedureStatement(AlterProcedureStatement statement)
        {
            VisitConditional(
                statement.CreateIfNotExists,
                statement.Name,
                "P",
                false,
                () =>
                {
                    State.Write(Symbols.CREATE);
                    VisitProcedureParametersAndBody(statement);
                },
                () =>
                {
                    State.Write(Symbols.ALTER);
                    VisitProcedureParametersAndBody(statement);
                }
                );
        }

        protected override void VisitPrepareStatement(IExecutableStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitExecuteProcedureStatement(ExecuteProcedureStatement statement)
        {
            State.Write(Symbols.EXEC);

            var retVal = statement.Parameters.FirstOrDefault(p => p.Direction == ParameterDirection.ReturnValue);
            if (retVal != null)
            {
                State.Write(retVal.Name);
                State.Write(Symbols.AssignVal);

                State.Parameters.Add(retVal.Clone().ParameterDirection(ParameterDirection.Output));
            }

            VisitNameToken(statement.Name);

            VisitTokenSet(statement.Parameters.Where(p => p.Direction != ParameterDirection.ReturnValue),
                visitToken: parameter =>
                {
                    if (parameter.UseDefault)
                    {
                        State.Write(parameter.Name);
                        State.Write(Symbols.DEFAULT);
                    }
                    else
                    {
                        State.Write(parameter.Name);
                        State.Write(Symbols.AssignVal);
                        State.Write(parameter.Name);
                        if (parameter.Direction == ParameterDirection.Output ||
                            parameter.Direction == ParameterDirection.InputOutput)
                        {
                            State.Write(Symbols.OUTPUT);
                        }
                    }

                    State.Parameters.Add(parameter);
                });

            if (statement.Recompile)
            {
                State.Write(Symbols.WITH, Symbols.RECOMPILE);
            }
        }

        protected override void VisitCreateFunctionStatement(CreateFunctionStatement statement)
        {
            VisitConditional(
                statement.CheckIfNotExists,
                statement.Name,
                "FN",
                false,
                () =>
                {
                    State.Write(Symbols.CREATE);
                    VisitFunctionParametersAndBody(statement);
                }
                );
        }

        protected override void VisitAlterFunctionStatement(AlterFunctionStatement statement)
        {
            VisitConditional(
                statement.CheckIfNotExists,
                statement.Name,
                "FN",
                false,
                () =>
                {
                    State.Write(Symbols.CREATE);
                    VisitProcedureParametersAndBody(statement);
                },
                () =>
                {
                    State.Write(Symbols.ALTER);
                    VisitProcedureParametersAndBody(statement);
                }
                );
        }

        protected override void VisitDropFunctionStatement(DropFunctionStatement statement)
        {
            VisitConditional(
                statement.CheckExists,
                statement.Name,
                "FN",
                true,
                () =>
                {
                    State.Write(Symbols.DROP);
                    State.Write(Symbols.FUNCTION);
                    VisitNameToken(statement.Name);
                }
                );
        }

        protected override void VisitExecuteFunctionStatement(ExecuteFunctionStatement statement)
        {
            State.Write(Symbols.EXEC);

            var retVal = statement.Parameters.FirstOrDefault(p => p.Direction == ParameterDirection.ReturnValue);
            if (retVal != null)
            {
                State.Write(retVal.Name);
                State.Write(Symbols.AssignVal);
            }

            VisitNameToken(statement.Name);

            State.Write(Symbols.OpenParenthesis);

            VisitTokenSet(statement.Parameters.Where(p => p.Direction != ParameterDirection.ReturnValue),
                visitToken: parameter =>
                {
                    if (parameter.Value != null)
                    {
                        VisitValue(parameter.Value);
                    }
                    else
                    {
                        State.Write(parameter.Name);
                    }

                    State.Parameters.Add(parameter);
                });

            State.Write(Symbols.CloseParenthesis);
        }

        private void VisitProcedureParametersAndBody(IProcedureStatement s)
        {
            State.Write(Symbols.PROCEDURE);

            VisitNameToken(s.Name);
            State.WriteCRLF();

            var separator = Symbols.OpenParenthesis;
            foreach (var p in s.Parameters
                .Where(p => p.Direction != ParameterDirection.ReturnValue))
            {
                State.Write(separator);
                State.WriteCRLF();
                separator = Symbols.Comma;

                VisitNameToken(p.Name);
                VisitType(p);

                if (p.DefaultValue != null)
                {
                    State.Write(Symbols.AssignVal);
                    VisitValue(p.DefaultValue);
                }
                if (p.Direction != ParameterDirection.Input)
                {
                    State.Write(Symbols.OUTPUT);
                }
                if (p.ReadOnly)
                {
                    State.Write(Symbols.READONLY);
                }
            }
            if (separator == Symbols.Comma)
            {
                State.WriteCRLF();
                State.Write(Symbols.CloseParenthesis);
                State.WriteCRLF();
            }

            if (s.Recompile)
            {
                State.Write(Symbols.WITH, Symbols.RECOMPILE);
                State.WriteCRLF();
            }

            State.Write(Symbols.AS);
            State.WriteCRLF();
            State.Write(Symbols.BEGIN);
            State.WriteStatementTerminator();
            VisitStatement(s.Body);
            State.Write(Symbols.END);
            State.WriteStatementTerminator();
        }

        private void VisitFunctionParametersAndBody(IProcedureStatement s)
        {
            State.Write(Symbols.FUNCTION);

            VisitNameToken(s.Name);
            State.WriteCRLF();

            var separator = Symbols.OpenParenthesis;
            Parameter returnParam = s.Parameters
                .Where(p => p.Direction == ParameterDirection.ReturnValue).FirstOrDefault();

            foreach (var p in s.Parameters
                .Where(p => p.Direction != ParameterDirection.ReturnValue))
            {
                State.Write(separator);
                State.WriteCRLF();
                separator = Symbols.Comma;

                VisitNameToken(p.Name);
                VisitType(p);

                if (p.DefaultValue != null)
                {
                    State.Write(Symbols.AssignVal);
                    VisitValue(p.DefaultValue);
                }
                if ((p.Direction != 0) && (p.Direction != ParameterDirection.Input))
                {
                    State.Write(Symbols.OUTPUT);
                }
                if (p.ReadOnly)
                {
                    State.Write(Symbols.READONLY);
                }
            }
            if (separator == Symbols.Comma)
            {
                State.WriteCRLF();
                State.Write(Symbols.CloseParenthesis);
                State.WriteCRLF();
            }
            if (returnParam != null)
            {
                State.Write(Symbols.RETURNS);
                VisitNameToken(returnParam.Name);
                VisitType(returnParam);
                State.WriteCRLF();
            }

            if (s.Recompile)
            {
                State.Write(Symbols.WITH, Symbols.RECOMPILE);
                State.WriteCRLF();
            }

            State.Write(Symbols.AS);
            State.WriteCRLF();
            State.Write(Symbols.BEGIN);
            State.WriteStatementTerminator();
            VisitStatement(s.Body);
            State.Write(Symbols.END);
            State.WriteStatementTerminator();
        }

        private void VisitConditional(bool doCheck, Name name, string objectType, bool inverse, Action isNullAction,
            Action isNotNullAction = null)
        {
            if (doCheck)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.OBJECT_ID);
                State.Write(Symbols.OpenParenthesis);
                State.Write(LiteralOpenQuote, ResolveName(name), LiteralCloseQuote);
                State.Write(Symbols.Comma);
                State.Write(LiteralOpenQuote, objectType, LiteralCloseQuote);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.IS);
                if (inverse)
                {
                    State.Write(Symbols.NOT);
                }
                State.Write(Symbols.NULL);
                State.WriteCRLF();
                State.Write(Symbols.BEGIN);
                State.WriteStatementTerminator();

                State.Write(Symbols.EXEC);
                State.Write(Symbols.OpenParenthesis);
                State.WriteBeginStringify(LiteralOpenQuote, LiteralCloseQuote);

                isNullAction();

                State.WriteEndStringify();
                State.Write(Symbols.CloseParenthesis);
                State.WriteStatementTerminator();

                State.Write(Symbols.END);
                State.WriteStatementTerminator();

                if (isNotNullAction != null)
                {
                    State.Write(Symbols.ELSE);
                    State.Write(Symbols.BEGIN);
                    State.WriteStatementTerminator();

                    State.Write(Symbols.EXEC);
                    State.Write(Symbols.OpenParenthesis);
                    State.WriteBeginStringify(LiteralOpenQuote, LiteralCloseQuote);

                    isNotNullAction();

                    State.WriteEndStringify();
                    State.Write(Symbols.CloseParenthesis);
                    State.WriteStatementTerminator();


                    State.Write(Symbols.END);
                    State.WriteStatementTerminator();
                }
            }
            else
            {
                isNullAction();
            }
        }

        protected class SqlSymbols : Symbols
        {
            // ReSharper disable InconsistentNaming
            public const string DATEADD = "DATEADD";
            public const string DATEDIFF = "DATEDIFF";
            public const string DATEPART = "DATEPART";
            public const string DATETIMEFROMPARTS = "DATETIMEFROMPARTS";
            public const string GETDATE = "GETDATE";
            public const string GETUTCDATE = "GETUTCDATE";
            public const string IDENTITY_INSERT = "IDENTITY_INSERT";
            public const string NEWID = "NEWID";
            public const string TIMEFROMPARTS = "TIMEFROMPARTS";
            public const string d = "d";
            public const string hh = "hh";
            public const string m = "m";
            public const string mi = "mi";
            public const string ms = "ms";
            public const string s = "s";
            public const string ss = "ss";
            public const string ww = "ww";
            public const string yy = "yy";
            // ReSharper restore InconsistentNaming
        }

        #region Statements

        protected override void VisitJoinType(Joins join)
        {
            State.Write(JoinStrings[(int)join]);
        }

        protected override void VisitSet(SetStatement statement)
        {
            State.Write(Symbols.SET);

            if (statement.Assign != null)
            {
                VisitToken(statement.Assign);
            }
        }

        protected override void VisitMerge(MergeStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions);

            State.Write(Symbols.MERGE);

            VisitTop(statement.Top);

            VisitInto(statement.Into);

            VisitAlias(statement.Into.Alias);

            State.Write(Symbols.USING);
            VisitToken(statement.Using);

            VisitAlias((statement.Using as IAliasToken)?.Alias);

            State.Write(Symbols.ON);

            VisitToken(statement.On);

            foreach (var when in statement.WhenMatched)
            {
                State.Write(Symbols.WHEN);
                State.Write(Symbols.MATCHED);
                if (when.AndCondition != null)
                {
                    State.Write(Symbols.AND);
                    VisitToken(when.AndCondition);
                }
                State.Write(Symbols.THEN);

                VisitToken(when);
            }

            foreach (var when in statement.WhenNotMatched)
            {
                State.Write(Symbols.WHEN);
                State.Write(Symbols.NOT);
                State.Write(Symbols.MATCHED);
                State.Write(Symbols.BY);
                State.Write(Symbols.TARGET);
                if (when.AndCondition != null)
                {
                    State.Write(Symbols.AND);
                    VisitToken(when.AndCondition);
                }
                State.Write(Symbols.THEN);

                VisitToken(when);
            }

            foreach (var when in statement.WhenNotMatchedBySource)
            {
                State.Write(Symbols.WHEN);
                State.Write(Symbols.NOT);
                State.Write(Symbols.MATCHED);
                State.Write(Symbols.BY);
                State.Write(Symbols.SOURCE);
                if (when.AndCondition != null)
                {
                    State.Write(Symbols.AND);
                    VisitToken(when.AndCondition);
                }
                State.Write(Symbols.THEN);

                VisitToken(when);
            }

            VisitOutput(statement.Output, statement.OutputInto);
        }

        // ReSharper disable once UnusedParameter.Local
        protected override void VisitWhenMatchedThenDelete(WhenMatchedTokenThenDeleteToken token)
        {
            State.Write(Symbols.DELETE);
        }

        protected override void VisitWhenMatchedThenUpdateSet(WhenMatchedTokenThenUpdateSetToken token)
        {
            State.Write(Symbols.UPDATE);
            State.Write(Symbols.SET);
            VisitTokenSet(token.Set);
        }

        protected override void VisitWhenNotMatchedThenInsert(WhenNotMatchedTokenThenInsertToken token)
        {
            State.Write(Symbols.INSERT);
            if (token.Columns.Count > 0)
            {
                VisitTokenSetInParenthesis(token.Columns);
            }
            if (token.Values.Count > 0)
            {
                VisitTokenSetInParenthesis(token.Values, () => State.Write(Symbols.VALUES));
            }
            else
            {
                State.Write(Symbols.DEFAULT);
                State.Write(Symbols.VALUES);
            }
        }

        protected override void VisitSelect(SelectStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions);

            State.Write(Symbols.SELECT);

            if (statement.Distinct)
            {
                State.Write(Symbols.DISTINCT);
            }

            if (statement.Offset == null)
            {
                VisitTop(statement.Top);
            }

            // assignments
            if (statement.Set.Count > 0)
            {
                VisitTokenSet(statement.Set);
            }
            else
            {
                // output columns
                if (statement.Output.Count == 0)
                {
                    State.Write(Symbols.Asterisk);
                }
                else
                {
                    VisitAliasedTokenSet(statement.Output, null, Symbols.Comma, null);
                }
            }

            VisitInto(statement.Into);

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

            if (statement.Offset != null)
            {
                State.Write(Symbols.OFFSET);
                VisitToken(statement.Offset);
                State.Write(Symbols.ROWS);
                State.Write(Symbols.FETCH);
                State.Write(Symbols.NEXT);
                if (statement.Top.Value != null)
                {
                    VisitToken(statement.Top.Value);
                }
                State.Write(Symbols.ROWS);
                State.Write(Symbols.ONLY);
            }

            //WITH CUBE or WITH ROLLUP
        }

        private void VisitInto(Name into)
        {
            if (into != null)
            {
                State.Write(Symbols.INTO);
                VisitNameToken(into);
            }
        }

        protected override void VisitDelete(DeleteStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions);

            State.Write(Symbols.DELETE);

            VisitTop(statement.Top);

            // if 'FROM' has an alias or joins , we need to re-arrange tokens in a statement
            // even more, if it has joins, it MUST have an alias :)
            var hasAlias = !string.IsNullOrWhiteSpace(statement.RecordsetSource?.Alias);

            if (statement.Joins.Count > 0 || hasAlias)
            {
                if (hasAlias)
                {
                    State.Write(this.IdentifierOpenQuote, statement.RecordsetSource.Alias, this.IdentifierCloseQuote);
                }
                VisitOutput(statement.Output, statement.OutputInto);

                State.Write(Symbols.FROM);

                VisitFromToken(statement.RecordsetSource);

                VisitJoin(statement.Joins);

                VisitWhereToken(statement.Where);
            }
            else
            {
                State.Write(Symbols.FROM);

                VisitFromToken(statement.RecordsetSource);

                VisitOutput(statement.Output, statement.OutputInto);

                VisitWhereToken(statement.Where);
            }
        }

        protected override void VisitUpdate(UpdateStatement statement)
        {
            VisitCommonTableExpressions(statement.CommonTableExpressions);

            State.Write(Symbols.UPDATE);

            VisitTop(statement.Top);

            VisitToken(statement.Target, true);

            State.Write(Symbols.SET);
            VisitTokenSet(statement.Set);

            VisitOutput(statement.Output, statement.OutputInto);

            VisitFromToken(statement.RecordsetSource);

            VisitJoin(statement.Joins);

            VisitWhereToken(statement.Where);
        }

        protected override void VisitInsert(InsertStatement statement)
        {
            if (statement.IdentityInsert)
            {
                //SET IDENTITY_INSERT my_table ON
                State.Write(Symbols.SET);
                State.Write(SqlSymbols.IDENTITY_INSERT);
                VisitNameToken(statement.Into);
                State.Write(Symbols.ON);
                State.WriteStatementTerminator();
            }

            VisitCommonTableExpressions(statement.CommonTableExpressions);

            State.Write(Symbols.INSERT);

            VisitTop(statement.Top);

            VisitInto(statement.Into);

            VisitAliasedTokenSet(statement.Columns, Symbols.OpenParenthesis, Symbols.Comma, Symbols.CloseParenthesis);

            VisitOutput(statement.Output, statement.OutputInto);

            if (statement.DefaultValues)
            {
                State.Write(Symbols.DEFAULT);
                State.Write(Symbols.VALUES);
            }
            else if (statement.Values.Count > 0)
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
            }

            if (statement.IdentityInsert)
            {
                State.WriteStatementTerminator();
                //SET IDENTITY_INSERT my_table OFF
                State.Write(Symbols.SET);
                State.Write(SqlSymbols.IDENTITY_INSERT);
                VisitNameToken(statement.Into);
                State.Write(Symbols.OFF);
                State.WriteStatementTerminator();
            }
        }

        protected override void VisitBeginTransaction(BeginTransactionStatement statement)
        {
            State.Write(Symbols.BEGIN);
            State.Write(Symbols.TRANSACTION);
            if (VisitTransactionName(statement) && !string.IsNullOrWhiteSpace(statement.Description))
            {
                State.Write(Symbols.WITH);
                State.Write(Symbols.MARK);
                State.Write(this.LiteralOpenQuote, statement.Description, this.LiteralCloseQuote);
            }
        }

        protected override void VisitCommitTransaction(CommitTransactionStatement statement)
        {
            State.Write(Symbols.COMMIT);
            State.Write(Symbols.TRANSACTION);
            VisitTransactionName(statement);
        }

        protected override void VisitRollbackTransaction(RollbackTransactionStatement statement)
        {
            State.Write(Symbols.ROLLBACK);
            State.Write(Symbols.TRANSACTION);
            VisitTransactionName(statement);
        }

        protected override void VisitSaveTransaction(SaveTransactionStatement statement)
        {
            State.Write(Symbols.SAVE);
            State.Write(Symbols.TRANSACTION);
            VisitTransactionName(statement);
        }

        protected override void VisitDeclareStatement(DeclareStatement statement)
        {
            if (statement.Variable != null)
            {
                State.Variables.Add(statement.Variable);

                State.Write(Symbols.DECLARE);
                State.Write(statement.Variable.Name);

                VisitType(statement.Variable);

                if (statement.Initializer != null)
                {
                    State.Write(Symbols.AssignVal);
                    VisitToken(statement.Initializer);
                }
            }
        }

        // ReSharper disable once UnusedParameter.Local
        protected override void VisitBreakStatement(BreakStatement statement)
        {
            State.Write(Symbols.BREAK);
        }

        // ReSharper disable once UnusedParameter.Local
        protected override void VisitContinueStatement(ContinueStatement statement)
        {
            State.Write(Symbols.CONTINUE);
        }

        protected override void VisitExitStatement(ExitStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitGotoStatement(GotoStatement statement)
        {
            State.Write(Symbols.GOTO);
            State.Write(statement.Label);
        }

        protected override void VisitReturnStatement(ReturnStatement statement)
        {
            State.Write(Symbols.RETURN);
            if (statement.ReturnExpression != null)
            {
                VisitToken(statement.ReturnExpression);
            }
        }

        protected override void VisitThrowStatement(ThrowStatement statement)
        {
            State.Write(Symbols.THROW);
            if (statement.ErrorNumber != null && statement.Message != null && statement.State != null)
            {
                VisitToken(statement.ErrorNumber);
                State.Write(Symbols.Comma);
                VisitToken(statement.Message);
                State.Write(Symbols.Comma);
                VisitToken(statement.State);
            }
        }

        protected override void VisitTryCatchStatement(TryCatchStatement stmt)
        {
            State.Write(Symbols.BEGIN);
            State.Write(Symbols.TRY);
            State.WriteCRLF();
            VisitStatement(stmt.TryStatement);
            State.WriteStatementTerminator();
            State.Write(Symbols.END);
            State.Write(Symbols.TRY);
            State.WriteCRLF();
            State.Write(Symbols.BEGIN);
            State.Write(Symbols.CATCH);
            State.WriteCRLF();
            if (stmt.CatchStatement != null)
            {
                VisitStatement(stmt.CatchStatement);
                State.WriteStatementTerminator();
            }
            State.Write(Symbols.END);
            State.Write(Symbols.CATCH);
            State.WriteStatementTerminator();
        }

        protected override void VisitLabelStatement(LabelStatement stmt)
        {
            State.Write(stmt.Label, Symbols.Colon);
        }

        protected override void VisitWaitforDelayStatement(WaitforDelayStatement stmt)
        {
            State.Write(Symbols.WAITFOR);
            State.Write(Symbols.DELAY);
            State.Write(LiteralOpenQuote, stmt.Delay.ToString("HH:mm:ss"), LiteralCloseQuote);
        }

        protected override void VisitWaitforTimeStatement(WaitforTimeStatement stmt)
        {
            State.Write(Symbols.WAITFOR);
            State.Write(Symbols.TIME);
            State.Write(LiteralOpenQuote, stmt.Time.ToString("yyyy-MM-ddTHH:mm:ss"), LiteralCloseQuote);
        }

        protected override void VisitWhileStatement(WhileStatement stmt)
        {
            if (stmt.Condition != null)
            {
                State.Write(Symbols.WHILE);
                VisitToken(stmt.Condition);

                if (stmt.Do != null)
                {
                    State.WriteCRLF();
                    State.Write(Symbols.BEGIN);
                    State.WriteStatementTerminator();

                    VisitStatement(stmt.Do);
                    State.WriteStatementTerminator();

                    State.Write(Symbols.END);
                    State.WriteStatementTerminator();
                }
            }
        }

        protected override void VisitIfStatement(IfStatement ifs)
        {
            if (ifs.Condition != null)
            {
                State.Write(Symbols.IF);
                VisitToken(ifs.Condition);

                if (ifs.Then != null)
                {
                    State.WriteCRLF();
                    State.Write(Symbols.BEGIN);
                    State.WriteStatementTerminator();

                    VisitStatement(ifs.Then);
                    State.WriteStatementTerminator();

                    State.Write(Symbols.END);
                    State.WriteStatementTerminator();

                    if (ifs.Else != null)
                    {
                        State.Write(Symbols.ELSE);
                        State.WriteCRLF();
                        State.Write(Symbols.BEGIN);
                        State.WriteStatementTerminator();

                        VisitStatement(ifs.Else);
                        State.WriteStatementTerminator();

                        State.Write(Symbols.END);
                        State.WriteStatementTerminator();
                    }
                }
            }
        }

        protected override void VisitDropTableStatement(DropTableStatement statement)
        {
            var tableName = ResolveName((statement.IsTemporary) ? GetTempTableName(statement.Name) : statement.Name);

            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.OBJECT_ID);
                State.Write(Symbols.OpenParenthesis);
                State.Write(LiteralOpenQuote, tableName, LiteralCloseQuote);
                State.Write(Symbols.Comma);
                State.Write(LiteralOpenQuote, "U", LiteralCloseQuote);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.IS);
                State.Write(Symbols.NOT);
                State.Write(Symbols.NULL);
            }

            State.Write(Symbols.DROP);
            State.Write(Symbols.TABLE);
            State.Write(tableName);
        }

        protected override void VisitCreateTableStatement(CreateTableStatement createStatement)
        {
            if (createStatement.IsTableVariable)
            {
                State.Write(Symbols.DECLARE);
                State.Write(GetTableVariableName(createStatement.Name));
                State.Write(Symbols.TABLE);
            }
            else
            {
                var tableName =
                    ResolveName((createStatement.IsTemporary)
                        ? GetTempTableName(createStatement.Name)
                        : createStatement.Name);

                if (createStatement.CheckIfNotExists)
                {
                    State.Write(Symbols.IF);
                    State.Write(Symbols.OBJECT_ID);
                    State.Write(Symbols.OpenParenthesis);
                    State.Write(LiteralOpenQuote, tableName, LiteralCloseQuote);
                    State.Write(Symbols.Comma);
                    State.Write(LiteralOpenQuote, "U", LiteralCloseQuote);
                    State.Write(Symbols.CloseParenthesis);
                    State.Write(Symbols.IS);
                    State.Write(Symbols.NULL);

                    State.WriteCRLF();
                    State.Write(Symbols.BEGIN);
                    State.WriteStatementTerminator();
                }

                State.Write(Symbols.CREATE);
                State.Write(Symbols.TABLE);
                State.Write(tableName);
                if (createStatement.AsFiletable)
                {
                    State.Write(Symbols.AS);
                    State.Write(Symbols.FILETABLE);
                }
            }


            var separator = Symbols.OpenParenthesis;
            foreach (var column in createStatement.Columns)
            {
                State.Write(separator);
                separator = Symbols.Comma;

                State.Write(this.IdentifierOpenQuote, column.Name, this.IdentifierCloseQuote);

                VisitType(column);

                if (column.Sparse)
                {
                    State.Write(Symbols.SPARSE);
                }
                if (column.Null.HasValue)
                {
                    if (!column.Null.Value)
                    {
                        State.Write(Symbols.NOT);
                    }
                    State.Write(Symbols.NULL);
                }
                if (column.Identity.On)
                {
                    State.Write(Symbols.IDENTITY);
                    State.Write(Symbols.OpenParenthesis);
                    State.Write(column.Identity.Seed.ToString(CultureInfo.InvariantCulture));
                    State.Write(Symbols.Comma);
                    State.Write(column.Identity.Increment.ToString(CultureInfo.InvariantCulture));
                    State.Write(Symbols.CloseParenthesis);
                }
                if (column.RowGuid)
                {
                    State.Write(Symbols.ROWGUIDCOL);
                }
                if (column.DefaultValue != null)
                {
                    State.Write(Symbols.DEFAULT);
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(column.DefaultValue);
                    State.Write(Symbols.CloseParenthesis);
                }
            }

            if (createStatement.PrimaryKey != null)
            {
                State.Write(Symbols.Comma);
                if (!createStatement.IsTableVariable)
                {
                    State.Write(Symbols.CONSTRAINT);
                    VisitNameToken(createStatement.PrimaryKey.Name);
                }

                State.Write(Symbols.PRIMARY);
                State.Write(Symbols.KEY);
                VisitTokenSetInParenthesis(createStatement.PrimaryKey.Columns);
            }

            foreach (var unique in createStatement.UniqueConstrains)
            {
                State.Write(Symbols.Comma);
                if (!createStatement.IsTableVariable)
                {
                    State.Write(Symbols.CONSTRAINT);
                    VisitNameToken(unique.Name);
                }

                State.Write(Symbols.UNIQUE);
                if (unique.Clustered.HasValue)
                {
                    State.Write(unique.Clustered.Value ? Symbols.CLUSTERED : Symbols.NONCLUSTERED);
                }
                VisitTokenSetInParenthesis(unique.Columns);
            }

            State.Write(Symbols.CloseParenthesis);
            State.WriteStatementTerminator();

            // if indecies are set, create them
            if (createStatement.Indicies.Count > 0 && !createStatement.IsTableVariable)
            {
                foreach (var createIndexStatement in createStatement.Indicies)
                {
                    VisitCreateIndexStatement(createIndexStatement);
                    State.WriteStatementTerminator();
                }
            }

            if (createStatement.CheckIfNotExists && !createStatement.IsTableVariable)
            {
                State.Write(Symbols.END);
            }
        }

        protected override void VisitStringifyStatement(StringifyStatement statement)
        {
            this.Stringify(statement.Content);
        }

        protected override void VisitDeallocateStatement(DeallocateStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitExecuteStatement(ExecuteStatement statement)
        {
            State.Write(Symbols.EXEC);
            State.Write(Symbols.OpenParenthesis);
            this.Stringify(statement.Target.Target);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitCreateIndexStatement(CreateIndexStatement createIndexStatement)
        {
            State.Write(Symbols.CREATE);

            if (createIndexStatement.Unique)
            {
                State.Write(Symbols.UNIQUE);
            }

            if (createIndexStatement.Clustered.HasValue)
            {
                State.Write(createIndexStatement.Clustered.Value ? Symbols.CLUSTERED : Symbols.NONCLUSTERED);
            }
            State.Write(Symbols.INDEX);

            VisitToken(createIndexStatement.Name);

            State.Write(Symbols.ON);

            VisitToken(createIndexStatement.On);

            // columns
            VisitTokenSetInParenthesis(createIndexStatement.Columns);

            VisitTokenSetInParenthesis(createIndexStatement.Include, () => State.Write(Symbols.INCLUDE));

            VisitWhereToken(createIndexStatement.Where);

            if (createIndexStatement.With.IsDefined)
            {
                State.Write(Symbols.WITH);
                State.Write(Symbols.OpenParenthesis);

                VisitWith(createIndexStatement.With.PadIndex, Symbols.PAD_INDEX);
                VisitWith(createIndexStatement.With.Fillfactor, Symbols.FILLFACTOR);
                VisitWith(createIndexStatement.With.SortInTempdb, Symbols.SORT_IN_TEMPDB);
                VisitWith(createIndexStatement.With.IgnoreDupKey, Symbols.IGNORE_DUP_KEY);
                VisitWith(createIndexStatement.With.StatisticsNorecompute, Symbols.STATISTICS_NORECOMPUTE);
                VisitWith(createIndexStatement.With.DropExisting, Symbols.DROP_EXISTING);
                VisitWith(createIndexStatement.With.Online, Symbols.ONLINE);
                VisitWith(createIndexStatement.With.AllowRowLocks, Symbols.ALLOW_ROW_LOCKS);
                VisitWith(createIndexStatement.With.AllowPageLocks, Symbols.ALLOW_PAGE_LOCKS);
                VisitWith(createIndexStatement.With.MaxDegreeOfParallelism, Symbols.MAXDOP);

                State.Write(Symbols.CloseParenthesis);
            }

            State.Write(Symbols.Semicolon);
        }

        protected override void VisitAlterIndexStatement(AlterIndexStatement alterStatement)
        {
            State.Write(Symbols.ALTER);
            State.Write(Symbols.INDEX);

            if (alterStatement.Name == null)
            {
                State.Write(Symbols.ALL);
            }
            else
            {
                VisitToken(alterStatement.Name);
            }

            State.Write(Symbols.ON);

            VisitToken(alterStatement.On);

            if (alterStatement.Rebuild)
            {
                State.Write(Symbols.REBUILD);

                //TODO: [PARTITION = ALL]
                if (alterStatement.RebuildWith.IsDefined)
                {
                    State.Write(Symbols.WITH);
                    State.Write(Symbols.OpenParenthesis);

                    VisitWith(alterStatement.RebuildWith.PadIndex, Symbols.PAD_INDEX);
                    VisitWith(alterStatement.RebuildWith.Fillfactor, Symbols.FILLFACTOR);
                    VisitWith(alterStatement.RebuildWith.SortInTempdb, Symbols.SORT_IN_TEMPDB);
                    VisitWith(alterStatement.RebuildWith.IgnoreDupKey, Symbols.IGNORE_DUP_KEY);
                    VisitWith(alterStatement.RebuildWith.StatisticsNorecompute, Symbols.STATISTICS_NORECOMPUTE);
                    VisitWith(alterStatement.RebuildWith.DropExisting, Symbols.DROP_EXISTING);
                    VisitWith(alterStatement.RebuildWith.Online, Symbols.ONLINE);
                    VisitWith(alterStatement.RebuildWith.AllowRowLocks, Symbols.ALLOW_ROW_LOCKS);
                    VisitWith(alterStatement.RebuildWith.AllowPageLocks, Symbols.ALLOW_PAGE_LOCKS);
                    VisitWith(alterStatement.RebuildWith.MaxDegreeOfParallelism, Symbols.MAXDOP);

                    State.Write(Symbols.CloseParenthesis);
                }
            }
            else if (alterStatement.Disable)
            {
                State.Write(Symbols.DISABLE);
            }
            else if (alterStatement.Reorganize)
            {
                State.Write(Symbols.REORGANIZE);
            }
            else
            {
                VisitWith(alterStatement.Set.AllowRowLocks, Symbols.ALLOW_ROW_LOCKS);
                VisitWith(alterStatement.Set.AllowPageLocks, Symbols.ALLOW_PAGE_LOCKS);
                VisitWith(alterStatement.Set.IgnoreDupKey, Symbols.IGNORE_DUP_KEY);
                VisitWith(alterStatement.Set.StatisticsNorecompute, Symbols.STATISTICS_NORECOMPUTE);
            }
        }

        protected override void VisitDropIndexStatement(DropIndexStatement dropIndexStatement)
        {
            State.Write(Symbols.DROP);
            State.Write(Symbols.INDEX);
            VisitToken(dropIndexStatement.Name);

            State.Write(Symbols.ON);

            VisitToken(dropIndexStatement.On);

            if (dropIndexStatement.With.IsDefined)
            {
                State.Write(Symbols.WITH);
                State.Write(Symbols.OpenParenthesis);

                if (dropIndexStatement.With.Online.HasValue)
                {
                    State.Write(Symbols.ONLINE);
                    State.Write(Symbols.AssignVal);
                    State.Write(dropIndexStatement.With.Online.Value ? Symbols.ON : Symbols.OFF);
                }
                if (dropIndexStatement.With.MaxDegreeOfParallelism.HasValue)
                {
                    State.Write(Symbols.MAXDOP);
                    State.Write(Symbols.AssignVal);
                    State.Write(
                        dropIndexStatement.With.MaxDegreeOfParallelism.Value.ToString(CultureInfo.InvariantCulture));
                }

                State.Write(Symbols.CloseParenthesis);
            }
        }

        protected override void VisitCreateViewStatement(CreateViewStatement createStatement)
        {
            var viewName = ResolveName(createStatement.Name);

            if (createStatement.CheckIfNotExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.OBJECT_ID);
                State.Write(Symbols.OpenParenthesis);
                State.Write(LiteralOpenQuote, viewName, LiteralCloseQuote);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.IS);
                State.Write(Symbols.NULL);
                State.Write(Symbols.EXEC);
                State.Write(Symbols.OpenParenthesis);

                Stringify(() =>
                {
                    State.Write(Symbols.CREATE);
                    State.Write(Symbols.VIEW);
                    State.Write(viewName);
                    State.Write(Symbols.AS);
                    VisitStatement(createStatement.DefinitionStatement);
                });
                State.Write(Symbols.CloseParenthesis);
            }
            else
            {
                State.Write(Symbols.CREATE);
                State.Write(Symbols.VIEW);
                State.Write(viewName);
                State.Write(Symbols.AS);
                VisitStatement(createStatement.DefinitionStatement);
            }
        }

        protected override void VisitCreateOrAlterViewStatement(CreateOrAlterViewStatement createStatement)
        {
            var viewName = ResolveName(createStatement.Name);
            State.Write(Symbols.IF);
            State.Write(Symbols.OBJECT_ID);
            State.Write(Symbols.OpenParenthesis);
            State.Write(LiteralOpenQuote, viewName, LiteralCloseQuote);
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.IS);
            State.Write(Symbols.NULL);
            State.Write(Symbols.EXEC);
            State.Write(Symbols.OpenParenthesis);

            Stringify(() =>
            {
                State.Write(Symbols.CREATE);
                State.Write(Symbols.VIEW);
                State.Write(viewName);
                State.Write(Symbols.AS);
                VisitStatement(createStatement.DefinitionStatement);
            });

            State.Write(Symbols.CloseParenthesis);
            State.WriteStatementTerminator();
            State.Write(Symbols.ELSE);
            State.Write(Symbols.EXEC);
            State.Write(Symbols.OpenParenthesis);

            Stringify(() =>
            {
                State.Write(Symbols.ALTER);
                State.Write(Symbols.VIEW);
                State.Write(viewName);
                State.Write(Symbols.AS);
                VisitStatement(createStatement.DefinitionStatement);
            });

            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDropViewStatement(DropViewStatement statement)
        {
            var viewName = ResolveName(statement.Name);

            if (statement.CheckExists)
            {
                State.Write(Symbols.IF);
                State.Write(Symbols.OBJECT_ID);
                State.Write(Symbols.OpenParenthesis);
                State.Write(LiteralOpenQuote, viewName, LiteralCloseQuote);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.IS);
                State.Write(Symbols.NOT);
                State.Write(Symbols.NULL);
                State.Write(Symbols.EXEC);
                State.Write(Symbols.OpenParenthesis);

                Stringify(() =>
                {
                    State.Write(Symbols.DROP);
                    State.Write(Symbols.VIEW);
                    State.Write(viewName);
                    State.WriteStatementTerminator(false);
                });

                State.Write(Symbols.CloseParenthesis);
            }
            else
            {
                State.Write(Symbols.DROP);
                State.Write(Symbols.VIEW);
                VisitNameToken(statement.Name);
            }
        }

        protected override void VisitPerformStatement(PerformStatement statement)
        {
            throw new NotImplementedException();
        }

        protected override void VisitAlterViewStatement(AlterViewStatement statement)
        {
            State.Write(Symbols.ALTER);
            State.Write(Symbols.VIEW);
            VisitNameToken(statement.Name);
            State.Write(Symbols.AS);
            VisitStatement(statement.DefinitionStatement);
        }

        #endregion Statements
    }
}