// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace TTRider.FluidSql
{
    public partial class Sql
    {
        public static FunctionExpressionToken Coalesce(params Token[] arguments)
        {
            var f = new CoalesceFunctionToken();
            f.Arguments.AddRange(arguments);
            return f;
        }

        public static FunctionExpressionToken Coalesce(IEnumerable<Token> arguments)
        {
            var f = new CoalesceFunctionToken();
            f.Arguments.AddRange(arguments);
            return f;
        }

        public static FunctionExpressionToken IsNull(Token first, Token second)
        {
            var f = new CoalesceFunctionToken();
            f.Arguments.Add(first);
            f.Arguments.Add(second);
            return f;
        }

        public static FunctionExpressionToken IfNull(Token first, Token second)
        {
            var f = new CoalesceFunctionToken();
            f.Arguments.Add(first);
            f.Arguments.Add(second);
            return f;
        }

        public static FunctionExpressionToken Nvl(Token first, Token second)
        {
            var f = new CoalesceFunctionToken();
            f.Arguments.Add(first);
            f.Arguments.Add(second);
            return f;
        }

        public static Function NullIf(Token argument1, Token argument2)
        {
            var f = new Function
            {
                Name = "NULLIF"
            };
            f.Arguments.Add(argument1);
            f.Arguments.Add(argument2);
            return f;
        }

        public static Function Function(string name, params Token[] arguments)
        {
            var f = new Function
            {
                Name = name
            };
            f.Arguments.AddRange(arguments);
            return f;
        }

        public static Function Function(string name, IEnumerable<Token> arguments)
        {
            var f = new Function
            {
                Name = name
            };
            if (arguments != null)
            {
                f.Arguments.AddRange(arguments);
            }
            return f;
        }


        public static NowFunctionToken Now(bool utc = false)
        {
            return new NowFunctionToken { Utc = utc };
        }

        public static NowFunctionToken GetDate(bool utc = false)
        {
            return new NowFunctionToken { Utc = utc };
        }

        public static UuidFunctionToken UUID()
        {
            return new UuidFunctionToken();
        }

        public static UuidFunctionToken NewId()
        {
            return new UuidFunctionToken();
        }

        public static IifFunctionToken IIF(ExpressionToken conditionToken, ExpressionToken thenToken,
            ExpressionToken elseToken)
        {
            return new IifFunctionToken
            {
                ConditionToken = conditionToken,
                ThenToken = thenToken,
                ElseToken = elseToken
            };
        }

        public static DatePartFunctionToken DatePart(DatePart part, ExpressionToken token)
        {
            return new DatePartFunctionToken
            {
                Token = token,
                DatePart = part
            };
        }

        public static DatePartFunctionToken Second(ExpressionToken token)
        {
            return new DatePartFunctionToken
            {
                Token = token,
                DatePart = FluidSql.DatePart.Second
            };
        }

        public static DatePartFunctionToken Minute(ExpressionToken token)
        {
            return new DatePartFunctionToken
            {
                Token = token,
                DatePart = FluidSql.DatePart.Minute
            };
        }

        public static DatePartFunctionToken Hour(ExpressionToken token)
        {
            return new DatePartFunctionToken
            {
                Token = token,
                DatePart = FluidSql.DatePart.Hour
            };
        }

        public static DatePartFunctionToken Week(ExpressionToken token)
        {
            return new DatePartFunctionToken
            {
                Token = token,
                DatePart = FluidSql.DatePart.Week
            };
        }

        public static DatePartFunctionToken Day(ExpressionToken token)
        {
            return new DatePartFunctionToken
            {
                Token = token,
                DatePart = FluidSql.DatePart.Day
            };
        }

        public static DatePartFunctionToken Month(ExpressionToken token)
        {
            return new DatePartFunctionToken
            {
                Token = token,
                DatePart = FluidSql.DatePart.Month
            };
        }

        public static DatePartFunctionToken Year(ExpressionToken token)
        {
            return new DatePartFunctionToken
            {
                Token = token,
                DatePart = FluidSql.DatePart.Year
            };
        }

        public static DatePartFunctionToken Millisecond(ExpressionToken token)
        {
            return new DatePartFunctionToken
            {
                Token = token,
                DatePart = FluidSql.DatePart.Millisecond
            };
        }

        public static DurationFunctionToken Duration(DatePart datePart, ExpressionToken start, ExpressionToken end)
        {
            return new DurationFunctionToken
            {
                DatePart = datePart,
                Start = start,
                End = end
            };
        }


        public static DurationFunctionToken DurationInYears(ExpressionToken start, ExpressionToken end)
        {
            return new DurationFunctionToken
            {
                DatePart = FluidSql.DatePart.Year,
                Start = start,
                End = end
            };
        }

        public static DurationFunctionToken DurationInMonth(ExpressionToken start, ExpressionToken end)
        {
            return new DurationFunctionToken { DatePart = FluidSql.DatePart.Month, Start = start, End = end };
        }

        public static DurationFunctionToken DurationInDays(ExpressionToken start, ExpressionToken end)
        {
            return new DurationFunctionToken { DatePart = FluidSql.DatePart.Day, Start = start, End = end };
        }

        public static DurationFunctionToken DurationInWeeks(ExpressionToken start, ExpressionToken end)
        {
            return new DurationFunctionToken { DatePart = FluidSql.DatePart.Week, Start = start, End = end };
        }

        public static DurationFunctionToken DurationInHours(ExpressionToken start, ExpressionToken end)
        {
            return new DurationFunctionToken { DatePart = FluidSql.DatePart.Hour, Start = start, End = end };
        }

        public static DurationFunctionToken DurationInMinutes(ExpressionToken start, ExpressionToken end)
        {
            return new DurationFunctionToken { DatePart = FluidSql.DatePart.Minute, Start = start, End = end };
        }

        public static DurationFunctionToken DurationInSeconds(ExpressionToken start, ExpressionToken end)
        {
            return new DurationFunctionToken { DatePart = FluidSql.DatePart.Second, Start = start, End = end };
        }

        public static DurationFunctionToken DurationInMilliseconds(ExpressionToken start, ExpressionToken end)
        {
            return new DurationFunctionToken { DatePart = FluidSql.DatePart.Millisecond, Start = start, End = end };
        }

        public static MakeDateFunctionToken MakeDate(int year, int month, int day, int hour = 0, int minute = 0,
            int second = 0)
        {
            return new MakeDateFunctionToken
            {
                Year = Scalar(year),
                Month = Scalar(month),
                Day = Scalar(day),
                Hour = Scalar(hour),
                Minute = Scalar(minute),
                Second = Scalar(second)
            };
        }

        public static MakeDateFunctionToken MakeDate(ExpressionToken year, ExpressionToken month, ExpressionToken day,
            ExpressionToken hour = null, ExpressionToken minute = null, ExpressionToken second = null)
        {
            return new MakeDateFunctionToken
            {
                Year = year,
                Month = month,
                Day = day,
                Hour = hour,
                Minute = minute,
                Second = second
            };
        }


        public static MakeTimeFunctionToken MakeTime(int hour, int minute, int second = 0)
        {
            return new MakeTimeFunctionToken
            {
                Hour = Scalar(hour),
                Minute = Scalar(minute),
                Second = Scalar(second)
            };
        }

        public static MakeTimeFunctionToken MakeTime(ExpressionToken hour, ExpressionToken minute,
            ExpressionToken second = null)
        {
            return new MakeTimeFunctionToken
            {
                Hour = hour,
                Minute = minute,
                Second = second
            };
        }
    }
}