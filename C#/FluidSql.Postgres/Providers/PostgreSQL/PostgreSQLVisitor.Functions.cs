using System.Text;

namespace TTRider.FluidSql.Providers.PostgreSQL
{
    internal partial class PostgreSQLVisitor
    {
        protected override void VisitNowFunctionToken(NowFunctionToken token)
        {
            State.Write(Symbols.NOW);
            State.Write(Symbols.OpenParenthesis);
            State.Write(Symbols.CloseParenthesis);

            if (token.Utc)
            {
                State.Write(Symbols.AT);
                State.Write(Symbols.TIME);
                State.Write(Symbols.ZONE);
                State.Write(Symbols.CurrentSetting);
                State.Write(Symbols.OpenParenthesis);
                VisitToken(Sql.Scalar(Symbols.UTC));
                State.Write(Symbols.CloseParenthesis);
            }
        }

        protected override void VisitUuidFunctionToken(UuidFunctionToken token)
        {
            State.Write(uuidGeneration);
        }

        protected override void VisitIIFFunctionToken(IifFunctionToken token)
        {
            VisitCaseToken(Sql.Case.When(token.ConditionToken, token.ThenToken).Else(token.ElseToken));
        }

        protected override void VisitDatePartFunctionToken(DatePartFunctionToken token)
        {
            if (token.DatePart != DatePart.Millisecond)
            {
                State.Write(PostgrSQLSymbols.DATEPART);
                State.Write(Symbols.OpenParenthesis);

                switch (token.DatePart)
                {
                    case DatePart.Day: State.Write(AddSingleQuotes(PostgrSQLSymbols.d)); break;
                    case DatePart.Year: State.Write(AddSingleQuotes(PostgrSQLSymbols.yy)); break;
                    case DatePart.Month: State.Write(AddSingleQuotes(PostgrSQLSymbols.m)); break;
                    case DatePart.Week: State.Write(AddSingleQuotes(PostgrSQLSymbols.ww)); break;
                    case DatePart.Hour: State.Write(AddSingleQuotes(PostgrSQLSymbols.hh)); break;
                    case DatePart.Minute: State.Write(AddSingleQuotes(PostgrSQLSymbols.mi)); break;
                    case DatePart.Second: State.Write(AddSingleQuotes(PostgrSQLSymbols.ss)); break;
                }
                State.Write(Symbols.Comma);
                VisitToken(token.Token);
                State.Write(Symbols.CloseParenthesis);
            }
            else
            {
                State.Write(Symbols.OpenParenthesis);
                State.Write(PostgrSQLSymbols.TRUNC);
                State.Write(Symbols.OpenParenthesis);
                State.Write(PostgrSQLSymbols.DATEPART);
                State.Write(Symbols.OpenParenthesis);
                State.Write(AddSingleQuotes(PostgrSQLSymbols.ms)); 
                State.Write(Symbols.Comma);
                VisitToken(token.Token);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.DivideVal);
                State.Write(PostgrSQLSymbols.milisecondInSecond);
                State.Write(Symbols.CloseParenthesis);
                State.Write(Symbols.CloseParenthesis);
            }
        }

        protected override void VisitDateAddFunctionToken(DateAddFunctionToken token)
        {
            string temp = string.Empty;

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
            State.Write(Symbols.SingleQuote);
            VisitToken((Scalar)token.Number);
            //VisitToken(Scalar)token.Number).Value);

            switch (token.DatePart)
            {
                case DatePart.Day: State.Write(PostgrSQLSymbols.d); break;
                case DatePart.Year: State.Write(PostgrSQLSymbols.yy); break;
                case DatePart.Month: State.Write(PostgrSQLSymbols.m); break;
                case DatePart.Week: State.Write(PostgrSQLSymbols.ww); break;
                case DatePart.Hour: State.Write(PostgrSQLSymbols.hh); break;
                case DatePart.Minute: State.Write(PostgrSQLSymbols.mi); break;
                case DatePart.Second: State.Write(PostgrSQLSymbols.ss); break;
                case DatePart.Millisecond: State.Write(PostgrSQLSymbols.ms); break;
            }
            State.Write(Symbols.SingleQuote);
        }

