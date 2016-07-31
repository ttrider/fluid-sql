// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System;

namespace TTRider.FluidSql.Providers.Sqlite
{
    internal partial class SqliteVisitor
    {
        protected override void VisitNowFunctionToken(NowFunctionToken token)
        {
            if (token.Utc)
            {
                State.Write(Symbols.DATETIME);
                State.Write(Symbols.OpenParenthesis);
                State.Write(SqliteSymbols.current_timestamp);
                State.Write(Symbols.CloseParenthesis);
            }
            else
            {
                State.Write(Symbols.DATETIME);
                State.Write(Symbols.OpenParenthesis);
                State.Write(SqliteSymbols.current_timestamp);
                State.Write(Symbols.Comma);
                State.Write(Symbols.SingleQuote);
                State.Write(SqliteSymbols.localtime);
                State.Write(Symbols.SingleQuote);
                State.Write(Symbols.CloseParenthesis);
            }
        }

        protected override void VisitUuidFunctionToken(UuidFunctionToken token)
        {
            State.Write(SqliteSymbols.hex);
            State.Write(Symbols.OpenParenthesis);
            State.Write(SqliteSymbols.randomblob);
            State.Write(Symbols.OpenParenthesis);
            State.Write("16");
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.CloseParenthesis);
        }


        protected override void VisitIIFFunctionToken(IifFunctionToken token)
        {
            // for sqlite, we need to use CASE function instead
            var cf = Sql.Case.When(token.ConditionToken, token.ElseToken).Else(token.ElseToken);
            VisitCaseToken(cf);
        }

        protected override void VisitDatePartFunctionToken(DatePartFunctionToken token)
        {
            State.Write(Symbols.CAST);
            State.Write(Symbols.OpenParenthesis);
            State.Write(SqliteSymbols.strftime);
            State.Write(Symbols.OpenParenthesis);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.ModuloVal);

            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(SqliteSymbols.d);
                    break;
                case DatePart.Year:
                    State.Write(SqliteSymbols.Y);
                    break;
                case DatePart.Month:
                    State.Write(SqliteSymbols.m);
                    break;
                case DatePart.Week:
                    State.Write(SqliteSymbols.W);
                    break;
                case DatePart.Hour:
                    State.Write(SqliteSymbols.H);
                    break;
                case DatePart.Minute:
                    State.Write(SqliteSymbols.M);
                    break;
                case DatePart.Second:
                    State.Write(SqliteSymbols.S);
                    break;
                case DatePart.Millisecond:
                    State.Write(SqliteSymbols.f);
                    State.Write(Symbols.SingleQuote);
                    State.Write(Symbols.Comma);
                    VisitToken(token.Token);
                    State.Write(Symbols.CloseParenthesis);
                    State.Write(Symbols.MinusVal);

