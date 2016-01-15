using System.Collections.Generic;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTRider.FluidSql;

namespace FluidSqlTests
{
    [TestClass]
    public class FunctionsTest : SqlProviderTests
    {
        [TestMethod]
        public void Now()
        {
            AssertSql(
                Sql.Select.Output(Sql.Now(), Sql.GetDate(), Sql.Now(true), Sql.GetDate(true)),
                "SELECT GETDATE ( ), GETDATE ( ), GETUTCDATE ( ), GETUTCDATE ( );"
                );
        }

        [TestMethod]
        public void Uuid()
        {
            AssertSql(
                Sql.Select.Output(Sql.UUID(), Sql.NewId()),
                "SELECT NEWID ( ), NEWID ( );"
                );
        }

        [TestMethod]
        public void DatePart()
        {
            AssertSql(
                Sql.Select.Output(Sql.Year(Sql.Now())),
                "SELECT DATEPART ( yy, GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.Month(Sql.Now())),
                "SELECT DATEPART ( m, GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.Day(Sql.Now())),
                "SELECT DATEPART ( d, GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.Week(Sql.Now())),
                "SELECT DATEPART ( ww, GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.Hour(Sql.Now())),
                "SELECT DATEPART ( hh, GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.Minute(Sql.Now())),
                "SELECT DATEPART ( mi, GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.Second(Sql.Now())),
                "SELECT DATEPART ( ss, GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.Millisecond(Sql.Now())),
                "SELECT DATEPART ( ms, GETDATE ( ) );"
                );
        }

        [TestMethod]
        public void DateDiff()
        {
            AssertSql(
                Sql.Select.Output(Sql.DurationInYears(Sql.Now(), Sql.Now())),
                "SELECT DATEDIFF ( yy, GETDATE ( ), GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInMonth(Sql.Now(), Sql.Now())),
                "SELECT DATEDIFF ( m, GETDATE ( ), GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInDays(Sql.Now(), Sql.Now())),
                "SELECT DATEDIFF ( d, GETDATE ( ), GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInWeeks(Sql.Now(), Sql.Now())),
                "SELECT DATEDIFF ( ww, GETDATE ( ), GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInHours(Sql.Now(), Sql.Now())),
                "SELECT DATEDIFF ( hh, GETDATE ( ), GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInMinutes(Sql.Now(), Sql.Now())),
                "SELECT DATEDIFF ( mi, GETDATE ( ), GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInSeconds(Sql.Now(), Sql.Now())),
                "SELECT DATEDIFF ( ss, GETDATE ( ), GETDATE ( ) );"
                );
            AssertSql(
                Sql.Select.Output(Sql.DurationInMilliseconds(Sql.Now(), Sql.Now())),
                "SELECT DATEDIFF ( ms, GETDATE ( ), GETDATE ( ) );"
                );
        }

        [TestMethod]
        public void IIF()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.IIF(
                        Sql.Scalar("a").IsEqual(Sql.Scalar("b")),
                        Sql.Scalar("a"),Sql.Scalar("b")
                        )),
                "SELECT IIF ( N'a' = N'b', N'a', N'b' );"
                );
        }

        [TestMethod]
        public void Case()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.Case.When(Sql.Scalar("a").IsEqual(Sql.Scalar("b")),Sql.Scalar("a"))
                    .When(Sql.Scalar("a").NotEqual(Sql.Scalar("b")), Sql.Scalar("b"))
                    .Else(Sql.Scalar("c"))),
                "SELECT CASE WHEN N'a' = N'b' THEN N'a' WHEN N'a' <> N'b' THEN N'b' ELSE N'c' END;"
                );
        }

        [TestMethod]
        public void MakeDate3()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015,1,1)),
                "SELECT DATETIMEFROMPARTS ( 2015, 1, 1, 0, 0, 0, 0 );"
                );
        }

        [TestMethod]
        public void MakeDate5()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015, 1, 1, 13, 20)),
                "SELECT DATETIMEFROMPARTS ( 2015, 1, 1, 13, 20, 0, 0 );"
                );
        }

        [TestMethod]
        public void MakeDate6()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeDate(2015, 1, 1, 13, 20, 50)),
                "SELECT DATETIMEFROMPARTS ( 2015, 1, 1, 13, 20, 50, 0 );"
                );
        }

        [TestMethod]
        public void MakeTime2()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeTime(21, 45)),
                "SELECT TIMEFROMPARTS ( 21, 45, 0, 0, 0 );"
                );
        }

        [TestMethod]
        public void MakeTime3() 
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeTime(21, 45, 32)),
                "SELECT TIMEFROMPARTS ( 21, 45, 32, 0, 0 );"
                );
        }


        [TestMethod]
        public void DateAdd()
        {
            AssertSql(
                Sql.Select.Output(
                    Sql.MakeTime(21, 45, 32).AddDays(1).SubtractWeeks(1)),
                "SELECT DATEADD ( ww, - 1, DATEADD ( d, 1, TIMEFROMPARTS ( 21, 45, 32, 0, 0 ) ) );"
                );
        }
    }
}