        protected override void VisitDurationFunctionToken(DurationFunctionToken token)
        {
            switch (token.DatePart)
            {
                case DatePart.Day:
                    //DATE_PART('day', end - start) 
                    State.Write(Symbols.OpenParenthesis);
                    VisitDurationTokenWithDiff(DatePart.Day, token);
                    State.Write(Symbols.CloseParenthesis);
                    break;
                case DatePart.Year:
                    //DATE_PART('year', end) - DATE_PART('year', start)
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(Sql.Year(token.Start));
                    State.Write(Symbols.MinusVal);
                    VisitToken(Sql.Year(token.End));
                    State.Write(Symbols.CloseParenthesis);
                    break;

                case DatePart.Month:
                    //years_diff * 12 + (DATE_PART('month', end) - DATE_PART('month', start)) 
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(Sql.DurationInYears(token.Start, token.End));
                    State.Write(Symbols.MultiplyVal);
                    State.Write(PostgrSQLSymbols.monthsInYear); //month in year
                    State.Write(Symbols.PlusVal);
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(Sql.Month(token.Start));
                    State.Write(Symbols.MinusVal);
                    VisitToken(Sql.Month(token.End));
                    State.Write(Symbols.CloseParenthesis);
                    State.Write(Symbols.CloseParenthesis);
                    break;

                case DatePart.Week:
                    //TRUNC(DATE_PART('day', end - start) / 7)
                    State.Write(Symbols.OpenParenthesis);
                    State.Write(Symbols.TRUNC);
                    State.Write(Symbols.OpenParenthesis);
                    VisitDurationTokenWithDiff(DatePart.Day, token);
                    State.Write(Symbols.DivideVal);
                    State.Write(PostgrSQLSymbols.daysInWeek);
                    State.Write(Symbols.CloseParenthesis);
                    State.Write(Symbols.CloseParenthesis);
                    break;

                case DatePart.Hour:
                    //days_diff * 24 + DATE_PART('hour', end - start ) 
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(Sql.DurationInDays(token.Start, token.End));
                    State.Write(Symbols.MultiplyVal);
                    State.Write(PostgrSQLSymbols.hoursInDay);
                    State.Write(Symbols.PlusVal);
                    VisitDurationTokenWithDiff(DatePart.Hour, token);
                    State.Write(Symbols.CloseParenthesis);
                    break;

                case DatePart.Minute:
                    //hours_diff * 60 + DATE_PART('minute', end - start ) 
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(Sql.DurationInHours(token.Start, token.End));
                    State.Write(Symbols.MultiplyVal);
                    State.Write(PostgrSQLSymbols.minutesInHour);
                    State.Write(Symbols.PlusVal);
                    VisitDurationTokenWithDiff(DatePart.Minute, token);
                    State.Write(Symbols.CloseParenthesis);
                    break;

                case DatePart.Second:
                    //minutes_diff * 60 + DATE_PART('second', end - start )
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(Sql.DurationInMinutes(token.Start, token.End));
                    State.Write(Symbols.MultiplyVal);
                    State.Write(PostgrSQLSymbols.secondInMinute);
                    State.Write(Symbols.PlusVal);
                    VisitDurationTokenWithDiff(DatePart.Second, token);
                    State.Write(Symbols.CloseParenthesis);

                    break;
                    case DatePart.Millisecond:
                    //seconds_diff * 1000 + DATE_PART('microsecond', end - start )/1000)
                    State.Write(Symbols.OpenParenthesis);
                    VisitToken(Sql.DurationInSeconds(token.Start, token.End));
                    State.Write(Symbols.MultiplyVal);
                    State.Write(PostgrSQLSymbols.milisecondInSecond);
                    State.Write(Symbols.PlusVal);
                    State.Write(Symbols.OpenParenthesis);
                    State.Write(Symbols.TRUNC);
                    State.Write(Symbols.OpenParenthesis);
                    VisitDurationTokenWithDiff(DatePart.Millisecond, token);
                    State.Write(Symbols.DivideVal);
                    State.Write(PostgrSQLSymbols.milisecondInSecond);
                    State.Write(Symbols.CloseParenthesis);
                    State.Write(Symbols.CloseParenthesis);
                    State.Write(Symbols.CloseParenthesis);
                    break;
            }
        }