                    State.Write(SqliteSymbols.floor);
                    State.Write(Symbols.OpenParenthesis);
                    State.Write(SqliteSymbols.strftime);
                    State.Write(Symbols.OpenParenthesis);
                    State.Write(Symbols.SingleQuote);
                    State.Write(Symbols.ModuloVal);
                    State.Write(Symbols.SingleQuote);
                    State.Write(Symbols.Comma);
                    VisitToken(token.Token);
                    State.Write(Symbols.CloseParenthesis);
                    State.Write(Symbols.CloseParenthesis);
                    State.Write(Symbols.AS);
                    State.Write(SqliteSymbols.INTEGER);
                    State.Write(Symbols.CloseParenthesis);
                    return;
            }
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.Comma);
            VisitToken(token.Token);
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.AS);
            State.Write(SqliteSymbols.INTEGER);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDurationFunctionToken(DurationFunctionToken token)
        {
            State.Write(Symbols.CAST);
            State.Write(Symbols.OpenParenthesis);
            State.Write(Symbols.CAST);
            State.Write(Symbols.OpenParenthesis);
            State.Write(SqliteSymbols.strftime);
            State.Write(Symbols.OpenParenthesis);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.ModuloVal);
            State.Write(SqliteSymbols.S);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.Comma);
            VisitToken(token.End);
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.MinusVal);
            State.Write(SqliteSymbols.strftime);
            State.Write(Symbols.OpenParenthesis);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.ModuloVal);
            State.Write(SqliteSymbols.S);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.Comma);
            VisitToken(token.Start);
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.AS);
            State.Write(SqliteSymbols.REAL);
            State.Write(Symbols.CloseParenthesis);

            switch (token.DatePart)
            {
                case DatePart.Day:
                    State.Write(Symbols.DivideVal);
                    State.Write("86400");
                    break;
                case DatePart.Year:
                    State.Write(Symbols.DivideVal);
                    State.Write("31536000.0");
                    break;
                case DatePart.Month:
                    State.Write(Symbols.DivideVal);
                    State.Write("2592000.0");
                    break;
                case DatePart.Week:
                    State.Write(Symbols.DivideVal);
                    State.Write("604800.0");
                    break;
                case DatePart.Hour:
                    State.Write(Symbols.DivideVal);
                    State.Write("3600.0");
                    break;
                case DatePart.Minute:
                    State.Write(Symbols.DivideVal);
                    State.Write("60.0");
                    break;
                case DatePart.Second:
                    break;
                case DatePart.Millisecond:
                    State.Write(Symbols.MultiplyVal);
                    State.Write("100.0");
                    break;
            }
            State.Write(Symbols.AS);
            State.Write(SqliteSymbols.INTEGER);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitMakeDateFunctionToken(MakeDateFunctionToken token)
        {
            //datetime('YYYY-MM-DDTHH:MM:SS.SSS');
            State.Write(SqliteSymbols.datetime);
            State.Write(Symbols.OpenParenthesis);


            //year
            WriteDateTimePart("04d", token.Year, "2000");
            // - 
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.MinusVal);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.Concat);
            // month
            WriteDateTimePart("02d", token.Month, "01");
            // - 
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.MinusVal);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.Concat);
            // day
            WriteDateTimePart("02d", token.Day, "01");
            // time part
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write("T");
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.Concat);

            // hour
            WriteDateTimePart("02d", token.Hour, "00");
            // :
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.Colon);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.Concat);

            // minute
            WriteDateTimePart("02d", token.Minute, "00");
            // :
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.Colon);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.Concat);

            // second
            WriteDateTimePart("02d", token.Second, "00");
            // .
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.DotVal);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.Concat);
            // ms
            WriteDateTimePart("03d", token.Millisecond, "000");

            State.Write(Symbols.CloseParenthesis);
        }

        void WriteDateTimePart(string format, Token token, string defaultValue)
        {
            State.Write(Symbols.CAST);
            State.Write(Symbols.OpenParenthesis);

            if (token == null)
            {
                State.Write(Symbols.SingleQuote);
                State.Write(defaultValue);
                State.Write(Symbols.SingleQuote);
            }
            else
            {
                State.Write(SqliteSymbols.printf);
                State.Write(Symbols.SingleQuote);
                State.Write(Symbols.ModuloVal);
                State.Write(format);
                State.Write(Symbols.SingleQuote);
                State.Write(Symbols.Comma);
                State.Write(Symbols.CAST);
                State.Write(Symbols.OpenParenthesis);
                State.Write(SqliteSymbols.IFNULL);
                State.Write(Symbols.OpenParenthesis);
                VisitToken(token);
                State.Write(Symbols.Comma);
                State.Write(Symbols.SingleQuote);
                State.Write(defaultValue);
                State.Write(Symbols.SingleQuote);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.AS);
                State.Write(SqliteSymbols.INTEGER);
                State.Write(Symbols.CloseParenthesis);
            }
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.AS);
            State.Write(SqliteSymbols.TEXT);
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitMakeTimeFunctionToken(MakeTimeFunctionToken token)
        {
            State.Write(SqliteSymbols.datetime);
            State.Write(Symbols.OpenParenthesis);
            // hour
            WriteDateTimePart("02d", token.Hour, "00");
            // :
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.Colon);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.Concat);

            // minute
            WriteDateTimePart("02d", token.Minute, "00");
            // :
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.Colon);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.Concat);

            // second
            WriteDateTimePart("02d", token.Second, "00");
            // .
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.DotVal);
            State.Write(Symbols.SingleQuote);
            State.Write(SqliteSymbols.Concat);
            // ms
            WriteDateTimePart("03d", null, "000");

            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitDateAddFunctionToken(DateAddFunctionToken token)
        {
            //,datetime(datetime('now'),CAST( CAST(-5 AS INTEGER) * 7 as TEXT)||' day')
            State.Write(SqliteSymbols.datetime);
            State.Write(Symbols.OpenParenthesis);

            State.Write(Symbols.CAST);
            State.Write(Symbols.OpenParenthesis);
            State.Write(Symbols.CAST);
            State.Write(Symbols.OpenParenthesis);
            VisitToken(token.Subtract ? new UnaryMinusToken { Token = token.Number } : token.Number);
            State.Write(Symbols.AS);
            State.Write(SqliteSymbols.INTEGER);
            State.Write(Symbols.CloseParenthesis);

            switch (token.DatePart)
            {
                case DatePart.Week:
                    State.Write(" * 7");
                    break;
                case DatePart.Millisecond:
                    State.Write(" / 100.0");
                    break;
            }
            State.Write(Symbols.AS);
            State.Write(SqliteSymbols.TEXT);
            State.Write(Symbols.CloseParenthesis);
            State.Write(SqliteSymbols.Concat);
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.Space);

            switch (token.DatePart)
            {
                case DatePart.Day:
                case DatePart.Week:
                    State.Write(SqliteSymbols.days);
                    break;
                case DatePart.Year:
                    State.Write(SqliteSymbols.years);
                    break;
                case DatePart.Month:
                    State.Write(SqliteSymbols.months);
                    break;
                case DatePart.Hour:
                    State.Write(SqliteSymbols.hours);
                    break;
                case DatePart.Minute:
                    State.Write(SqliteSymbols.minutes);
                    break;
                case DatePart.Second:
                case DatePart.Millisecond:
                    State.Write(SqliteSymbols.seconds);
                    break;
            }
            State.Write(Symbols.SingleQuote);
            State.Write(Symbols.CloseParenthesis);
        }
    }
}