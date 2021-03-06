﻿// <license>
//     The MIT License (MIT)
// </license>
// <copyright company="TTRider, L.L.C.">
//     Copyright (c) 2014-2016 All Rights Reserved
// </copyright>

using TTRider.FluidSql;
using Xunit;

namespace xUnit.Postgres
{
    public class FunctionsTest : PostgreSqlProviderTests
    {
        [Fact]
        public void GetNow()
        {
            var statement = Sql.Select.Output(Sql.Now(), Sql.GetDate());

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT NOW ( ), NOW ( );", command.CommandText);
        }

        [Fact]
        public void GetNowUTC()
        {
            var statement = Sql.Select.Output(Sql.Now(true), Sql.GetDate(true));

            var command = Provider.GetCommand(statement);

            Assert.NotNull(command);
            Assert.Equal("SELECT NOW ( ) AT TIME ZONE ( 'UTC' ), NOW ( ) AT TIME ZONE ( 'UTC' );", command.CommandText);
        }

        [Fact]
        public void GetUuid()
        {
            AssertSql(
                Sql.Select.Output(Sql.UUID(), Sql.NewId()),
                "SELECT uuid_in(md5(random()::text || now()::text)::cstring), uuid_in(md5(random()::text || now()::text)::cstring);"
                );
        }

        [Fact] 
        public void DatePart()
        {
            AssertSql( Sql.Select.Output(Sql.Year(Sql.Now())), "SELECT DATE_PART ( 'year', NOW ( ) );");
            AssertSql(Sql.Select.Output(Sql.Month(Sql.Now())), "SELECT DATE_PART ( 'month', NOW ( ) );");
            AssertSql(Sql.Select.Output(Sql.Day(Sql.Now())), "SELECT DATE_PART ( 'day', NOW ( ) );");
            AssertSql(Sql.Select.Output(Sql.Week(Sql.Now())), "SELECT DATE_PART ( 'week', NOW ( ) );");
            AssertSql(Sql.Select.Output(Sql.Hour(Sql.Now())), "SELECT DATE_PART ( 'hour', NOW ( ) );");
            AssertSql(Sql.Select.Output(Sql.Minute(Sql.Now())), "SELECT DATE_PART ( 'minute', NOW ( ) );");
            AssertSql(Sql.Select.Output(Sql.Second(Sql.Now())), "SELECT DATE_PART ( 'second', NOW ( ) );");
            AssertSql(Sql.Select.Output(Sql.Millisecond(Sql.Now())), "SELECT ( TRUNC ( DATE_PART ( 'microsecond', NOW ( ) ) / 1000 ) );");
        }

        [Fact]
        public void DateDiff()
        {
            AssertSql(
                Sql.Select.Output(Sql.DurationInYears(Sql.Now(), Sql.Now())),
                "SELECT ( DATE_PART ( 'year', NOW ( ) ) - DATE_PART ( 'year', NOW ( ) ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInMonth(Sql.Now(), Sql.Now())),
                "SELECT ( ( DATE_PART ( 'year', NOW ( ) ) - DATE_PART ( 'year', NOW ( ) ) ) * 12 + ( DATE_PART ( 'month', NOW ( ) ) - DATE_PART ( 'month', NOW ( ) ) ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInDays(Sql.Now(), Sql.Now())),
                "SELECT ( ( DATE_PART ( 'day', NOW ( ) - NOW ( ) ) ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInWeeks(Sql.Now(), Sql.Now())),
                "SELECT ( TRUNC ( ( DATE_PART ( 'day', NOW ( ) - NOW ( ) ) ) / 7 ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInHours(Sql.Now(), Sql.Now())),
                "SELECT ( ( ( DATE_PART ( 'day', NOW ( ) - NOW ( ) ) ) ) * 24 + ( DATE_PART ( 'hour', NOW ( ) - NOW ( ) ) ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInMinutes(Sql.Now(), Sql.Now())),
                "SELECT ( ( ( ( DATE_PART ( 'day', NOW ( ) - NOW ( ) ) ) ) * 24 + ( DATE_PART ( 'hour', NOW ( ) - NOW ( ) ) ) ) * 60 + ( DATE_PART ( 'minute', NOW ( ) - NOW ( ) ) ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInSeconds(Sql.Now(), Sql.Now())),
                "SELECT ( ( ( ( ( DATE_PART ( 'day', NOW ( ) - NOW ( ) ) ) ) * 24 + ( DATE_PART ( 'hour', NOW ( ) - NOW ( ) ) ) ) * 60 + ( DATE_PART ( 'minute', NOW ( ) - NOW ( ) ) ) ) * 60 + ( DATE_PART ( 'second', NOW ( ) - NOW ( ) ) ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInMilliseconds(Sql.Now(), Sql.Now())),
                "SELECT ( ( ( ( ( ( DATE_PART ( 'day', NOW ( ) - NOW ( ) ) ) ) * 24 + ( DATE_PART ( 'hour', NOW ( ) - NOW ( ) ) ) ) * 60 + ( DATE_PART ( 'minute', NOW ( ) - NOW ( ) ) ) ) * 60 + ( DATE_PART ( 'second', NOW ( ) - NOW ( ) ) ) ) * 1000 + ( TRUNC ( ( DATE_PART ( 'microsecond', NOW ( ) - NOW ( ) ) ) / 1000 ) ) );"
                );
        }

        [Fact]
        public void Case()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.Case.When(Sql.Scalar("a").IsEqual(Sql.Scalar("b")), Sql.Scalar("a"))
                    .When(Sql.Scalar("a").NotEqual(Sql.Scalar("b")), Sql.Scalar("b"))
                    .Else(Sql.Scalar("c"))),
                "SELECT CASE WHEN 'a' = 'b' THEN 'a' WHEN 'a' <> 'b' THEN 'b' ELSE 'c' END;"
                );
        }

        [Fact]
        public void IIF()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.IIF(
                        Sql.Scalar("a").IsEqual(Sql.Scalar("b")),
                        Sql.Scalar("a"), Sql.Scalar("b"))),
                "SELECT CASE WHEN 'a' = 'b' THEN 'a' ELSE 'b' END;"
                );
        }

        [Fact]
        public void MakeDate3()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015, 1, 1)),
                "SELECT TIMESTAMP '2015-1-1 0:0:0';"
                );
        }

        [Fact]
        public void MakeDate5()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015, 1, 1, 13, 20)),
                "SELECT TIMESTAMP '2015-1-1 13:20:0';"
                );
        }

        [Fact]
        public void MakeDate6()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015, 1, 1, 13, 20, 50)),
                "SELECT TIMESTAMP '2015-1-1 13:20:50';"
                );
        }

        [Fact]
        public void MakeTime2()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeTime(21, 45)),
                "SELECT TIME '21:45:0';"
                );
        }
        
        [Fact]
        public void MakeTime3()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeTime(21, 45, 32)),
                "SELECT TIME '21:45:32';"
                );
        }

        [Fact]
        public void DateAdd()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeTime(21, 45, 32).AddDays(1).SubtractWeeks(1)),
                "SELECT TIME '21:45:32' + INTERVAL ' 1 day ' - INTERVAL ' 1 week ';"
                );
        }
    }
}