        protected override void VisitMakeDateFunctionToken(MakeDateFunctionToken token)
        {
            State.Write(PostgrSQLSymbols.TIMESTAMP);

            StringBuilder sb = new StringBuilder(Symbols.SingleQuote);
            sb.Append(((Scalar)token.Year).Value);
            sb.Append(Symbols.MinusVal);
            sb.Append(((Scalar)token.Day).Value);
            sb.Append(Symbols.MinusVal);
            sb.Append(((Scalar)token.Month).Value);
            sb.Append(Symbols.Space);

            if (token.Hour != null)
            {
                sb.Append(((Scalar)token.Hour).Value);
            }
            else
            {
                sb.Append(0);
            }
            sb.Append(Symbols.Colon);
            

            if (token.Minute != null)
            {
                sb.Append(((Scalar)token.Minute).Value);
            }
            else
            {
                sb.Append(0);
            }
            sb.Append(Symbols.Colon);

            if (token.Second != null)
            {
                sb.Append(((Scalar)token.Second).Value);
            }
            else
            {
                sb.Append(0);
            }
            sb.Append(Symbols.SingleQuote);

            State.Write(sb.ToString());
        }

        protected override void VisitMakeTimeFunctionToken(MakeTimeFunctionToken token)
        {
            State.Write(Symbols.TIME);

            StringBuilder sb = new StringBuilder(Symbols.SingleQuote);
            sb.Append(((Scalar)token.Hour).Value);
            sb.Append(Symbols.Colon);
            sb.Append(((Scalar)token.Minute).Value);
            sb.Append(Symbols.Colon);
            
            if (token.Second != null)
            {
                sb.Append(((Scalar)token.Second).Value);
            }
            else
            {
                sb.Append(0);
            }
            sb.Append(Symbols.SingleQuote);

            State.Write(sb.ToString());
        }

        private void VisitDurationTokenWithDiff(DatePart datePart, DurationFunctionToken token)
        {
            State.Write(Symbols.OpenParenthesis);
            State.Write(PostgrSQLSymbols.DATEPART);
            State.Write(Symbols.OpenParenthesis);
            switch (datePart)
            {
                case DatePart.Day:
                    State.Write(AddSingleQuotes(PostgrSQLSymbols.d));
                    break;
                case DatePart.Year:
                    State.Write(AddSingleQuotes(PostgrSQLSymbols.yy));
                    break;
                case DatePart.Month:
                    State.Write(AddSingleQuotes(PostgrSQLSymbols.m));
                    break;
                case DatePart.Week:
                    State.Write(AddSingleQuotes(PostgrSQLSymbols.ww));
                    break;
                case DatePart.Hour:
                    State.Write(AddSingleQuotes(PostgrSQLSymbols.hh));
                    break;
                case DatePart.Minute:
                    State.Write(AddSingleQuotes(PostgrSQLSymbols.mi));
                    break;
                case DatePart.Second:
                    State.Write(AddSingleQuotes(PostgrSQLSymbols.ss));
                    break;
                case DatePart.Millisecond:
                    State.Write(AddSingleQuotes(PostgrSQLSymbols.ms));
                    break;
            }
            State.Write(Symbols.Comma);
            VisitToken(token.Start);
            State.Write(Symbols.MinusVal);
            VisitToken(token.End);
            State.Write(Symbols.CloseParenthesis);
            State.Write(Symbols.CloseParenthesis);
        }

        private string AddSingleQuotes(string str)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Symbols.SingleQuote);
            sb.Append(str);
            sb.Append(Symbols.SingleQuote);
            return sb.ToString();
        }
        private string uuidGeneration = "uuid_in(md5(random()::text || now()::text)::cstring)";

    }
}
