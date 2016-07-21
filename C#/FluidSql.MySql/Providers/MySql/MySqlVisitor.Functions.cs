using System;
using System.Text;

namespace TTRider.FluidSql.Providers.MySql
{
    internal partial class MySqlVisitor
    {
        protected override void VisitNowFunctionToken(NowFunctionToken token)
        {
            if (token.Utc)
            {
                State.Write(MySqlSymbols.UTC_TIMESTAMP);
            }
            else
            {
                State.Write(Symbols.NOW);
            }
            State.AddToTheEnd(Symbols.OpenParenthesis);
            State.AddToTheEnd(Symbols.CloseParenthesis);
        }

        protected override void VisitUuidFunctionToken(UuidFunctionToken token)
        {
            State.Write(MySqlSymbols.UUID);
            State.AddToTheEnd(Symbols.OpenParenthesis);
            State.AddToTheEnd(Symbols.CloseParenthesis);
        }

        protected override void VisitIIFFunctionToken(IifFunctionToken token)
        {
            State.Write(Symbols.IF);
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
            if (token.Subtract)
            {
                State.Write(Symbols.MinusVal);
            }
            else
            {
                State.Write(Symbols.PlusVal);
            }
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
            State.Write(MySqlSymbols.TIMESTAMPDIFF);
            State.AddToTheEnd(Symbols.OpenParenthesis);

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
            State.Write(MySqlSymbols.TIMESTAMP);
            State.AddToTheEnd(MySqlSymbols.OpenParenthesis);

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
            State.Write(MySqlSymbols.MAKETIME);
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
            State.Write(Symbols.CloseParenthesis);
        }

        protected override void VisitCoalesceFunctionToken(CoalesceFunctionToken token) { throw new NotImplementedException(); }
        protected override void VisitNullIfFunctionToken(NullIfFunctionToken token) { throw new NotImplementedException(); }

    }
}
