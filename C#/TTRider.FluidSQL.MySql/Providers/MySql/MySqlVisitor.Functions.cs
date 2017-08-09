using System;

namespace TTRider.FluidSql.Providers.MySql
{
    internal partial class MySqlVisitor
    {
        protected override void VisitCastToken(CastToken token)
        {
            State.Write(Symbols.CAST, Symbols.OpenParenthesis);
            VisitToken(token.Token);
            State.Write(Symbols.AS);

            // cast supports only these data types:
            // BINARY[(N)]
            // CHAR[(N)][charset_info]
            // DATE
            // DATETIME
            // DECIMAL[(M[, D])]
            // NCHAR[(N)]
            // SIGNED[INTEGER]
            // TIME
            // UNSIGNED[INTEGER]

            CommonDbType? targetType;

            switch (token.DbType)
            {
                case CommonDbType.Bit:
                case CommonDbType.BigInt:
                case CommonDbType.SmallInt:
                case CommonDbType.TinyInt:
                    targetType = CommonDbType.Int;
                    break;
                case CommonDbType.Float:
                case CommonDbType.Money:
                case CommonDbType.Real:
                case CommonDbType.SmallMoney:
                    targetType = CommonDbType.Decimal;
                    break;
                case CommonDbType.NText:
                case CommonDbType.NVarChar:
                case CommonDbType.Xml:
                    targetType = CommonDbType.NChar;
                    break;
                case CommonDbType.UniqueIdentifier:
                case CommonDbType.Text:
                case CommonDbType.VarChar:
                    targetType = CommonDbType.Char;
                    break;
                case CommonDbType.SmallDateTime:
                case CommonDbType.Timestamp:
                case CommonDbType.Date:
                case CommonDbType.DateTime2:
                case CommonDbType.DateTimeOffset:
                    targetType = CommonDbType.DateTime;
                    break;
                case CommonDbType.VarBinary:
                case CommonDbType.Image:
                case CommonDbType.Variant:
                    targetType = CommonDbType.Binary;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (targetType != token.DbType)
            {
                var clonedToken = new CastToken
                {
                    Alias = token.Alias,
                    DbType = targetType,
                    Length = token.Length,
                    Precision = token.Precision,
                    Scale = token.Scale,
                    Token = token.Token,
                };
                foreach (var parameter in token.Parameters)
                {
                    clonedToken.Parameters.Add(parameter);
                }
                foreach (var parameterValue in token.ParameterValues)
                {
                    clonedToken.ParameterValues.Add(parameterValue);
                }
                VisitType(clonedToken);
            }
            else
            {
                VisitType(token);
            }
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitNowFunctionToken(NowFunctionToken token)
        {
            this.State.Write(token.Utc ? MySqlSymbols.UTC_TIMESTAMP : Symbols.NOW);
            State.AddToTheEnd(Symbols.OpenParenthesis);
            State.AddToTheEnd(Symbols.CloseParenthesis);
        }

        protected override void VisitUuidFunctionToken(UuidFunctionToken token)
        {
            State.Write(MySqlSymbols.UUID, Symbols.OpenParenthesis, Symbols.CloseParenthesis);
        }

        protected override void VisitIIFFunctionToken(IifFunctionToken token)
        {
            State.Write(Symbols.IF, Symbols.OpenParenthesis);
            VisitToken(token.ConditionToken);
            State.Write(Symbols.Comma);
            VisitToken(token.ThenToken);
            State.Write(Symbols.Comma);
            VisitToken(token.ElseToken);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDatePartFunctionToken(DatePartFunctionToken token)
        {
            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(MySqlSymbols.DAY);
                    break;
                case DatePart.Year:
                    State.Write(MySqlSymbols.YEAR);
                    break;
                case DatePart.Month:
                    State.Write(MySqlSymbols.MONTH);
                    break;
                case DatePart.Week:
                    State.Write(MySqlSymbols.WEEK);
                    break;
                case DatePart.Hour:
                    State.Write(MySqlSymbols.HOUR);
                    break;
                case DatePart.Minute:
                    State.Write(MySqlSymbols.MINUTE);
                    break;
                case DatePart.Second:
                    State.Write(MySqlSymbols.SECOND);
                    break;
                case DatePart.Millisecond:
                    State.Write(MySqlSymbols.MICROSECOND);
                    break;
            }
            State.AddToTheEnd(Symbols.OpenParenthesis);
            VisitToken(token.Token);
            State.Write(Symbols.CloseParenthesis);

            if (token.DatePart == DatePart.Millisecond)
            {
                State.Write(Symbols.DIV);
                State.Write(MySqlSymbols.milisecondInSecond);
            }
        }

        protected override void VisitDateAddFunctionToken(DateAddFunctionToken token)
        {
            State.Write(Symbols.OpenParenthesis);
            VisitToken(token.Token);
            this.State.Write(token.Subtract ? Symbols.MinusVal : Symbols.PlusVal);
            State.Write(Symbols.INTERVAL);

            VisitToken(token.Number);

            if (token.DatePart == DatePart.Millisecond)
            {
                State.Write(Symbols.DIV);
                State.Write(MySqlSymbols.milisecondInSecond);
            }

            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(MySqlSymbols.DAY);
                    break;
                case DatePart.Year:
                    State.Write(MySqlSymbols.YEAR);
                    break;
                case DatePart.Month:
                    State.Write(MySqlSymbols.MONTH);
                    break;
                case DatePart.Week:
                    State.Write(MySqlSymbols.WEEK);
                    break;
                case DatePart.Hour:
                    State.Write(MySqlSymbols.HOUR);
                    break;
                case DatePart.Minute:
                    State.Write(MySqlSymbols.MINUTE);
                    break;
                case DatePart.Second:
                    State.Write(MySqlSymbols.SECOND);
                    break;
                case DatePart.Millisecond:
                    State.Write(MySqlSymbols.MICROSECOND);
                    break;
            }                     
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDurationFunctionToken(DurationFunctionToken token)
        {
            State.Write(MySqlSymbols.TIMESTAMPDIFF, Symbols.OpenParenthesis);

            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(MySqlSymbols.DAY);
                    break;
                case DatePart.Year:
                    State.Write(MySqlSymbols.YEAR);
                    break;
                case DatePart.Month:
                    State.Write(MySqlSymbols.MONTH);
                    break;
                case DatePart.Week:
                    State.Write(MySqlSymbols.WEEK);
                    break;
                case DatePart.Hour:
                    State.Write(MySqlSymbols.HOUR);
                    break;
                case DatePart.Minute:
                    State.Write(MySqlSymbols.MINUTE);
                    break;
                case DatePart.Second:
                    State.Write(MySqlSymbols.SECOND);
                    break;
                case DatePart.Millisecond:
                    State.Write(MySqlSymbols.MICROSECOND);
                    break;
            }
            State.Write(Symbols.Comma);
            VisitToken(token.Start);
            State.Write(Symbols.Comma);
            VisitToken(token.End);
            State.Write(Symbols.CloseParenthesis);

            if (token.DatePart == DatePart.Millisecond)
            {
                State.Write(Symbols.DIV);
                State.Write(MySqlSymbols.milisecondInSecond);
            }
        }

        protected override void VisitMakeDateFunctionToken(MakeDateFunctionToken token)
        {
            State.Write(MySqlSymbols.TIMESTAMP, Symbols.OpenParenthesis);

            DateTime date = new DateTime((int)((Scalar)token.Year).Value, (int)((Scalar)token.Month).Value, (int)((Scalar)token.Day).Value);

            State.Write(MySqlSymbols.MAKEDATE);
            State.AddToTheEnd(Symbols.OpenParenthesis);
            State.Write(date.Year.ToString());
            State.Write(Symbols.Comma);
            State.Write(date.DayOfYear.ToString());

            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.Comma);

            State.Write(MySqlSymbols.MAKETIME);
            State.AddToTheEnd(Symbols.OpenParenthesis);
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
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitMakeTimeFunctionToken(MakeTimeFunctionToken token)
        {
            State.Write(MySqlSymbols.MAKETIME, Symbols.OpenParenthesis);

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
            State.Write(Symbols.CloseParenthesis);
        }

        

        protected override void VisitContainsToken(BinaryToken token)
        {
            VisitToken(token.First);
            State.Write(Symbols.LIKE);
            State.Write(Symbols.CONCAT);
            State.Write(Symbols.OpenParenthesis);
            State.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
            State.Write(Symbols.Comma);
            VisitToken(token.Second);
            State.Write(Symbols.Comma);
            State.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
            State.Write(Symbols.CloseParenthesis);

        }

        protected override void VisitStartsWithToken(BinaryToken token)
        {
            VisitToken(token.First);
            State.Write(Symbols.LIKE);
            State.Write(Symbols.CONCAT);
            State.Write(Symbols.OpenParenthesis);
            VisitToken(token.Second);
            State.Write(Symbols.Comma);
            State.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitEndsWithToken(BinaryToken token)
        {
            VisitToken(token.First);
            State.Write(Symbols.LIKE);
            State.Write(Symbols.CONCAT);
            State.Write(Symbols.OpenParenthesis);
            State.Write(this.LiteralOpenQuote, Symbols.ModuloVal, this.LiteralCloseQuote);
            State.Write(Symbols.Comma);
            VisitToken(token.Second);
            State.Write(Symbols.CloseParenthesis);
        }
    }
}
